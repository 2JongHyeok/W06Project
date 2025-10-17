using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Planet: MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private MyTileData defaultTileData;
    [SerializeField] private float respawnDelay = 3f;

    // 각 타일별 상태 저장용
    private Dictionary<Vector3Int, int> tileHPs = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();

    void Start()
    {
        // 맵 전체 초기화
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                originalTiles[pos] = tilemap.GetTile(pos);
                tileHPs[pos] = defaultTileData.maxHP;
            }
        }
    }

    // 타일 데미지 처리
    public void DamageTile(Vector3Int cellPos, int damage)
    {
        if (!tileHPs.ContainsKey(cellPos)) return;
        tileHPs[cellPos] -= damage;

        if (tileHPs[cellPos] <= 0)
        {
            BreakTile(cellPos);
        }
    }

    private void BreakTile(Vector3Int cellPos)
    {
        tilemap.SetTile(cellPos, null); // 타일 제거
        StartCoroutine(RespawnTile(cellPos)); // 일정 시간 후 재생성
    }

    private IEnumerator RespawnTile(Vector3Int cellPos)
    {
        yield return new WaitForSeconds(respawnDelay);


        if (originalTiles.ContainsKey(cellPos))
        {
            tilemap.SetTile(cellPos, originalTiles[cellPos]);
            tileHPs[cellPos] = defaultTileData.maxHP;
        }
    }
}
