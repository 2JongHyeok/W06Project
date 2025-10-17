// ItemDrop.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ItemDrop : MonoBehaviour
{
    [Tooltip("아이템이 드랍된 후 자동으로 사라질 시간 (초)")]
    public float autoDestroyTime = 300f;

    /* 원점 중력 관련 변수들을 주석 처리 또는 삭제합니다.
    [Header("원점(0,0) 중력 설정")]
    [Tooltip("원점(0,0) 방향으로의 중력 가속도 (m/s^2)")]
    public float gravityAcceleration = 9.81f;

    [Tooltip("너무 가까워졌을 때 가속도 폭주를 막기 위한 최소 반지름 클램프")]
    public float minRadius = 0.05f;

    [Tooltip("역제곱 법칙 사용 여부 (행성처럼 가까울수록 더 강하게)")]
    public bool useInverseSquare = false;
    */

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 이 값을 1로 바꾸면 유니티의 기본 중력(아래 방향)이 적용됩니다.
        // 0으로 두면 중력 없이 그냥 그 자리에 떠 있습니다.
        rb.gravityScale = 0f;

        Destroy(gameObject, autoDestroyTime);
    }

    // 원점으로 끌어당기는 로직이 있던 FixedUpdate() 메서드를 완전히 제거했습니다.
}