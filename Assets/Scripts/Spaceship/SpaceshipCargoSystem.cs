// SpaceshipCargoSystem.cs
using System.Collections.Generic;
using System.Linq; // LINQ를 사용하기 위해 추가
using UnityEngine;

// 수집된 광물 정보를 관리하기 위한 작은 클래스
public class CollectedOreInfo
{
    public GameObject OreObject { get; }
    public SpringJoint2D Joint { get; }
    public LineRenderer Line { get; }

    public CollectedOreInfo(GameObject oreObject, SpringJoint2D joint, LineRenderer line)
    {
        OreObject = oreObject;
        Joint = joint;
        Line = line;
    }
}

public class SpaceshipCargoSystem : MonoBehaviour
{
    [Header("수집 설정")]
    [Tooltip("광물이 연결될 우주선의 지점 (빈 오브젝트를 자식으로 만들어 연결)")]
    [SerializeField] private Transform cargoHook;

    [Tooltip("수집 가능한 최대 광물 수")]
    [SerializeField] private int maxCargoCount = 5;

    [Tooltip("광물을 감지하는 트리거 콜라이더")]
    [SerializeField] private CircleCollider2D collectionTrigger;

    [Tooltip("연결선이 끊어지는 최대 거리")]
    [SerializeField] private float maxRopeLength = 20f;

    [Header("연결선(Line) 프리팹")]
    [SerializeField] private GameObject linePrefab;

    // 수집 가능한 범위 안에 들어온 광물 목록
    private List<GameObject> potentialOres = new List<GameObject>();

    // 현재 수집해서 끌고 다니는 광물 목록
    private List<CollectedOreInfo> collectedOres = new List<CollectedOreInfo>();

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (collectionTrigger == null)
        {
            // 트리거가 할당되지 않았다면, 이 오브젝트에서 직접 찾으려고 시도
            collectionTrigger = GetComponent<CircleCollider2D>();
            if (collectionTrigger != null && !collectionTrigger.isTrigger)
            {
                Debug.LogWarning("SpaceshipCargoSystem: 할당된 콜라이더가 Trigger가 아닙니다! Is Trigger를 체크해주세요.");
            }
        }
    }

    void Update()
    {
        // E 키를 눌러 가장 가까운 광물 수집
        if (Input.GetKeyDown(KeyCode.E))
        {
            CollectNearestOre();
        }

        // Q 키를 눌러 가장 최근에 수집한 광물 버리기
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropLastCollectedOre();
        }

        // 매 프레임마다 연결선 상태 업데이트 및 거리 체크
        UpdateOreConnections();
    }

    private void CollectNearestOre()
    {
        // 1. 화물칸이 꽉 찼거나, 수집할 광물이 없으면 아무것도 하지 않음
        if (collectedOres.Count >= maxCargoCount)
        {
            Debug.Log("화물칸이 가득 찼습니다!");
            return;
        }

        // 2. null인 (파괴된) 오브젝트를 리스트에서 제거
        potentialOres.RemoveAll(item => item == null);
        if (potentialOres.Count == 0) return;

        // 3. 가장 가까운 광물 찾기
        GameObject nearestOre = potentialOres
            .OrderBy(ore => Vector2.Distance(transform.position, ore.transform.position))
            .FirstOrDefault();

        if (nearestOre == null) return;
        
        // 4. 해당 광물 수집 처리
        potentialOres.Remove(nearestOre);
        
        // SpringJoint2D를 광물에 추가하고 설정
        SpringJoint2D joint = nearestOre.AddComponent<SpringJoint2D>();
        joint.connectedBody = this.rb; // 우주선 리지드바디에 연결
        joint.autoConfigureDistance = false;
        joint.distance = Vector2.Distance(cargoHook.position, nearestOre.transform.position);
        joint.dampingRatio = 0.5f; // 자연스러운 움직임을 위한 댐핑
        joint.frequency = 2f;      // 스프링의 탄성

        // 라인 렌더러 생성 및 설정
        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();
        line.positionCount = 2; // 선은 2개의 점(시작, 끝)으로 구성
        
        // 수집된 광물 정보를 리스트에 추가
        collectedOres.Add(new CollectedOreInfo(nearestOre, joint, line));
    }

    private void DropLastCollectedOre()
    {
        if (collectedOres.Count == 0) return;
        
        // 가장 마지막에 추가된 (가장 최근) 광물 정보를 가져옴
        CollectedOreInfo lastOreInfo = collectedOres[collectedOres.Count - 1];
        
        // 연결 끊기
        BreakConnection(lastOreInfo);
    }

    private void UpdateOreConnections()
    {
        // 역순으로 순회해야 리스트에서 아이템을 제거할 때 에러가 발생하지 않음
        for (int i = collectedOres.Count - 1; i >= 0; i--)
        {
            CollectedOreInfo oreInfo = collectedOres[i];
            
            // 광물이 파괴되었다면 리스트에서 제거
            if (oreInfo.OreObject == null)
            {
                Destroy(oreInfo.Line.gameObject);
                collectedOres.RemoveAt(i);
                continue;
            }

            // 거리가 너무 멀어지면 연결 끊기
            float distance = Vector2.Distance(cargoHook.position, oreInfo.OreObject.transform.position);
            if (distance > maxRopeLength)
            {
                BreakConnection(oreInfo);
                continue; // 다음 아이템으로 넘어감
            }

            // 라인 렌더러의 위치 업데이트
            oreInfo.Line.SetPosition(0, cargoHook.position);
            oreInfo.Line.SetPosition(1, oreInfo.OreObject.transform.position);
        }
    }
    
    // 특정 광물의 연결을 끊는 공통 로직
    private void BreakConnection(CollectedOreInfo oreInfo)
    {
        if (oreInfo == null) return;

        // 광물이 아직 존재한다면, 조인트 컴포넌트만 파괴
        if (oreInfo.OreObject != null)
        {
            Destroy(oreInfo.Joint);
        }
        
        // 라인 렌더러 오브젝트 파괴
        Destroy(oreInfo.Line.gameObject);
        
        // 관리 리스트에서 제거
        collectedOres.Remove(oreInfo);
    }


    // 트리거 범위 안에 Ore 레이어 오브젝트가 들어왔을 때
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Ore" 레이어를 가진 오브젝트만 리스트에 추가
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            if (!potentialOres.Contains(other.gameObject))
            {
                potentialOres.Add(other.gameObject);
            }
        }
    }

    // 트리거 범위에서 나갔을 때
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            potentialOres.Remove(other.gameObject);
        }
    }
}