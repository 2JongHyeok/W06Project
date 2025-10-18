using UnityEngine;

// 역할: Rigidbody2D를 제어하여 실제 우주선의 모든 물리적 움직임을 담당.
[RequireComponent(typeof(Rigidbody2D))]
public class SpaceshipMotor : MonoBehaviour
{
    [Header("Thrust Settings")]
    [SerializeField] private float thrustPower = 2000f;

    [Header("Inertia & Drag Settings")]
    [Tooltip("기본 저항값. 속도에 비례하며 최고 속도에 영향을 줍니다.")]
    [SerializeField] private float movementDrag = 0.5f;

    // --- 여기부터가 당신의 '소믈리에'를 위한 변수들입니다 ---
    [Header("Active Deceleration (The Brakes)")]
    [Tooltip("추진 입력이 없을 때, 반대 방향으로 가하는 제동력의 강도입니다.")]
    [Range(0f, 20f)]
    [SerializeField] private float stoppingPower = 5f;

    [Tooltip("추진 입력이 없을 때, 매 프레임 속도를 몇 퍼센트씩 줄일지 결정합니다. (1 = 미끄러짐 없음, 0.9 = 많이 미끄러짐)")]
    [Range(0.9f, 1f)]
    [SerializeField] private float glideReduction = 0.98f;

    [Tooltip("이 속도 이하로 떨어지면 강제로 멈춥니다. 미세한 움직임을 방지합니다.")]
    [SerializeField] private float stopThreshold = 0.1f;



    [Header("Rotational Inertia & Drag (회전 운동)")]
    [SerializeField] private float additiveTorque = 10f;
    [Tooltip("회전 저항값. 높을수록 회전이 빨리 멈춥니다.")]
    [SerializeField] private float angularDrag = 1f;


    [Range(0f, 20f)] [SerializeField] private float stoppingTorque = 5f;
    [Range(0.9f, 1f)] [SerializeField] private float rotationalGlideReduction = 0.95f;
    [SerializeField] private float angularStopThreshold = 0.1f;

    public Rigidbody2D Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale = 0;

        // Drag는 물리적으로 더 정확한 '항력'을 의미하므로, 그대로 사용합니다.
        Rb.linearDamping = movementDrag;
        Rb.angularDamping = angularDrag;
    }

    private void Update()
    {
        // 매 프레임, 인스펙터의 최신 값을 Rigidbody의 실제 물리 값으로 갱신합니다.
        // 이제 당신의 '소믈리에' 활동이 즉시 반영될 것이오.
        // 참고: linearDamping보다 drag가 더 정확한 물리 용어라, 그것으로 교체했소.
        Rb.linearDamping = movementDrag;
        Rb.angularDamping = angularDrag;
    }


    // Move 함수는 이제 오직 '가속'만을 담당합니다.
    public void Move(float thrustInput, float boostMultiplier)
    {
        if (Mathf.Abs(thrustInput) > 0.01f)
        {
            Rb.AddForce(transform.up * thrustPower * thrustInput * boostMultiplier, ForceMode2D.Force);
        }
    }

    // Rotate 함수는 변경할 필요가 없습니다.
    public void Rotate(float rotateInput)
    {
        if (Mathf.Abs(rotateInput) > 0.1f)
        {
            Rb.AddTorque(-rotateInput * additiveTorque);
        }
    }

    // --- 이 새로운 함수가 바로 '하이브리드 제동'의 핵심입니다 ---
    public void ApplyActiveDeceleration(float thrustInput)
    {
        // 1. 추진 입력이 없을 때만 제동 로직을 실행합니다.
        if (Mathf.Abs(thrustInput) < 0.1f)
        {
            // 2. 방법 A: 현재 속도의 반대 방향으로 '제동력'을 가합니다.
            if (Rb.linearVelocity.sqrMagnitude > 0) // 움직이고 있을 때만
            {
                Vector2 counterForce = -Rb.linearVelocity.normalized * stoppingPower;
                Rb.AddForce(counterForce, ForceMode2D.Force);
            }

            // 3. 방법 C: 현재 속도를 매 프레임 '비율'로 감소시킵니다.
            Rb.linearVelocity *= glideReduction;

            // 4. 속도가 거의 0에 가까워지면, 강제로 멈춰서 미세한 떨림을 방지합니다.
            if (Rb.linearVelocity.magnitude < stopThreshold)
            {
                Rb.linearVelocity = Vector2.zero;
            }
        }
    }
    public void ApplyActiveRotationalDeceleration(float rotateInput)
    {
        if (Mathf.Abs(rotateInput) < 0.1f)
        {
            if (Mathf.Abs(Rb.angularVelocity) > 0)
            {
                // 현재 회전 방향의 반대로 '제동 토크'를 가합니다.
                float counterTorque = -Mathf.Sign(Rb.angularVelocity) * stoppingTorque;
                Rb.AddTorque(counterTorque);
            }
            // 현재 각속도를 '비율'로 감소시킵니다.
            Rb.angularVelocity *= rotationalGlideReduction;
            if (Mathf.Abs(Rb.angularVelocity) < angularStopThreshold)
            {
                Rb.angularVelocity = 0f;
            }
        }
    }

}