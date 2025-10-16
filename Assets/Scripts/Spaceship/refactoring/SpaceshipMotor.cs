using UnityEngine;

// 역할: Rigidbody2D를 제어하여 실제 우주선의 모든 물리적 움직임을 담당.
[RequireComponent(typeof(Rigidbody2D))]
public class SpaceshipMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float thrustPower = 2000f;
    [Tooltip("이동 저항값. 높을수록 최고 속도가 낮아지고 감속이 빨라집니다.")]
    [SerializeField] private float movementDrag = 0.5f;

    [Header("Rotation")]
    [SerializeField] private float additiveTorque = 10f;
    // --- 아래 한 줄 추가 ---
    [Tooltip("회전 저항값. 높을수록 회전이 빨리 멈춥니다.")]
    [SerializeField] private float angularDrag = 1f;

    public Rigidbody2D Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale = 0;
        
        Rb.linearDamping = movementDrag;
        // --- 아래 한 줄 추가 ---
        Rb.angularDamping = angularDrag; // Rigidbody2D의 각저항 값을 설정합니다.
    }

    public void Move(float thrustInput, float boostMultiplier)
    {
        if (thrustInput > 0f)
        {
            Rb.AddForce(transform.up * thrustPower * thrustInput * boostMultiplier, ForceMode2D.Force);
        }
    }

    public void Rotate(float rotateInput)
    {
        if (Mathf.Abs(rotateInput) > 0.1f)
        {
            Rb.AddTorque(-rotateInput * additiveTorque);
        }
    }
}