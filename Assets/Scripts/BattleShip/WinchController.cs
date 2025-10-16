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

    // --- 내부 변수 ---
    private LineRenderer lineRenderer;
    private GameObject currentForcepInstance;
    private DistanceJoint2D distanceJoint;
    private ForcepController forcepController;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false; // 처음에는 로프를 숨깁니다.
    }

    void Update()
    {
        HandleInput();
        UpdateRopeVisuals();
    }

    private void HandleInput()
    {
        // W 키를 처음 눌렀고, Forcep이 없다면 새로 생성
        if (Input.GetKeyDown(KeyCode.W) && currentForcepInstance == null)
        {
            SpawnForcep();
        }

        // Forcep이 존재할 때만 로프 길이 조절
        if (currentForcepInstance != null)
        {
            // W를 누르고 있으면 로프가 길어짐
            if (Input.GetKey(KeyCode.W))
            {
                distanceJoint.distance += ropeSpeed * Time.deltaTime;
            }

            // S를 누르고 있으면 로프가 짧아짐
            if (Input.GetKey(KeyCode.S))
            {
                distanceJoint.distance -= ropeSpeed * Time.deltaTime;
            }

            // 로프 길이 제한
            distanceJoint.distance = Mathf.Clamp(distanceJoint.distance, minRopeLength, maxRopeLength);

            // 로프 길이가 최소값에 도달하면 Forcep과 수집된 광물 처리
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
        
        // 생성된 Forcep에서 필요한 컴포넌트들을 가져와 저장합니다.
        distanceJoint = currentForcepInstance.GetComponent<DistanceJoint2D>();
        forcepController = currentForcepInstance.GetComponent<ForcepController>();

        // Distance Joint의 연결 대상을 이 Winch 오브젝트로 설정합니다.
        distanceJoint.connectedBody = GetComponent<Rigidbody2D>();
        distanceJoint.distance = minRopeLength; // 초기 길이는 최소값으로 설정

        lineRenderer.enabled = true; // 로프를 보이게 합니다.
    }

    private void UpdateRopeVisuals()
    {
        // Forcep이 있을 때만 로프를 그립니다.
        if (currentForcepInstance != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, currentForcepInstance.transform.position);
        }
    }

    // Forcep이 파괴된 후 Winch를 초기 상태로 되돌립니다.
    private void ResetWinch()
    {
        currentForcepInstance = null;
        distanceJoint = null;
        forcepController = null;
        lineRenderer.enabled = false;
    }
}
