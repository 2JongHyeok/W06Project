using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DockingStation : MonoBehaviour
{
    [Header("필수 연결 요소")]
    [Tooltip("씬에 있는 인벤토리 매니저를 여기다 끌어다 놔.")]
    [SerializeField] private InventoryManger inventoryManger;
    [SerializeField] private CameraSwitcher cameraSwitcher;

    [Header("우주선 정보")]
    [Tooltip("씬에 배치된 실제 우주선 인스턴스 (씬 오브젝트)")]
    [SerializeField] private GameObject dockedShip;
    
    [Header("출격 설정")]
    [Tooltip("우주선이 출격할 원의 반지름입니다.")]
    [SerializeField] private float departureCircleRadius = 5f;

    // --- 내부 변수 ---
    private SpaceshipCargoSystem cargoSystem;
    [SerializeField] public bool isSpaceshipMode = false;
    private bool isSpaceshipInRange = false;

    private Vector3 nextDeparturePosition;
    private Quaternion nextDepartureRotation;

    private void Reset()
    {
        // 트리거 보장
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void Start()
    {
        if (dockedShip != null)
        {
            // 비활성화 전에, '초기 위치'를 기준으로 첫 출격 지점을 미리 계산
            CalculateNextDeparturePoint(dockedShip.transform.position);
            dockedShip.SetActive(false);
        }
        if (!cameraSwitcher)
            Debug.LogWarning("[DockingStation] CameraSwitcher가 비었습니다.");
        if (!inventoryManger)
            Debug.LogWarning("[DockingStation] InventoryManger가 비었습니다.");
    }

    private void Update()
    {
        // 우주선 '출격' 로직만 여기에 남겨둡니다. (F키를 누르고, 우주선 모드가 아닐 때)
        if (Input.GetKeyDown(KeyCode.F) && !isSpaceshipMode)
        {
            if (!dockedShip)
            {
                Debug.LogWarning("[DockingStation] dockedShip이 없습니다.");
                return;
            }

            // 활성화 전에 위치와 회전을 먼저 적용
            dockedShip.transform.SetPositionAndRotation(nextDeparturePosition, nextDepartureRotation);
            
            cameraSwitcher?.ToggleCameraMode();
            dockedShip.SetActive(true);      // 우주선 조작 시작
            isSpaceshipMode = true;
            Debug.Log("우주선 모드 진입. 출격 위치로 이동.");
        }
    }
    
    // 출격 위치와 방향을 계산하는 함수 (변경 없음)
    private void CalculateNextDeparturePoint(Vector3 basisPosition)
    {
        // 도킹 스테이션 위치(원의 중심)에서 우주선이 있던 방향으로 벡터 계산
        Vector3 direction = (basisPosition - transform.position).normalized;
        
        // 방향이 0이면 (위치가 겹치면) 기본값으로 위쪽을 보도록 설정
        if (direction == Vector3.zero) direction = Vector3.up;

        // 원의 테두리상 위치 계산
        nextDeparturePosition = transform.position + direction * departureCircleRadius;
        
        // 우주선의 위쪽(up)이 바깥(direction)을 향하도록 회전값 계산
        nextDepartureRotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Spaceship")) return;

        // [변경] 현재 우주선 모드일 때 (플레이어가 조종 중일 때) 자동 도킹을 실행합니다.
        if (isSpaceshipMode)
        {
            isSpaceshipInRange = true; // 도킹이 발생했으므로 InRange 상태로 간주
            Debug.Log("우주선이 범위에 진입하여 자동 도킹을 시작합니다.");

            // --- 기존 Update()에 있던 도킹 로직을 여기로 이동 ---
            cameraSwitcher?.ToggleCameraMode();
            isSpaceshipMode = false;

            // 도킹할 우주선과 카고 시스템 참조
            dockedShip = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
            cargoSystem = other.GetComponentInParent<SpaceshipCargoSystem>();

            // 하역
            if (cargoSystem != null && inventoryManger != null)
            {
                //cargoSystem.UnloadAllOres(inventoryManger);
            }
            else
            {
                Debug.LogWarning("[DockingStation] cargoSystem 또는 inventoryManger가 null이라 하역을 건너뜁니다.");
            }

            // 비활성화 전에, '현재 위치'를 기준으로 다음 출격 지점을 계산
            CalculateNextDeparturePoint(dockedShip.transform.position);

            // 우주선 비활성
            if (dockedShip)
                dockedShip.SetActive(false);

            // 참조 해제
            cargoSystem = null;
            //Debug.Log("모든 광물을 기지에 저장하고 우주선을 격납했습니다.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Spaceship")) return;
        isSpaceshipInRange = false;
        Debug.Log("우주선 도킹 불가");
    }
}