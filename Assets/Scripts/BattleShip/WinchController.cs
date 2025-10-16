using UnityEngine;

public class WinchController : MonoBehaviour
{
    [Header("오브젝트 연결")]
    [Tooltip("생성할 Forcep 프리팹을 연결하세요.")]
    public GameObject forcepPrefab;

    [Header("로프 설정")]
    [Tooltip("로프 조절 속도")]
    public float ropeSpeed = 5f;
    [Tooltip("로프 최대 길이")]
    public float maxRopeLength = 20f;
    [Tooltip("로프 최소 길이")]
    public float minRopeLength = 1.5f;
    [SerializeField]
    [Tooltip("씬에 있는 InventoryManger 오브젝트를 여기에 연결하세요.")]
    private InventoryManger inventoryManger;

    // --- 내부 변수 ---
    private LineRenderer lineRenderer;
    private GameObject currentForcepInstance;
    private DistanceJoint2D distanceJoint;
    private ForcepController forcepController;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        HandleInput();
        UpdateRopeVisuals();
    }

    private void HandleInput()
    {
        // [수정] S 키를 처음 눌렀고, Forcep이 없다면 새로 생성
        if (Input.GetKeyDown(KeyCode.S) && currentForcepInstance == null)
        {
            SpawnForcep();
        }

        if (currentForcepInstance != null)
        {
            // [수정] S키: Forcep이 땅에 닿아있지 않을 때만 로프가 길어짐
            if (Input.GetKey(KeyCode.S) && !forcepController.isGrounded)
            {
                distanceJoint.distance += ropeSpeed * Time.deltaTime;
            }

            // [수정] W키: 로프가 짧아짐
            if (Input.GetKey(KeyCode.W))
            {
                distanceJoint.distance -= ropeSpeed * Time.deltaTime;
            }

            distanceJoint.distance = Mathf.Clamp(distanceJoint.distance, minRopeLength, maxRopeLength);

            if (distanceJoint.distance <= minRopeLength)
            {
                forcepController.ProcessAndDestroy();
                ResetWinch();
            }
        }
    }

    private void SpawnForcep()
    {
        currentForcepInstance = Instantiate(forcepPrefab, transform.position, Quaternion.identity);
        
        distanceJoint = currentForcepInstance.GetComponent<DistanceJoint2D>();
        forcepController = currentForcepInstance.GetComponent<ForcepController>();

        forcepController.inventoryManger = this.inventoryManger;

        distanceJoint.connectedBody = GetComponent<Rigidbody2D>();
        distanceJoint.distance = minRopeLength;

        lineRenderer.enabled = true;

        // [추가] 무기 시스템 비활성화 (Weapon 스크립트가 Singleton일 경우)
        if (Weapon.Instance != null)
        {
            Weapon.Instance.enabled = false;
            Debug.Log("Weapon 비활성화됨");
        }
    }

    private void UpdateRopeVisuals()
    {
        if (currentForcepInstance != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentForcepInstance.transform.position);
        }
    }
    
    private void ResetWinch()
    {
        currentForcepInstance = null;
        distanceJoint = null;
        forcepController = null;
        lineRenderer.enabled = false;

        // [추가] 무기 시스템 다시 활성화
        if (Weapon.Instance != null)
        {
            Weapon.Instance.enabled = true;
            Debug.Log("Weapon 다시 활성화됨");
        }
    }
}