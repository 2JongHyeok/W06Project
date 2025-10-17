using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class DockingStation : MonoBehaviour
{
    [Tooltip("씬에 있는 인벤토리 매니저를 여기다 끌어다 놔.")]
    [SerializeField] private InventoryManger inventoryManger;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    // 현재 도킹 범위 안에 '들어온' 실제 우주선 인스턴스 (씬 오브젝트)
    [SerializeField] private GameObject dockedShip;
    private SpaceshipCargoSystem cargoSystem;
    // 현재 카메라/조작이 우주선 모드인지 여부
    [SerializeField] public bool isSpaceshipMode = false;
    // '처음엔' 범위 밖이 정상
    private bool isSpaceshipInRange = true;
    private void Reset()
    {
        // 트리거 보장
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }
    private void Start()
    {
        if (dockedShip != null)
            dockedShip.SetActive(false);
        if (!cameraSwitcher)
            Debug.LogWarning("[DockingStation] CameraSwitcher가 비었습니다.");
        if (!inventoryManger)
            Debug.LogWarning("[DockingStation] InventoryManger가 비었습니다.");
    }
    private void Update()
    {
        // 1) 우주선 모드 ON (도킹 범위 안에서만)
        if ( Input.GetKeyDown(KeyCode.E) && !isSpaceshipMode)
        {
            if (!dockedShip)
            {
                Debug.LogWarning("[DockingStation] dockedShip이 없습니다. OnTriggerEnter2D에서 못 잡은 것 같습니다.");
                return;
            }
            cameraSwitcher?.ToggleCameraMode();
            dockedShip.SetActive(true);    // 우주선 조작 시작
            isSpaceshipMode = true;
            Debug.Log("우주선 모드 진입");
            return;
        }
        // 2) 우주선 모드 OFF + 하역 (도킹 범위 안에서만)
        if (isSpaceshipInRange && Input.GetKeyDown(KeyCode.E) && isSpaceshipMode)
        {
            cameraSwitcher?.ToggleCameraMode();
            isSpaceshipMode = false;
            // 하역
            if (cargoSystem != null && inventoryManger != null)
            {
                cargoSystem.UnloadAllOres(inventoryManger);
            }
            else
            {
                Debug.LogWarning("[DockingStation] cargoSystem 또는 inventoryManger가 null이라 하역을 건너뜁니다.");
            }
            // 우주선 비활성
            if (dockedShip)
                dockedShip.SetActive(false);
            // 참조 해제
            cargoSystem = null;
            Debug.Log("모든 광물을 기지에 저장하고 우주선을 격납했습니다.");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Spaceship")) return;
        isSpaceshipInRange = true;
        dockedShip = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        cargoSystem = other.GetComponentInParent<SpaceshipCargoSystem>();
        if (!cargoSystem)
            Debug.LogWarning("[DockingStation] 우주선은 들어왔는데 SpaceshipCargoSystem이 없습니다.");
        Debug.Log("우주선 도킹 가능");
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Spaceship")) return;
        isSpaceshipInRange = false;
        Debug.Log("우주선 도킹 불가");
    }
}