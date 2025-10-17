using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 내구도에 따른 색상 매핑 정보를 담는 클래스입니다.
/// </summary>
[System.Serializable]
public class DurabilityColorMapping
{
    [Tooltip("이 내구도 수치 '이상'일 경우 적용될 색상입니다.")]
    public int durabilityThreshold;
    [Tooltip("원래 타일 텍스처를 유지하려면 흰색에 가깝게 설정하세요.")]
    public Color color = Color.white;
}

/// <summary>
/// 여러 타일맵의 내구도를 관리하고, 데미지 이벤트를 수신하여 타일을 변경하거나 파괴합니다.
/// </summary>
public class TilemapManager : MonoBehaviour
{
    [Header("수동 설정 (선택 사항)")]
    [Tooltip("여기에 직접 추가한 타일맵들은 태그와 상관없이 관리 목록에 포함됩니다.")]
    public Tilemap[] targetTilemaps; // ✨ 유지: 수동으로 추가하는 배열

    [Header("일반 타일 색상 설정")]
    [Tooltip("내구도가 높은 순서대로 정렬하지 않아도 괜찮습니다. 자동으로 정렬됩니다.")]
    public DurabilityColorMapping[] colorMappings;

    [Header("이벤트 채널 구독")]
    public TileDamageEventChannelSO onTileDamageChannel;

    // ✨ 최종적으로 관리될 모든 타일맵 리스트 (수동 + 자동)
    private List<Tilemap> managedTilemaps = new List<Tilemap>();

    private Dictionary<Vector3Int, int> currentDurabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, int> maxDurabilityMap = new Dictionary<Vector3Int, int>();

    private void OnEnable()
    {
        if (onTileDamageChannel != null)
        {
            onTileDamageChannel.OnEventRaised += ReceiveDamage;
        }
    }

    private void OnDisable()
    {
        if (onTileDamageChannel != null)
        {
            onTileDamageChannel.OnEventRaised -= ReceiveDamage;
        }
    }

    void Start()
    {
        if (colorMappings != null && colorMappings.Length > 0)
        {
            colorMappings = colorMappings.OrderByDescending(mapping => mapping.durabilityThreshold).ToArray();
        }

        InitializeDurability();
    }

    /// <summary>
    /// 수동 및 자동으로 타일맵을 찾아 목록을 만들고, 내구도와 색상을 초기화합니다.
    /// </summary>
    void InitializeDurability()
    {
        // ✨ CORE CHANGE: 관리 리스트를 초기화하고 수동+자동으로 채웁니다.
        managedTilemaps.Clear();

        // 1. 수동으로 할당된 타일맵들을 먼저 리스트에 추가합니다.
        if (targetTilemaps != null && targetTilemaps.Length > 0)
        {
            managedTilemaps.AddRange(targetTilemaps.Where(t => t != null));
        }

        // 2. "Asteroid" 태그를 가진 모든 타일맵을 찾습니다.
        Tilemap[] taggedTilemaps = FindObjectsOfType<Tilemap>().Where(t => t.CompareTag("Asteroid")).ToArray();
        
        foreach (var taggedTilemap in taggedTilemaps)
        {
            // ✨ 3. 아직 관리 리스트에 없다면 (중복 방지) 추가합니다.
            if (!managedTilemaps.Contains(taggedTilemap))
            {
                managedTilemaps.Add(taggedTilemap);
            }
        }

        if (managedTilemaps.Count == 0)
        {
            Debug.LogWarning("관리할 타일맵이 없거나, 'Asteroid' 태그를 가진 타일맵을 찾을 수 없습니다.");
            return;
        }
        
        Debug.Log($"총 {managedTilemaps.Count}개의 타일맵을 찾아 초기화를 시작합니다.");

        // 최종적으로 정리된 'managedTilemaps' 리스트를 순회하며 초기화합니다.
        foreach (var tilemap in managedTilemaps)
        {
            if (tilemap == null) continue;
            
            tilemap.CompressBounds();

            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (!tilemap.HasTile(pos)) continue;

                var tileBase = tilemap.GetTile(pos);
                int maxDurability = 0;
                tilemap.SetTileFlags(pos, TileFlags.None);

                if (tileBase is MineralRuleTile mineralTile)
                {
                    maxDurability = mineralTile.maxDurability;
                    tilemap.SetColor(pos, mineralTile.mineralColor);
                }
                else if (tileBase is DurabilityRuleTile durabilityTile)
                {
                    maxDurability = durabilityTile.maxDurability;
                    tilemap.SetColor(pos, GetColorForDurability(maxDurability));
                }

                if (maxDurability > 0 && !maxDurabilityMap.ContainsKey(pos))
                {
                    maxDurabilityMap[pos] = maxDurability;
                    currentDurabilityMap[pos] = maxDurability;
                }
            }
        }
    }
    
    // ... (ReceiveDamage 함수는 변경 없음) ...
    private void ReceiveDamage(TileDamageEvent damageEvent)
    {
        DamageTile(damageEvent.cellPosition, damageEvent.damageAmount);
    }
    
    // ... (DamageTile 함수는 변경 없음, GetTilemapAtPosition만 수정) ...
    public void DamageTile(Vector3Int cellPosition, int damage)
    {
        if (!currentDurabilityMap.ContainsKey(cellPosition)) return;

        Tilemap targetTilemap = GetTilemapAtPosition(cellPosition);
        if (targetTilemap == null) return; 

        var tileBeingDamaged = targetTilemap.GetTile(cellPosition);
        int newDurability = currentDurabilityMap[cellPosition] - damage;
        currentDurabilityMap[cellPosition] = newDurability;

        if (newDurability <= 0)
        {
            if (tileBeingDamaged is MineralRuleTile mineralTile && mineralTile.itemDropPrefab != null)
            {
                Vector3 spawnPosition = targetTilemap.GetCellCenterWorld(cellPosition);
                Instantiate(mineralTile.itemDropPrefab, spawnPosition, Quaternion.identity);
            }

            targetTilemap.SetTile(cellPosition, null);
            currentDurabilityMap.Remove(cellPosition);
            maxDurabilityMap.Remove(cellPosition);
        }
        else
        {
            if (!(tileBeingDamaged is MineralRuleTile))
            {
                targetTilemap.SetColor(cellPosition, GetColorForDurability(newDurability));
            }
        }
    }
    
    private Tilemap GetTilemapAtPosition(Vector3Int cellPosition)
    {
        // ✨ 변경: 최종 관리 리스트에서 타일맵을 찾도록 수정
        foreach (var tilemap in managedTilemaps)
        {
            if (tilemap != null && tilemap.HasTile(cellPosition))
            {
                return tilemap;
            }
        }
        return null;
    }

    // ... (GetColorForDurability 함수는 변경 없음) ...
    private Color GetColorForDurability(int currentDurability)
    {
        if (colorMappings == null || colorMappings.Length == 0)
        {
            return Color.white;
        }

        foreach (var mapping in colorMappings)
        {
            if (currentDurability >= mapping.durabilityThreshold)
            {
                return mapping.color;
            }
        }
        
        return colorMappings.Last().color;
    }
}