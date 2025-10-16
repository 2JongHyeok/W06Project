using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    [Header("총알 설정")]
    public float speed = 10f;
    public int damage = 10;
    public float lifeTime = 3f;

    [Header("이벤트 채널")]
    public TileDamageEventChannelSO onTileDamageChannel;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * speed * -1;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // (0,0,0)와 거리가 0.1 이하이면 총알 삭제
        if (Vector3.Distance(transform.position, Vector3.zero) <= 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tilemap tilemap = collision.collider.GetComponent<Tilemap>();

        if (tilemap != null)
        {
            Vector3 hitPosition = collision.GetContact(0).point;
            Vector3 normalV3 = collision.GetContact(0).normal;
            Vector3Int cellPos = tilemap.WorldToCell(hitPosition - normalV3 * 0.01f);

            TileDamageEvent damageEvent = new TileDamageEvent
            {
                cellPosition = cellPos,
                damageAmount = this.damage
            };

            if (onTileDamageChannel != null)
            {
                onTileDamageChannel.RaiseEvent(damageEvent);
            }
            else
            {
                Debug.LogError("OnTileDamageChannel이 총알에 할당되지 않았습니다!");
            }

            Destroy(gameObject);
        }
    }
}
