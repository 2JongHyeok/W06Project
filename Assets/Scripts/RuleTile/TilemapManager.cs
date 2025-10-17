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
    [Header("필수 설정")]
    [Tooltip("내구도를 관리할 모든 타일맵을 여기에 등록하세요.")]
    public Tilemap[] targetTilemaps; // ✨ CHANGED: 단일 타일맵에서 배열로 변경

    [Header("일반 타일 색상 설정")]
    [Tooltip("내구도가 높은 순서대로 정렬하지 않아도 괜찮습니다. 자동으로 정렬됩니다.")]
    public DurabilityColorMapping[] colorMappings;

    [Header("이벤트 채널 구독")]
    public TileDamageEventChannelSO onTileDamageChannel;

    // 모든 타일맵의 내구도 데이터를 하나의 딕셔너리에서 관리합니다.
    // (타일맵들이 서로 겹치지 않는다는 가정 하에 효율적입니다.)
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
    /// 게임 시작 시 등록된 모든 타일맵을 스캔하여 타일의 내구도와 색상을 초기화합니다.
    /// </summary>
    void InitializeDurability()
    {
        if (targetTilemaps == null || targetTilemaps.Length == 0)
        {
            Debug.LogWarning("관리할 타일맵이 지정되지 않았습니다.");
            return;
        }

        // 등록된 모든 타일맵을 순회합니다.
        foreach (var tilemap in targetTilemaps)
        {
            if (tilemap == null) continue;

            // ✨ OPTIMIZATION: 타일맵의 경계를 실제 타일이 있는 영역으로 압축합니다.
            // 이렇게 하면 비어있는 공간을 검색하는 것을 방지하여 성능이 크게 향상됩니다.
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

                if (maxDurability > 0)
                {
                    // 딕셔너리에 이미 키가 있는지 확인 (겹치는 타일맵의 경우)
                    if (!maxDurabilityMap.ContainsKey(pos))
                    {
                        maxDurabilityMap[pos] = maxDurability;
                        currentDurabilityMap[pos] = maxDurability;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 이벤트 채널로부터 데미지 정보를 수신하는 함수입니다.
    /// </summary>
    private void ReceiveDamage(TileDamageEvent damageEvent)
    {
        DamageTile(damageEvent.cellPosition, damageEvent.damageAmount);
    }

    /// <summary>
    /// 특정 위치의 타일에 데미지를 적용하고, 상태를 업데이트합니다.
    /// </summary>
    public void DamageTile(Vector3Int cellPosition, int damage)
    {
        if (!currentDurabilityMap.ContainsKey(cellPosition)) return;

        // ✨ NEW: 해당 위치에 타일을 가지고 있는 타일맵을 찾습니다.
        Tilemap targetTilemap = GetTilemapAtPosition(cellPosition);
        if (targetTilemap == null) return; // 타일맵을 찾지 못하면 아무것도 하지 않음

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
    
    /// <summary>
    /// 주어진 셀 위치에 타일을 가지고 있는 타일맵을 찾아 반환합니다.
    /// </summary>
    /// <returns>타일맵을 찾으면 해당 Tilemap 객체를, 못 찾으면 null을 반환합니다.</returns>
    private Tilemap GetTilemapAtPosition(Vector3Int cellPosition)
    {
        foreach (var tilemap in targetTilemaps)
        {
            if (tilemap != null && tilemap.HasTile(cellPosition))
            {
                return tilemap;
            }
        }
        return null;
    }


    /// <summary>
    /// 현재 내구도 수치에 맞는 색상을 찾아 반환합니다.
    /// </summary>
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