using UnityEngine;

// 역할: Rigidbody2D를 제어하여 실제 우주선의 모든 물리적 움직임을 담당.
[RequireComponent(typeof(Rigidbody2D))]
public class SpaceshipMotor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float thrustPower = 2000f;
    [SerializeField] private float maxSpeed = 10000f;

    [Header("Rotation")]
    [SerializeField] private float directRotationSpeed = 30f;
    [SerializeField] private float additiveTorque = 10f;
    private const float VELOCITY_THRESHOLD = 0.01f;
    private const float ANGULAR_VELOCITY_THRESHOLD = 0.1f;
    private bool isDirectRotationActive = false;
    private float desiredRotation;

    public Rigidbody2D Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale = 0;
        desiredRotation = Rb.rotation;
    }

    public void Move(float thrustInput, float boostMultiplier)
    {
        if (thrustInput > 0f)
        {
            Rb.AddForce(transform.up * thrustPower * thrustInput * boostMultiplier, ForceMode2D.Force);
        }

        if (Rb.linearVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            Rb.linearVelocity = Rb.linearVelocity.normalized * maxSpeed;
        }
    }

    public void Rotate(float rotateInput)
    {
        bool isTurning = Mathf.Abs(rotateInput) > 0.1f;
        
        if (isTurning)
        {
            if (!isDirectRotationActive && Rb.linearVelocity.sqrMagnitude < VELOCITY_THRESHOLD && Mathf.Abs(Rb.angularVelocity) < ANGULAR_VELOCITY_THRESHOLD)
            {
                isDirectRotationActive = true;
                desiredRotation = Rb.rotation;
            }

            if (isDirectRotationActive)
            {
                desiredRotation += -rotateInput * directRotationSpeed * Time.fixedDeltaTime;
                Rb.MoveRotation(desiredRotation);
                Rb.angularVelocity = 0f;
            }
            else
            {
                Rb.AddTorque(-rotateInput * additiveTorque);
            }
        }
        else
        {
            isDirectRotationActive = false;
        }
    }
}