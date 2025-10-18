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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// SpaceshipWeapon이 미사일을 생성한 직후 호출하여 초기 속도를 설정해주는 함수입니다.
    /// </summary>
    /// <param name="shipVelocity">미사일이 발사되는 순간의 우주선 속도</param>
    public void Initialize(Vector2 shipVelocity)
    {
        // ★ 핵심: 우주선의 현재 속도 + 미사일 자체의 발사 속도 = 최종 초기 속도
        rb.linearVelocity = shipVelocity + (Vector2)transform.up * speed;
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
            if (TilemapShadowGenerator.Instance != null)
            {
                        Vector3Int explosionCenterCell = targetAsteroid.myTilemap.WorldToCell(collision.GetContact(0).point);
                        float explosionRadius = SpaceshipWeapon.Instance.GetExplosionRadius();

                        // 월드 단위의 float 반경을 그대로 전달합니다.
                        TilemapShadowGenerator.Instance.UpdateShadowsAround(explosionCenterCell, explosionRadius);

            }
            Destroy(gameObject);

            }
        }
    }

