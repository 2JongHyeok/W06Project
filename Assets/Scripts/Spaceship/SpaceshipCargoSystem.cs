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

        // --- 바로 이 부분이 사건의 해결책이오, 왓슨! ---
        List<GameObject> ropeSegments = new List<GameObject>();

        // 1. 연결 대상은 '우주선의 심장', 즉 this.rb가 되어야만 하오.
        Rigidbody2D previousSegmentRB = this.rb;

        Vector2 hookPos = cargoHook.position;
        Vector2 orePos = nearestOre.transform.position;
        float totalDistance = Vector2.Distance(hookPos, orePos);
        Vector2 direction = (orePos - hookPos).normalized;
        float segmentLength = totalDistance / (numberOfSegments + 1);

        for (int i = 0; i < numberOfSegments; i++)
        {
            Vector2 spawnPos = hookPos + direction * segmentLength * (i + 1);
            GameObject segmentObj = Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity);
            ropeSegments.Add(segmentObj);
            HingeJoint2D joint = segmentObj.GetComponent<HingeJoint2D>();
            
            // 2. 모든 마디는 우주선의 '심장'에 연결됩니다.
            joint.connectedBody = previousSegmentRB;

            // 3. 하지만, 가장 첫 번째 마디(i=0)만은 예외적으로,
            //    그 연결점을 '심장'의 중심이 아닌 '카고 훅의 로컬 좌표'로 지정합니다!
            if (i == 0)
            {
                // 이것이 바로 '꼬리뼈'를 조준하는 한 발의 총알이오, 왓슨.
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
                if (segments[j] != null)
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

        foreach (var segment in oreInfo.RopeSegments)
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
// --- 이 함수를 스크립트 맨 아래 (닫히는 괄호 '}' 바로 앞)에 추가해 ---
    public void UnloadAllOres(InventoryManger inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("인벤토리가 없는데 어디다 납품하라는 거야!");
            return;
        }

        // 가지고 있는 모든 광물 정보를 순회한다.
        foreach (var oreInfo in collectedOres)
        {
            // 광물 오브젝트가 존재하는지 다시 한번 확인하는 건 기본이지.
            if (oreInfo.OreObject != null)
            {
                Ore oreComponent = oreInfo.OreObject.GetComponent<Ore>();
                if (oreComponent != null)
                {
                    // 1. 인벤토리에 광물을 추가.
                    inventory.AddOre(oreComponent.oreType, oreComponent.amount);
                }
                
                // 2. 이제 쓸모없어진 광물 게임 오브젝트를 파괴.
                Destroy(oreInfo.OreObject);
            }

            // 3. 광물에 연결됐던 모든 밧줄 마디들도 파괴.
            foreach (var segment in oreInfo.RopeSegments)
            {
                Destroy(segment);
            }

            // 4. 눈에 보이던 선(Line Renderer)도 파괴.
            if (oreInfo.Line != null)
            {
                Destroy(oreInfo.Line.gameObject);
            }
        }

        // 5. 모든 짐을 내렸으니, 수집 목록을 깨끗하게 비운다.
        collectedOres.Clear();
    }

}