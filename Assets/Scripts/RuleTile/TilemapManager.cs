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
/// 타일맵의 내구도를 관리하고, 데미지 이벤트를 수신하여 타일을 변경하거나 파괴합니다.
/// </summary>
public class TilemapManager : MonoBehaviour
{
    [Header("필수 설정")]
    public Tilemap mainTilemap;

    [Header("일반 타일 색상 설정")]
    [Tooltip("내구도가 높은 순서대로 정렬하지 않아도 괜찮습니다. 자동으로 정렬됩니다.")]
    public DurabilityColorMapping[] colorMappings;

    [Header("이벤트 채널 구독")]
    public TileDamageEventChannelSO onTileDamageChannel;

    private Dictionary<Vector3Int, int> currentDurabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, int> maxDurabilityMap = new Dictionary<Vector3Int, int>();

    private void OnEnable()
    {
        // 이벤트 채널을 구독합니다.
        if (onTileDamageChannel != null)
        {
            onTileDamageChannel.OnEventRaised += ReceiveDamage;
        }
    }

    private void OnDisable()
    {
        // 이벤트 채널 구독을 취소하여 메모리 누수를 방지합니다.
        if (onTileDamageChannel != null)
        {
            onTileDamageChannel.OnEventRaised -= ReceiveDamage;
        }
    }

    void Start()
    {
        // 사용자가 입력한 색상 매핑을 내구도 역순으로 자동 정렬합니다.
        if (colorMappings != null && colorMappings.Length > 0)
        {
            colorMappings = colorMappings.OrderByDescending(mapping => mapping.durabilityThreshold).ToArray();
        }

        InitializeDurability();
    }

    /// <summary>
    /// 게임 시작 시 타일맵을 스캔하여 모든 타일의 내구도 데이터를 초기화합니다.
    /// </summary>
/// <summary>
    /// 게임 시작 시 타일맵을 스캔하여 모든 타일의 내구도와 색상을 초기화합니다.
    /// </summary>
    void InitializeDurability()
    {
        foreach (var pos in mainTilemap.cellBounds.allPositionsWithin)
        {
            if (!mainTilemap.HasTile(pos)) continue;

            var tileBase = mainTilemap.GetTile(pos);
            int maxDurability = 0;

            // 런타임에 색상을 변경하기 위해 타일 플래그를 먼저 해제합니다.
            mainTilemap.SetTileFlags(pos, TileFlags.None);

            // 타일의 종류를 확인하여 내구도와 색상을 설정합니다.
            if (tileBase is MineralRuleTile mineralTile)
            {
                // MineralRuleTile일 경우
                maxDurability = mineralTile.maxDurability;

                // ✨ 핵심: 광물 애셋에 저장된 고유 색상(mineralColor)을 가져와 적용합니다.
                mainTilemap.SetColor(pos, mineralTile.mineralColor);
            }
            else if (tileBase is DurabilityRuleTile durabilityTile)
            {
                // 일반 DurabilityRuleTile일 경우
                maxDurability = durabilityTile.maxDurability;

                // 내구도 수치에 맞는 색상을 매핑에서 찾아 적용합니다.
                mainTilemap.SetColor(pos, GetColorForDurability(maxDurability));
            }

            // 유효한 내구도 값을 가진 타일만 맵에 등록합니다.
            if (maxDurability > 0)
            {
                maxDurabilityMap[pos] = maxDurability;
                currentDurabilityMap[pos] = maxDurability;
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

        // 데미지를 적용하기 전에 타일 정보를 미리 가져옵니다.
        var tileBeingDamaged = mainTilemap.GetTile(cellPosition);
        int newDurability = currentDurabilityMap[cellPosition] - damage;
        currentDurabilityMap[cellPosition] = newDurability;

        if (newDurability <= 0)
        {
            // 내구도가 0 이하가 되면 타일을 파괴합니다.
            // 파괴되는 타일이 광물인지 확인합니다.
            if (tileBeingDamaged is MineralRuleTile mineralTile && mineralTile.itemDropPrefab != null)
            {
                // 설정된 아이템이 있다면, 타일 중앙 위치에 생성합니다.
                Vector3 spawnPosition = mainTilemap.GetCellCenterWorld(cellPosition);
                Instantiate(mineralTile.itemDropPrefab, spawnPosition, Quaternion.identity);
            }

            // 타일을 맵에서 제거하고 데이터를 삭제합니다.
            mainTilemap.SetTile(cellPosition, null);
            currentDurabilityMap.Remove(cellPosition);
            maxDurabilityMap.Remove(cellPosition);
        }
        else
        {
            // 타일이 파괴되지 않았다면, 상태를 업데이트합니다.
            // 광물 타일이 아닐 경우에만 내구도에 따라 색상을 변경합니다.
            if (!(tileBeingDamaged is MineralRuleTile))
            {
                mainTilemap.SetColor(cellPosition, GetColorForDurability(newDurability));
            }
        }
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