// ItemDrop.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ItemDrop : MonoBehaviour
{
    [Tooltip("아이템이 드랍된 후 자동으로 사라질 시간 (초)")]
    public float autoDestroyTime = 300f;

    [Header("원점(0,0) 중력 설정")]
    [Tooltip("원점(0,0) 방향으로의 중력 가속도 (m/s^2)")]
    public float gravityAcceleration = 9.81f;

    [Tooltip("너무 가까워졌을 때 가속도 폭주를 막기 위한 최소 반지름 클램프")]
    public float minRadius = 0.05f;

    [Tooltip("역제곱 법칙 사용 여부 (행성처럼 가까울수록 더 강하게)")]
    public bool useInverseSquare = false;   // false면 일정한 속도로.

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;

        Destroy(gameObject, autoDestroyTime);
    }

    void FixedUpdate()
    {
        Vector2 toOrigin = -(Vector2)transform.position;
        float dist = toOrigin.magnitude;

        if (dist < Mathf.Epsilon) return;

        Vector2 dir = toOrigin / Mathf.Max(dist, 1e-6f);

        float accel = gravityAcceleration;

        if (useInverseSquare)
        {
            float r = Mathf.Max(dist, minRadius);
            accel = gravityAcceleration / (r * r);
        }

        Vector2 force = dir * (rb.mass * accel);
        rb.AddForce(force, ForceMode2D.Force);
    }
}
