using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class WorldGenerator : MonoBehaviour
{
    [Header("핵심 연결 요소")]
    [Tooltip("모든 소행성이 그려질 메인 월드 타일맵")]
    [SerializeField] private Tilemap worldTilemap;

    [Header("월드 생성 설정")]
    [Tooltip("중심(0,0)에서부터 생성할 월드의 전체 반경")]
    [SerializeField] private float generationRadius = 1000f;

    [Tooltip("소행성을 배치할 격자의 크기. 작을수록 촘촘하게 검사합니다.")]
    [SerializeField] private int gridCellSize = 30;

    [Header("구역(Zone) 설정")]
    [Tooltip("생성할 모든 구역의 설정값(SO)들을 여기에 등록하세요.")]
    [SerializeField] private List<GenerationZoneSettingsSO> zoneSettings;

    // 게임이 시작될 때 월드 생성을 자동으로 실행합니다.
    void Start()
    {
        GenerateWorld();
    }

    /// <summary>
    /// 절차적 월드 생성을 시작하는 메인 함수입니다.
    /// </summary>
    public void GenerateWorld()
    {
        Debug.Log("월드 생성을 시작합니다...");
        // 테스트를 위해 기존 타일을 모두 지웁니다.
        worldTilemap.ClearAllTiles();

        // generationRadius와 gridCellSize에 따라 격자를 순회합니다.
        for (float x = -generationRadius; x < generationRadius; x += gridCellSize)
        {
            for (float y = -generationRadius; y < generationRadius; y += gridCellSize)
            {
                Vector2 currentPosition = new Vector2(x, y);

                // 1. 현재 위치가 어떤 구역에 속하는지 확인합니다.
                float distanceFromCenter = Vector2.Distance(currentPosition, Vector2.zero);
                GenerationZoneSettingsSO currentZone = GetZoneForDistance(distanceFromCenter);
                
                // 유효한 구역이 아니면 (예: 중심의 빈 공간) 건너뜁니다.
                if (currentZone == null) continue;

                // 2. 이 위치에 소행성을 생성할지 확률(spawnChance)에 따라 결정합니다.
                if (Random.value > currentZone.spawnChance) continue;

                // 3. 이 구역의 소행성 풀에서 어떤 소행성을 생성할지 확률(weight)에 따라 선택합니다.
                GameObject asteroidPrefabToSpawn = SelectRandomAsteroid(currentZone.asteroidPool);
                if (asteroidPrefabToSpawn == null) continue;
                
                // 4. 생성하기 전에, 해당 위치가 비어있는지 확인합니다. (겹침 방지)
                CircleCollider2D prefabCollider = asteroidPrefabToSpawn.GetComponent<CircleCollider2D>();
                if (prefabCollider == null)
                {
                    Debug.LogWarning($"{asteroidPrefabToSpawn.name}에 CircleCollider2D가 없어 겹침 확인을 건너뜁니다.");
                }
                else if (Physics2D.OverlapCircle(currentPosition, prefabCollider.radius))
                {
                    // 이미 무언가 있다면 건너뜁니다.
                    continue; 
                }

                // 5. 모든 조건을 통과했으면, 소행성을 월드 타일맵에 '도장'처럼 찍습니다.
                StampAsteroid(currentPosition, asteroidPrefabToSpawn);
            }
        }
        Debug.Log("월드 생성 완료!");
    }

    /// <summary>
    /// 주어진 거리에 해당하는 구역 설정(SO)을 찾아 반환합니다.
    /// </summary>
    private GenerationZoneSettingsSO GetZoneForDistance(float distance)
    {
        foreach (var zone in zoneSettings)
        {
            if (distance >= zone.minDistance && distance < zone.maxDistance)
            {
                return zone;
            }
        }
        return null; // 해당하는 구역이 없으면 null을 반환합니다.
    }

    /// <summary>
    /// 주어진 소행성 풀에서 설정된 가중치에 따라 무작위로 소행성 프리팹 하나를 선택합니다.
    /// </summary>
    private GameObject SelectRandomAsteroid(List<AsteroidSpawnData> pool)
    {
        if (pool == null || pool.Count == 0) return null;

        float totalWeight = pool.Sum(data => data.weight);
        float randomValue = Random.Range(0, totalWeight);

        foreach (var data in pool)
        {
            if (randomValue <= data.weight)
            {
                return data.asteroidPrefab;
            }
            randomValue -= data.weight;
        }
        return null;
    }

    /// <summary>
    /// 선택된 소행성 프리팹의 모양을 월드 타일맵의 특정 위치에 그대로 복사합니다.
    /// </summary>
    private void StampAsteroid(Vector2 worldPosition, GameObject asteroidPrefab)
    {
        Tilemap prefabTilemap = asteroidPrefab.GetComponentInChildren<Tilemap>();
        if (prefabTilemap == null)
        {
            Debug.LogError($"{asteroidPrefab.name} 프리팹 안에 Tilemap이 없습니다!");
            return;
        }

        // 프리팹 타일맵의 모든 타일 정보를 순회합니다.
        foreach (var pos in prefabTilemap.cellBounds.allPositionsWithin)
        {
            if (prefabTilemap.HasTile(pos))
            {
                TileBase tile = prefabTilemap.GetTile(pos);
                // 월드 타일맵에 찍힐 최종 위치를 계산합니다.
                // (생성 위치 + 타일의 상대 위치)
                Vector3Int targetPos = worldTilemap.WorldToCell(worldPosition) + pos;
                worldTilemap.SetTile(targetPos, tile);
            }
        }
    }
}