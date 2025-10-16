// TilemapManager.cs
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq; // Linq를 사용하기 위함

// TilemapManager.cs 상단에 추가
[System.Serializable]
public class DurabilityColorMapping
{
    [Tooltip("이 내구도 수치 '이상'일 경우 적용될 색상입니다.")]
    public int durabilityThreshold;
    [Tooltip("원래 타일 텍스처를 유지하려면 흰색에 가깝게 설정하세요.")]
    public Color color = Color.white;
}

public class TilemapManager : MonoBehaviour
{
    [Header("필수 설정")]
    public Tilemap mainTilemap;

    [Header("절대 수치 내구도 색상 설정")]
    [Tooltip("내구도가 높은 순서대로 정렬하지 않아도 괜찮습니다. 자동으로 정렬됩니다.")]
    public DurabilityColorMapping[] colorMappings;

    private Dictionary<Vector3Int, int> currentDurabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, int> maxDurabilityMap = new Dictionary<Vector3Int, int>();

    void Start()
    {
        // ✨ 중요: 사용자가 순서에 상관없이 입력해도, 내구도 높은 순으로 자동 정렬합니다.
        if (colorMappings != null && colorMappings.Length > 0)
        {
            colorMappings = colorMappings.OrderByDescending(mapping => mapping.durabilityThreshold).ToArray();
        }

        InitializeDurability();
    }

    void InitializeDurability()
    {
        foreach (var pos in mainTilemap.cellBounds.allPositionsWithin)
        {
            if (!mainTilemap.HasTile(pos)) continue;

            DurabilityRuleTile tile = mainTilemap.GetTile<DurabilityRuleTile>(pos);
            if (tile != null)
            {
                maxDurabilityMap[pos] = tile.maxDurability;
                currentDurabilityMap[pos] = tile.maxDurability;

                mainTilemap.SetTileFlags(pos, TileFlags.None);
                // ✨ 타일의 최대 내구도에 맞는 시작 색상을 찾아 적용
                mainTilemap.SetColor(pos, GetColorForDurability(tile.maxDurability));
            }
        }
    }

    /// <summary>
    /// 현재 내구도 수치에 맞는 색상을 찾아 반환하는 함수
    /// </summary>
    private Color GetColorForDurability(int currentDurability)
    {
        if (colorMappings == null || colorMappings.Length == 0)
        {
            return Color.white; // 설정된 색상이 없으면 흰색 반환
        }

        // 정렬된 배열을 순회하며 조건에 맞는 첫 번째 색상을 찾음
        foreach (var mapping in colorMappings)
        {
            if (currentDurability >= mapping.durabilityThreshold)
            {
                return mapping.color; // "문턱값" 이상인 첫 색상을 반환
            }
        }

        // 모든 문턱값보다 내구도가 낮으면, 가장 낮은 단계의 색상으로 처리
        return colorMappings.Last().color;
    }

    public void DamageTile(Vector3Int cellPosition, int damage)
    {
        if (!currentDurabilityMap.ContainsKey(cellPosition)) return;

        int newDurability = currentDurabilityMap[cellPosition] - damage;
        currentDurabilityMap[cellPosition] = newDurability;

        if (newDurability <= 0)
        {
            mainTilemap.SetTile(cellPosition, null);
            currentDurabilityMap.Remove(cellPosition);
            maxDurabilityMap.Remove(cellPosition);
        }
        else
        {
            // ✨ 현재 내구도 수치에 맞는 색상을 찾아 적용
            mainTilemap.SetColor(cellPosition, GetColorForDurability(newDurability));
        }
    }
}