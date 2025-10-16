using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float speed = 10f;
    [Tooltip("폭발 반경 내의 각 타일에 입힐 데미지 양입니다.")]
    public int damagePerTile = 10;
    public float lifeTime = 3f;

    [Header("폭발 설정")]
    [Tooltip("데미지를 입힐 원의 반경입니다. 이 원 안의 모든 타일이 데미지를 받습니다.")]
    public float explosionRadius = 1.5f;

    [Header("이벤트 채널")]
    public TileDamageEventChannelSO onTileDamageChannel;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 주의: linearVelocity 대신 Unity의 최신 버전에서 권장되는 velocity를 사용합니다.
        // 우주선 방향(transform.up)의 역방향으로 속도를 설정합니다.
        rb.linearVelocity = transform.up * speed * -1;
        
        Destroy(gameObject, lifeTime);
    }

    // 💡 참고: 기존 Update 함수는 AoE 로직과 관련 없으며, 원점 근처에서 예상치 못한 파괴를 유발할 수 있어 제거했습니다.
    // 필요하다면 다시 추가할 수 있습니다.

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tilemap tilemap = collision.collider.GetComponent<Tilemap>();

        // 타일맵과 충돌했을 때만 폭발 처리를 진행합니다.
        if (tilemap != null)
        {
            // 1. 충돌 지점을 폭발의 중심 좌표로 설정합니다.
            Vector3 explosionCenterWorld = collision.GetContact(0).point;
            
            // 2. 타일맵의 유효 범위(Bounds)를 가져옵니다.
            BoundsInt bounds = tilemap.cellBounds;
            
            // 3. 타일맵의 모든 셀을 순회하며 폭발 반경 내에 있는지 확인합니다.
            foreach (var cellPos in bounds.allPositionsWithin)
            {
                // 현재 셀 위치에 타일이 실제로 있는지 확인합니다.
                if (!tilemap.HasTile(cellPos)) continue;
                
                // 타일 셀의 월드 좌표 중심을 가져옵니다.
                Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPos);
                
                // 4. 타일 중심과 폭발 중심 사이의 거리를 계산하여 반경 내에 있는지 확인합니다.
                if (Vector3.Distance(cellCenterWorld, explosionCenterWorld) <= explosionRadius)
                {
                    // 5. 폭발 반경 내에 있는 타일에 데미지 이벤트를 개별적으로 보냅니다.
                    TileDamageEvent damageEvent = new TileDamageEvent
                    {
                        cellPosition = cellPos,
                        damageAmount = this.damagePerTile // AoE 내의 모든 타일이 동일한 데미지를 받습니다.
                    };

                    if (onTileDamageChannel != null)
                    {
                        onTileDamageChannel.RaiseEvent(damageEvent);
                    }
                    // else에 대한 Debug.LogError는 매번 루프에서 발생하는 것을 막기 위해 생략했습니다.
                }
            }
            
            // 폭발 처리가 끝났으므로 총알은 파괴됩니다.
            Destroy(gameObject);
        }
        else
        {
             // 타일맵이 아닌 다른 물체(예: 플레이어)와 부딪혔다면, 즉시 파괴합니다.
             Destroy(gameObject);
        }
    }
}