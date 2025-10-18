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

    [SerializeField] private int skipChecksFramesAfterWarp = 4;
    private int skipChecksUntilFrame = -1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        WorldWarper.OnWarped += HandleWorldWarped;   // 구독
    }

    void OnDisable()
    {
        WorldWarper.OnWarped -= HandleWorldWarped;   // 해제 (중복구독 방지)
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) CollectNearestOre();
        if (Input.GetKeyDown(KeyCode.Q)) DropLastCollectedOre();
    }
    void LateUpdate()
    {
        // 워프는 WorldWarper.LateUpdate()에서 발생
        // 그 프레임에 HandleWorldWarped() → SyncTransforms()까지 끝난 뒤
        // 여기서 거리 체크/라인 업데이트를 수행하면 순서가 안전하다.
        UpdateAndCheckConnections_Late();
    }

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

        Vector2 hookPos = cargoHook.position;
        Vector2 orePos = nearestOre.transform.position;
        float totalDistance = Vector2.Distance(hookPos, orePos);
        Vector2 direction = (orePos - hookPos).normalized;
        float segmentLength = totalDistance / (numberOfSegments + 1);

        // 우주선 로컬에서 본 카고 훅 좌표(자식이 아니어도 안전)
        Vector2 shipLocalHook = rb.transform.InverseTransformPoint(cargoHook.position);

        for (int i = 0; i < numberOfSegments; i++)
        {
            // ⬇︎ 첫 세그먼트를 훅 위치에 스폰
            Vector2 spawnPos = hookPos + direction * segmentLength * i;

            GameObject segmentObj = Instantiate(ropeSegmentPrefab, spawnPos, Quaternion.identity);
            ropeSegments.Add(segmentObj);

            // 세그먼트가 꼭 RB2D/Hinge를 갖도록 보강
            var segRB = segmentObj.GetComponent<Rigidbody2D>();
            if (segRB == null) segRB = segmentObj.AddComponent<Rigidbody2D>();

            HingeJoint2D joint = segmentObj.GetComponent<HingeJoint2D>();
            if (joint == null) joint = segmentObj.AddComponent<HingeJoint2D>();

            joint.connectedBody = previousSegmentRB;

            if (i == 0)
            {
                // ⬇︎ 이것 때문에 "첫 마디가 뻣뻣한 고정처럼" 보였을 가능성이 큼
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = shipLocalHook; // 우주선 RB 로컬 기준의 훅 위치
                // 필요시: joint.enableCollision = true; // 로프끼리 충돌 원하면
            }
            else
            {
                joint.autoConfigureConnectedAnchor = true;
            }

            previousSegmentRB = segRB;
        }

        // 광물에 힌지 연결 (필요시 RB 보강)
        var oreRB = nearestOre.GetComponent<Rigidbody2D>();
        if (oreRB == null) oreRB = nearestOre.AddComponent<Rigidbody2D>();

        HingeJoint2D oreJoint = nearestOre.GetComponent<HingeJoint2D>();
        if (oreJoint == null) oreJoint = nearestOre.AddComponent<HingeJoint2D>();
        oreJoint.connectedBody = previousSegmentRB;
        oreJoint.autoConfigureConnectedAnchor = true;

        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();
        collectedOres.Add(new CollectedOreInfo(nearestOre, line, ropeSegments));
    }
    private void DropLastCollectedOre()
    {
        if (collectedOres.Count == 0) return;
        BreakConnection(collectedOres.Last());
    }


    private void UpdateAndCheckConnections_Late()
    {
        for (int i = collectedOres.Count - 1; i >= 0; i--)
        {
            CollectedOreInfo oreInfo = collectedOres[i];

            if (oreInfo.OreObject == null)
            {
                BreakConnection(oreInfo);
                continue;
            }

            // 워프 직후 몇 프레임 스킵 (이미 HandleWorldWarped에서 설정)
            if (Time.frameCount <= skipChecksUntilFrame)
            {
                UpdateLine(oreInfo);
                continue;
            }

            float distance = Vector2.Distance(cargoHook.position, oreInfo.OreObject.transform.position);
            if (distance > maxRopeLength)
            {
                Debug.Log("거리가 너무 멀어져 밧줄이 끊어집니다!");
                BreakConnection(oreInfo);
                continue;
            }

            UpdateLine(oreInfo);
        }
    }


    private void UpdateLine(CollectedOreInfo oreInfo)
    {
        var line = oreInfo.Line;
        var segments = oreInfo.RopeSegments;
        if (line == null) return;

        var alive = segments.Where(s => s != null).ToList();

        line.positionCount = alive.Count + 1; // 세그먼트들 + 마지막 광물
        for (int j = 0; j < alive.Count; j++)
            line.SetPosition(j, alive[j].transform.position);

        line.SetPosition(alive.Count, oreInfo.OreObject.transform.position);
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
    // 다른 스크립트에서 현재 수집한 광물 개수를 물어볼 수 있도록 통로를 열어줍니다.
    public int GetCollectedOreCount()
    {
        return collectedOres.Count;
    }

    private void HandleWorldWarped(Vector3 delta)
    {
        // 세그먼트/광물 모두 같은 델타로 이동
        foreach (var oreInfo in collectedOres)
        {
            foreach (var seg in oreInfo.RopeSegments)
            {
                if (seg == null) continue;
                var srb = seg.GetComponent<Rigidbody2D>();
                if (srb) srb.position += (Vector2)delta;
                else seg.transform.position += delta;
            }

            if (oreInfo.OreObject != null)
            {
                var orb = oreInfo.OreObject.GetComponent<Rigidbody2D>();
                if (orb) orb.position += (Vector2)delta;
                else oreInfo.OreObject.transform.position += delta;
            }
        }

        // 물리 동기화 (힌지/충돌 안정화)
        Physics2D.SyncTransforms();

        // 이 프레임 포함 N프레임 길이 체크 스킵
        skipChecksUntilFrame = Time.frameCount + skipChecksFramesAfterWarp;
    }


}