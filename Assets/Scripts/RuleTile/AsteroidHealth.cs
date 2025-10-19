using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class AsteroidHealth : MonoBehaviour
{
    [Header("공유 설정")]
    [Tooltip("모든 소행성이 공유할 색상 설정 SO 파일을 연결해주세요.")]
    public DurabilityColorSettingsSO colorSettings;

    public Tilemap myTilemap { get; private set; }

    private Dictionary<Vector3Int, int> currentDurabilityMap = new Dictionary<Vector3Int, int>();
    private Dictionary<Vector3Int, int> maxDurabilityMap = new Dictionary<Vector3Int, int>();

    void Awake()
    {
        myTilemap = GetComponent<Tilemap>();
        if (myTilemap == null)
        {
            Debug.LogError("AsteroidHealth: 이 게임오브젝트에서 Tilemap 컴포넌트를 찾을 수 없습니다!", gameObject);
        }
    }

    void Start()
    {
        InitializeDurability();
    }
    
    void InitializeDurability()
    {
        if (myTilemap == null) return;
        if (colorSettings == null)
        {
            Debug.LogError("AsteroidHealth: colorSettings SO가 할당되지 않았습니다!", gameObject);
            return;
        }

        myTilemap.CompressBounds();

        // 1. 비어있는 리스트를 먼저 생성합니다.
        List<Vector3Int> positions = new List<Vector3Int>();
        
        // 2. foreach 루프를 돌면서 모든 위치를 리스트에 직접 추가합니다.
        foreach (var pos in myTilemap.cellBounds.allPositionsWithin)
        {
            positions.Add(pos);
        }

        foreach (var pos in positions)
            {
                if (!myTilemap.HasTile(pos)) continue;
                
                TileBase tileBase = myTilemap.GetTile(pos);

                // ✨ --- 여기가 핵심 로직! --- ✨
                // 1. 만약 현재 타일이 '랜덤 스포너 타일'이라면?
                if (tileBase is RandomizedSpawnerTile spawnerTile)
                {
                    // 2. 스포너에게서 확률에 따른 결과 타일을 받아옵니다.
                    TileBase newTile = spawnerTile.GetRandomOutcome();

                    if (newTile != null)
                    {
                        // 3. 현재 위치의 '스포너 타일'을 받아온 '결과 타일'로 교체합니다!
                        myTilemap.SetTile(pos, newTile);
                        // 4. 방금 교체한 새 타일로 tileBase 변수를 업데이트하여, 아래의 기존 로직이 처리할 수 있도록 합니다.
                        tileBase = newTile;
                    }
                    else
                    {
                        // 변할 타일이 없으면 그냥 지워버립니다.
                        myTilemap.SetTile(pos, null);
                        continue; // 아래 로직을 실행할 필요가 없으므로 다음 칸으로 넘어갑니다.
                    }
                }
            int maxDurability = 0;
            myTilemap.SetTileFlags(pos, TileFlags.None);

            if (tileBase is MineralRuleTile mineralTile)
            {
                maxDurability = mineralTile.maxDurability;
                myTilemap.SetColor(pos, mineralTile.mineralColor);
            }
            else if (tileBase is DurabilityRuleTile durabilityTile)
            {
                maxDurability = durabilityTile.maxDurability;
                // 이제 색상 정보를 SO에서 직접 가져옵니다.
                myTilemap.SetColor(pos, colorSettings.GetColorForDurability(maxDurability));
            }

            if (maxDurability > 0)
            {
                maxDurabilityMap[pos] = maxDurability;
                currentDurabilityMap[pos] = maxDurability;
            }
        }
    }
    
    public void ApplyDamage(Vector3Int cellPosition, int damage)
    {
        if (!currentDurabilityMap.ContainsKey(cellPosition)) return;

        var tileBeingDamaged = myTilemap.GetTile(cellPosition);
        int newDurability = currentDurabilityMap[cellPosition] - damage;
        currentDurabilityMap[cellPosition] = newDurability;

        if (newDurability <= 0)
        {
            if (tileBeingDamaged is MineralRuleTile mineralTile && mineralTile.itemDropPrefab != null)
            {
                Vector3 spawnPosition = myTilemap.GetCellCenterWorld(cellPosition);
                Instantiate(mineralTile.itemDropPrefab, spawnPosition, Quaternion.identity);
            }

            myTilemap.SetTile(cellPosition, null);
            currentDurabilityMap.Remove(cellPosition);
            maxDurabilityMap.Remove(cellPosition);
        }
        else
        {
            if (!(tileBeingDamaged is MineralRuleTile))
            {
                // 데미지를 입었을 때도 색상 정보를 SO에서 가져옵니다.
                myTilemap.SetColor(cellPosition, colorSettings.GetColorForDurability(newDurability));
            }
        }
    }
}