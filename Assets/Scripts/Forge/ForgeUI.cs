using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ForgeUI : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject mainBranchPrefab;
    [SerializeField] private GameObject subBranchPrefab;
    [SerializeField] private GameObject forgeNodePrefab; // 버튼이 포함된 노드 프리팹

    [Header("UI Container")]
    [SerializeField] private Transform mainBranchContainer; // 메인 브랜치들이 생성될 부모

    [Header("Layout Settings")]
    [SerializeField] private float baseMainBranchHeight = 100f;
    [SerializeField] private float heightPerSubBranch = 100f;
    [SerializeField] private float spacing = 30f;

    [Header("References")]
    [SerializeField] private ForgeManger forgeManger;

    // 생성된 UI 요소들을 추적
    private Dictionary<MainBranchType, GameObject> mainBranchUIObjects = new Dictionary<MainBranchType, GameObject>();
    // private Dictionary<SubBranchType, GameObject> subBranchUIObjects = new Dictionary<SubBranchType, GameObject>();
    private Dictionary<BaseForgeSO, GameObject> forgeNodeUIObjects = new Dictionary<BaseForgeSO, GameObject>();

    public void GenerateForgeUI()
    {
        ClearExistingUI();

        if (forgeManger == null || forgeManger.mainBranches == null)
        {
            Debug.LogError("ForgeManger or mainBranches is null!");
            return;
        }

        // 각 메인 브랜치에 대해 UI 생성
        foreach (var mainBranch in forgeManger.mainBranches)
        {
            CreateMainBranchUI(mainBranch);
        }
    }

    private void CreateMainBranchUI(MainBranchSO mainBranchSO)
    {
        if (mainBranchPrefab == null || mainBranchContainer == null)
        {
            Debug.LogError("MainBranchPrefab or MainBranchContainer is null!");
            return;
        }

        // 메인 브랜치 UI 생성
        GameObject mainBranchUI = Instantiate(mainBranchPrefab, mainBranchContainer);
        mainBranchUIObjects[mainBranchSO.branchType] = mainBranchUI;

        // 메인 브랜치 이름 설정
        mainBranchUI.name = $"MainBranch_{mainBranchSO.branchType}";

        // 서브 브랜치 개수에 따라 높이 조정
        int subBranchCount = mainBranchSO.subBranches != null ? mainBranchSO.subBranches.Length : 0;
        float calculatedHeight = baseMainBranchHeight + (heightPerSubBranch * subBranchCount) - (spacing * Mathf.Max(0, subBranchCount - 1));

        // RectTransform 높이 설정
        RectTransform mainBranchRect = mainBranchUI.GetComponent<RectTransform>();
        if (mainBranchRect != null)
        {
            mainBranchRect.sizeDelta = new Vector2(mainBranchRect.sizeDelta.x, calculatedHeight);
        }

        // 메인 브랜치 UI 컴포넌트 가져오기 (있다면)
        var mainBranchUIComponent = mainBranchUI.GetComponent<MainBranchUI>();
        if (mainBranchUIComponent != null)
        {
            mainBranchUIComponent.Initialize(mainBranchSO);
        }

        // 서브 브랜치 컨테이너 찾기 (프리팹 내부에 "SubBranchContainer"라는 이름의 Transform이 있다고 가정)
        Transform subBranchContainer = mainBranchUI.transform.Find("SubBranchContainer");
        if (subBranchContainer == null)
        {
            // 없으면 메인 브랜치 자체를 컨테이너로 사용
            subBranchContainer = mainBranchUI.transform;
        }

        // 서브 브랜치 생성
        if (mainBranchSO.subBranches != null)
        {
            foreach (var subBranch in mainBranchSO.subBranches)
            {
                CreateSubBranchUI(subBranch, subBranchContainer);
            }
        }
    }

    private void CreateSubBranchUI(SubBranchSO subBranchSO, Transform parent)
    {
        if (subBranchPrefab == null)
        {
            Debug.LogError("SubBranchPrefab is null!");
            return;
        }

        // 서브 브랜치 UI 생성
        GameObject subBranchUI = Instantiate(subBranchPrefab, parent);
        // subBranchUIObjects[subBranchSO.subBranchType] = subBranchUI;

        // 서브 브랜치 이름 설정
        subBranchUI.name = $"SubBranch_{subBranchSO.subBranchType}";

        // 노드 컨테이너 찾기 (프리팹 내부에 "NodeContainer"라는 이름의 Transform이 있다고 가정)
        Transform nodeContainer = subBranchUI.transform.Find("NodeContainer");
        if (nodeContainer == null)
        {
            // 없으면 서브 브랜치 자체를 컨테이너로 사용
            nodeContainer = subBranchUI.transform;
        }

        // 노드(BaseForgeSO) 생성
        if (subBranchSO.baseForgeSOs != null)
        {
            foreach (var forgeSO in subBranchSO.baseForgeSOs)
            {
                CreateForgeNodeUI(forgeSO, nodeContainer);
            }
        }
    }

    private void CreateForgeNodeUI(BaseForgeSO forgeSO, Transform parent)
    {
        if (forgeNodePrefab == null)
        {
            Debug.LogError("ForgeNodePrefab is null!");
            return;
        }

        // 노드 UI 생성
        GameObject nodeUI = Instantiate(forgeNodePrefab, parent);
        forgeNodeUIObjects[forgeSO] = nodeUI;

        // 노드 이름 설정
        nodeUI.name = $"Node_{forgeSO.forgeId}_{forgeSO.upgradeName}";

        // 노드 UI 컴포넌트 가져오기
        var nodeUIComponent = nodeUI.GetComponent<ForgeNodeUI>();
        if (nodeUIComponent != null)
        {
            nodeUIComponent.Initialize(forgeSO, OnForgeNodeClicked);
        }
        else
        {
            // ForgeNodeUI 컴포넌트가 없으면 버튼에 직접 리스너 추가
            Button button = nodeUI.GetComponent<Button>();
            if (button == null)
            {
                button = nodeUI.GetComponentInChildren<Button>();
            }

            if (button != null)
            {
                button.onClick.AddListener(() => OnForgeNodeClicked(forgeSO));
            }
            else
            {
                Debug.LogWarning($"No Button found on ForgeNodePrefab for {forgeSO.upgradeName}");
            }
        }
    }

    // 노드 버튼이 클릭되었을 때 호출되는 콜백
    private void OnForgeNodeClicked(BaseForgeSO forgeSO)
    {
        Debug.Log($"Forge Node Clicked: {forgeSO.upgradeName} (ForgeId: {forgeSO.forgeId})");
        
        // 여기서 강화 적용 로직을 처리할 수 있습니다
        // 예: 비용 확인, 강화 적용, UI 업데이트 등
        
        // 임시로 정보만 출력
        Debug.Log($"Coal: {forgeSO.coalCost}, Iron: {forgeSO.ironCost}, Gold: {forgeSO.goldCost}, Diamond: {forgeSO.diamondCost}");
        Debug.Log($"Description: {forgeSO.upgradeDescription}");
    }

    private void ClearExistingUI()
    {
        // 기존 UI 요소들 제거
        foreach (var ui in mainBranchUIObjects.Values)
        {
            if (ui != null) Destroy(ui);
        }
        mainBranchUIObjects.Clear();
        // subBranchUIObjects.Clear();
        forgeNodeUIObjects.Clear();
    }

    // 에디터에서 테스트용
    [ContextMenu("Generate Forge UI")]
    private void TestGenerateUI()
    {
        GenerateForgeUI();
    }

    void Start()
    {
        // 자동으로 UI 생성 (원한다면)
        GenerateForgeUI();
    }
}
