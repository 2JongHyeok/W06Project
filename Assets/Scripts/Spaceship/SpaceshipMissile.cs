using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceshipMissile : MonoBehaviour
{
    [Header("기본 설정")]
    public float speed = 15f;
    public float lifeTime = 5f;

    // 더 이상 이벤트 채널을 사용하지 않습니다.
    // [Header("이벤트 채널")]
    // public TileDamageEventChannelSO onTileDamageChannel;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트 또는 그 부모에서 AsteroidHealth 컴포넌트를 찾습니다.
        AsteroidHealth targetAsteroid = collision.collider.GetComponentInParent<AsteroidHealth>();

        // AsteroidHealth를 가진 소행성과 충돌했을 때만 로직을 실행합니다.
        if (targetAsteroid != null)
        {
            Vector3 explosionCenterWorld = collision.GetContact(0).point;
            
            // 데미지를 입힐 타일맵은 이제 충돌한 소행성이 직접 알려줍니다.
            Tilemap targetTilemap = targetAsteroid.myTilemap;
            
            targetTilemap.CompressBounds();
            BoundsInt bounds = targetTilemap.cellBounds;
            
            foreach (var cellPos in bounds.allPositionsWithin)
            {
                if (!targetTilemap.HasTile(cellPos)) continue;
                
                Vector3 cellCenterWorld = targetTilemap.GetCellCenterWorld(cellPos);
                
                // 폭발 범위 내에 있는지 확인
                if (Vector3.Distance(cellCenterWorld, explosionCenterWorld) <= SpaceshipWeapon.Instance.GetExplosionRadius())
                {
                    // 이벤트 방송 대신, 타겟 소행성의 ApplyDamage 함수를 직접 호출합니다.
                    targetAsteroid.ApplyDamage(cellPos, SpaceshipWeapon.Instance.GetDamage());
                }
            }
            
            Destroy(gameObject); // 폭발 후 미사일 파괴
        }
    }
}

