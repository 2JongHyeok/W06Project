using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float speed = 10f;
    [Tooltip("폭발 반경 내의 각 타일에 입힐 데미지 양입니다.")]
    public float lifeTime = 3f;

    

    [Header("이벤트 채널")]
    public TileDamageEventChannelSO onTileDamageChannel;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
        
        Destroy(gameObject, lifeTime);
    }


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
                if (Vector3.Distance(cellCenterWorld, explosionCenterWorld) <= Weapon.Instance.GetExplosionRange())
                {
                    // 5. 폭발 반경 내에 있는 타일에 데미지 이벤트를 개별적으로 보냅니다.
                    TileDamageEvent damageEvent = new TileDamageEvent
                    {
                        cellPosition = cellPos,
                        damageAmount = Weapon.Instance.GetDamage()
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
    }
}