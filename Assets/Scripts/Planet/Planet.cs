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
    private readonly Dictionary<Vector2Int, int> tileHPs = new();
    private readonly Dictionary<Vector2Int, TileBase> originalTiles = new();

    void Start()
    {
        // 맵 전체 초기화
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                var key = new Vector2Int(pos.x, pos.y);
                originalTiles[key] = tilemap.GetTile(pos);
                tileHPs[key] = defaultTileData.maxHP;
            }
        }
    }

    // 타일 데미지 처리
    public void DamageTile(Vector3Int cellPos, int damage)
    {
        Vector2Int newCellPos = new Vector2Int(cellPos.x, cellPos.y);
        cellPos.z = tilemap.origin.z;
        if (!tileHPs.ContainsKey(newCellPos)) return;

        tileHPs[newCellPos] -= damage;

        if (tileHPs[newCellPos] <= 0)
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
        Vector2Int newCellPos = new Vector2Int(cellPos.x, cellPos.y);

        if (originalTiles.ContainsKey(newCellPos))
        {
            tilemap.SetTile(cellPos, originalTiles[newCellPos]);
            tileHPs[newCellPos] = defaultTileData.maxHP;
        }
    }
}
