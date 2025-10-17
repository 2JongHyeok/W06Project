using UnityEngine;
using UnityEngine.InputSystem;

// 역할: New Input System의 입력을 받아 다른 컴포넌트가 사용할 수 있도록 값을 제공.
[RequireComponent(typeof(PlayerInput))]
public class SpaceshipInput : MonoBehaviour
{
    public float ThrustInput { get; private set; }
    public float RotateInput { get; private set; }
    public bool IsBoosting { get; private set; }
    public bool ToggleControlPressed { get; private set; }

    private PlayerInput playerInput;
    private InputAction thrustAction, boostAction, rotateAction, toggleControlAction;
    private InputAction reverseThrustAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        thrustAction = playerInput.actions["Thrust"];
        boostAction = playerInput.actions["Boost"];
        rotateAction = playerInput.actions["Rotate"];
        toggleControlAction = playerInput.actions["ToggleControl"];
        reverseThrustAction = playerInput.actions["ReverseThrust"];
    }

    private void Update()
    {
        // thrustAction이 Button 타입이므로, IsPressed()로 눌림 상태를 확인하고
        // 눌렸으면 1.0f, 아니면 0.0f를 ThrustInput에 할당합니다.
        if (thrustAction.IsPressed())
        {
            ThrustInput = 1.0f; // W 누르면 전진
        }
        else if (reverseThrustAction.IsPressed())
        {
            ThrustInput = -1.0f; // S 누르면 후진
        }
        else
        {
            ThrustInput = 0.0f; // 아무것도 안 누르면 0
        }
        
        // boostAction도 Button 타입이므로 IsPressed()로 직접 상태를 가져옵니다.
        IsBoosting = boostAction.IsPressed();

        // 나머지 코드는 동일합니다.
        RotateInput = rotateAction.ReadValue<float>();
        ToggleControlPressed = toggleControlAction.WasPressedThisFrame();
    }
}