// SpaceshipCargoSystem.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// (CollectedOreInfo 클래스는 변경 없음)
public class CollectedOreInfo
{
    public GameObject OreObject { get; }
    public LineRenderer Line { get; }
    public List<GameObject> RopeSegments { get; }

    public CollectedOreInfo(GameObject oreObject, LineRenderer line, List<GameObject> ropeSegments)
    {
        OreObject = oreObject;
        Line = line;
        RopeSegments = ropeSegments;
    }
}

public class SpaceshipCargoSystem : MonoBehaviour
{
    [Header("수집 설정")]
    [SerializeField] private Transform cargoHook;
    [SerializeField] private int maxCargoCount = 5;
    [SerializeField] private CircleCollider2D collectionTrigger;

    [Header("밧줄(Rope) 설정")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject ropeSegmentPrefab;
    [SerializeField] private int numberOfSegments = 10;
    
    // --- 이 부분이 다시 추가되었습니다! ---
    [Tooltip("밧줄이 끊어지는 최대 직선 거리")]
    [SerializeField] private float maxRopeLength = 25f;

    private List<GameObject> potentialOres = new List<GameObject>();
    private List<CollectedOreInfo> collectedOres = new List<CollectedOreInfo>();
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) CollectNearestOre();
        if (Input.GetKeyDown(KeyCode.Q)) DropLastCollectedOre();
        
        // --- 함수 이름 변경 및 로직 수정 ---
        UpdateAndCheckConnections(); 
    }

    // (CollectNearestOre 함수는 변경 없음)
    private void CollectNearestOre()
    {
        if (collectedOres.Count >= maxCargoCount) return;
        potentialOres.RemoveAll(item => item == null);
        if (potentialOres.Count == 0) return;

        GameObject nearestOre = potentialOres
            .OrderBy(ore => Vector2.Distance(transform.position, ore.transform.position))
            .FirstOrDefault();

        if (nearestOre == null) return;
        
        potentialOres.Remove(nearestOre);
        
        List<GameObject> ropeSegments = new List<GameObject>();
        Rigidbody2D previousSegmentRB = this.rb;
        Vector2 spawnPos = cargoHook.position;

        for (int i = 0; i < numberOfSegments; i++)
        {
            GameObject segmentObj = Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity);
            ropeSegments.Add(segmentObj);

            HingeJoint2D joint = segmentObj.GetComponent<HingeJoint2D>();
            joint.connectedBody = previousSegmentRB;
            
            if (i == 0)
            {
                joint.connectedAnchor = cargoHook.localPosition;
            }
            
            previousSegmentRB = segmentObj.GetComponent<Rigidbody2D>();
        }

        HingeJoint2D oreJoint = nearestOre.AddComponent<HingeJoint2D>();
        oreJoint.connectedBody = previousSegmentRB;

        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();
        
        collectedOres.Add(new CollectedOreInfo(nearestOre, line, ropeSegments));
    }

    private void DropLastCollectedOre()
    {
        if (collectedOres.Count == 0) return;
        BreakConnection(collectedOres.Last());
    }
    
    // --- 이 함수가 크게 변경되었습니다! ---
    private void UpdateAndCheckConnections()
    {
        // 리스트에서 아이템을 제거할 때 에러가 나지 않도록 역순으로 순회합니다.
        for (int i = collectedOres.Count - 1; i >= 0; i--)
        {
            CollectedOreInfo oreInfo = collectedOres[i];
            
            // 광물이 중간에 파괴되었는지 먼저 확인
            if (oreInfo.OreObject == null)
            {
                BreakConnection(oreInfo);
                continue; // 다음 아이템으로 넘어감
            }

            // 1. 거리 체크 로직
            float distance = Vector2.Distance(cargoHook.position, oreInfo.OreObject.transform.position);
            if (distance > maxRopeLength)
            {
                Debug.Log("거리가 너무 멀어져 밧줄이 끊어집니다!");
                BreakConnection(oreInfo);
                continue; // 연결을 끊었으니 다음 아이템으로 넘어감
            }

            // 2. 라인 렌더러 위치 업데이트 (연결이 유효할 때만 실행)
            var line = oreInfo.Line;
            var segments = oreInfo.RopeSegments;
            
            line.positionCount = segments.Count + 2;
            line.SetPosition(0, cargoHook.position);
            for (int j = 0; j < segments.Count; j++)
            {
                // 마디가 파괴되었을 경우를 대비한 안전장치
                if(segments[j] != null)
                {
                    line.SetPosition(j + 1, segments[j].transform.position);
                }
            }
            line.SetPosition(segments.Count + 1, oreInfo.OreObject.transform.position);
        }
    }
    
    // (BreakConnection, OnTriggerEnter2D, OnTriggerExit2D 함수는 변경 없음)
    private void BreakConnection(CollectedOreInfo oreInfo)
    {
        if (oreInfo == null) return;
        
        if (oreInfo.OreObject != null)
        {
            Destroy(oreInfo.OreObject.GetComponent<HingeJoint2D>());
        }
        
        foreach(var segment in oreInfo.RopeSegments)
        {
            Destroy(segment);
        }
        
        if (oreInfo.Line != null)
        {
            Destroy(oreInfo.Line.gameObject);
        }

        collectedOres.Remove(oreInfo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            if (!potentialOres.Contains(other.gameObject)) potentialOres.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ore"))
        {
            potentialOres.Remove(other.gameObject);
        }
    }
}