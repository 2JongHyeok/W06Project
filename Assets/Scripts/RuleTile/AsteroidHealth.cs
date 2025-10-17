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
        foreach (var pos in myTilemap.cellBounds.allPositionsWithin)
        {
            if (!myTilemap.HasTile(pos)) continue;
            
            var tileBase = myTilemap.GetTile(pos);
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