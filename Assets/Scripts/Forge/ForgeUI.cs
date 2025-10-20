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
    // 서브 브랜치 설정
    [SerializeField] private float subBranchHeight = 100f;        // 서브 브랜치 높이
    [SerializeField] private float subBranchGapX = 130f;          // 서브 브랜치 간 X 간격 (depth용)
    [SerializeField] private float subBranchGapY = 30f;           // 서브 브랜치 간 Y 간격
    
    // 노드 설정
    [SerializeField] private float nodeWidth = 100f;              // 노드 하나의 너비
    [SerializeField] private float nodeHeight = 100f;             // 노드 하나의 높이
    
    // 메인 브랜치 설정
    [SerializeField] private float baseMainBranchHeight = 100f;   // 메인 브랜치 기본 높이

    [Header("References")]
    [SerializeField] private ForgeManager forgeManager;
    [SerializeField] private InventoryManger inventoryManger; // 인벤토리 매니저 추가
    [SerializeField] private ForgeTooltipUI tooltipUI; // Tooltip UI 프리팹 또는 씬의 Tooltip

    // 생성된 UI 요소들을 추적
    private Dictionary<MainBranchType, GameObject> mainBranchUIObjects = new Dictionary<MainBranchType, GameObject>();
    // private Dictionary<SubBranchType, GameObject> subBranchUIObjects = new Dictionary<SubBranchType, GameObject>();
    private Dictionary<BaseForgeSO, GameObject> forgeNodeUIObjects = new Dictionary<BaseForgeSO, GameObject>();

    public void GenerateForgeUI()
    {
        ClearExistingUI();

        if (forgeManager == null || forgeManager.mainBranches == null)
        {
            Debug.LogError("ForgeManger or mainBranches is null!");
            return;
        }

        // 각 메인 브랜치에 대해 UI 생성
        foreach (var mainBranch in forgeManager.mainBranches)
        {
            CreateMainBranchUI(mainBranch);
        }
    }
    
    public void ClearForgeUI()
    {
        ClearExistingUI();
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

        // 서브 브랜치 생성 (LockedSubBranch 포함하여 동적으로 생성)
        int totalSubBranchCount = 0;
        if (mainBranchSO.subBranches != null)
        {
            float currentYPosition = 0f;
            for (int i = 0; i < mainBranchSO.subBranches.Length; i++)
            {
                var result = CreateSubBranchUI(mainBranchSO.subBranches[i], subBranchContainer, currentYPosition, 0);
                currentYPosition = result.nextYPosition;
                totalSubBranchCount += result.createdCount;
            }
        }

        // 실제 생성된 서브 브랜치 개수로 높이 재계산
        // MainBranch 높이 = (서브브랜치개수 × 서브브랜치높이) + ((개수-1) × Y간격)
        float totalGapHeight = totalSubBranchCount > 0 ? (totalSubBranchCount - 1) * subBranchGapY : 0f;
        float calculatedHeight = (totalSubBranchCount * subBranchHeight) + totalGapHeight;
        
        RectTransform mainBranchRect = mainBranchUI.GetComponent<RectTransform>();
        if (mainBranchRect != null)
        {
            mainBranchRect.sizeDelta = new Vector2(mainBranchRect.sizeDelta.x, calculatedHeight);
        }
    }

    private (float nextYPosition, int createdCount) CreateSubBranchUI(SubBranchSO subBranchSO, Transform parent, float currentYPosition, int depth)
    {
        if (subBranchPrefab == null)
        {
            Debug.LogError("SubBranchPrefab is null!");
            return (currentYPosition, 0);
        }

        // 서브 브랜치 UI 생성
        GameObject subBranchUI = Instantiate(subBranchPrefab, parent);

        // 서브 브랜치 이름 설정
        string branchPrefix = depth > 0 ? "Locked" : "";
        subBranchUI.name = $"{branchPrefix}SubBranch_{subBranchSO.subBranchType}";

        // 노드 개수 세기
        int nodeCount = subBranchSO.baseForgeSOs != null ? subBranchSO.baseForgeSOs.Length : 0;
        
        // 서브 브랜치 길이를 노드 개수에 맞춰 조정
        RectTransform subBranchRect = subBranchUI.GetComponent<RectTransform>();
        if (subBranchRect != null)
        {
            // 프리팹의 기존 위치 저장
            Vector2 originalPosition = subBranchRect.anchoredPosition;
            
            // 노드 개수에 따라 너비 조정
            float calculatedWidth = nodeWidth * nodeCount;
            subBranchRect.sizeDelta = new Vector2(calculatedWidth, subBranchRect.sizeDelta.y);
            
            // 기존 위치에 계산된 오프셋 추가
            float xOffset = subBranchGapX * depth;
            subBranchRect.anchoredPosition = new Vector2(originalPosition.x + xOffset, originalPosition.y + currentYPosition);
            
            // Debug.Log($"{branchPrefix}SubBranch {subBranchSO.subBranchType} - Width: {calculatedWidth} (Nodes: {nodeCount}), Position: X={originalPosition.x + xOffset}, Y={originalPosition.y + currentYPosition} (depth: {depth})");
        }

        // 노드 컨테이너 찾기
        Transform nodeContainer = subBranchUI.transform.Find("NodeContainer");
        if (nodeContainer == null)
        {
            nodeContainer = subBranchUI.transform;
        }

        // 다음 Y 위치 계산 (서브브랜치 높이 + Y 간격)
        float nextYPosition = currentYPosition - subBranchHeight - subBranchGapY;
        int totalCreatedCount = 1; // 현재 서브브랜치

        // 노드(BaseForgeSO) 생성 및 LockedSubBranch 체크
        if (subBranchSO.baseForgeSOs != null)
        {
            // 각 ForgeId가 몇 번째인지 카운트
            Dictionary<ForgeId, int> forgeIdCount = new Dictionary<ForgeId, int>();
            
            for (int i = 0; i < subBranchSO.baseForgeSOs.Length; i++)
            {
                var forgeSO = subBranchSO.baseForgeSOs[i];
                
                // 같은 ForgeId 내에서 몇 번째인지 계산
                if (!forgeIdCount.ContainsKey(forgeSO.forgeId))
                {
                    forgeIdCount[forgeSO.forgeId] = 0;
                }
                int indexInSameId = forgeIdCount[forgeSO.forgeId];
                forgeIdCount[forgeSO.forgeId]++;
                
                CreateForgeNodeUI(forgeSO, subBranchSO.subBranchType, indexInSameId, nodeContainer, depth);
                
                // postSubBranches가 있으면 바로 다음에 LockedSubBranch 생성
                if (forgeSO.postSubBranches != null && forgeSO.postSubBranches.Length > 0)
                {
                    foreach (var lockedSubBranch in forgeSO.postSubBranches)
                    {
                        var result = CreateSubBranchUI(lockedSubBranch, parent, nextYPosition, depth + 1);
                        nextYPosition = result.nextYPosition;
                        totalCreatedCount += result.createdCount;
                    }
                }
            }
        }

        return (nextYPosition, totalCreatedCount);
    }

    private void CreateForgeNodeUI(BaseForgeSO forgeSO, SubBranchType subBranchType, int indexInSameId, Transform parent, int depth)
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

        // X 위치는 부모(SubBranch)가 이미 설정했으므로 여기서는 설정하지 않음
        // depth에 따른 추가 X 오프셋이 필요한 경우에만 설정
        RectTransform nodeRect = nodeUI.GetComponent<RectTransform>();
        if (nodeRect != null && depth > 0)
        {
            // 노드는 기본적으로 부모를 따르므로 추가 오프셋 불필요
            // 필요시 여기서 추가 조정
        }

        // 노드 UI 컴포넌트 가져오기
        var nodeUIComponent = nodeUI.GetComponent<ForgeNodeUI>();
        if (nodeUIComponent != null)
        {
            nodeUIComponent.Initialize(forgeSO, subBranchType, indexInSameId, forgeManager, OnForgeNodeClicked);
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

    // 노드 버튼이 클릭되었을 때 호출되는 콜백 (차징 완료 시)
    private void OnForgeNodeClicked(BaseForgeSO forgeSO)
    {
        if (forgeSO == null) return;
        
        Debug.Log("=================================================");
        Debug.Log($"<color=cyan>[Forge Apply Attempt]</color>");
        Debug.Log($"<color=yellow>Name:</color> {forgeSO.upgradeName}");
        Debug.Log($"<color=yellow>ForgeId:</color> {forgeSO.forgeId}");
        
        // ForgeManger 확인
        if (forgeManager == null)
        {
            Debug.LogError("<color=red>ForgeManger not found!</color>");
            Debug.Log("=================================================");
            return;
        }
        
        // 인벤토리 매니저 확인
        if (inventoryManger == null)
        {
            inventoryManger = FindFirstObjectByType<InventoryManger>();
            if (inventoryManger == null)
            {
                Debug.LogError("<color=red>InventoryManger not found!</color>");
                Debug.Log("=================================================");
                return;
            }
        }
        
        // 비용 체크
        if (!inventoryManger.CheckOre(forgeSO))
        {
            Debug.LogWarning($"<color=red>[Forge Failed]</color> Not enough resources!");
            Debug.Log($"<color=yellow>Required:</color>");
            Debug.Log($"  - Coal: {forgeSO.coalCost}");
            Debug.Log($"  - Iron: {forgeSO.ironCost}");
            Debug.Log($"  - Gold: {forgeSO.goldCost}");
            Debug.Log($"  - Diamond: {forgeSO.diamondCost}");
            Debug.Log($"<color=yellow>Current:</color>");
            Debug.Log($"  - Coal: {inventoryManger.OreList[(int)OreType.Coal]}");
            Debug.Log($"  - Iron: {inventoryManger.OreList[(int)OreType.Iron]}");
            Debug.Log($"  - Gold: {inventoryManger.OreList[(int)OreType.Gold]}");
            Debug.Log($"  - Diamond: {inventoryManger.OreList[(int)OreType.Diamond]}");
            Debug.Log("=================================================");
            return;
        }
        
        // 비용 차감
        if (inventoryManger.ConsumeOre(forgeSO))
        {
            Debug.Log($"<color=green>[Resources Consumed]</color>");
            Debug.Log($"  - Coal: -{forgeSO.coalCost}");
            Debug.Log($"  - Iron: -{forgeSO.ironCost}");
            Debug.Log($"  - Gold: -{forgeSO.goldCost}");
            Debug.Log($"  - Diamond: -{forgeSO.diamondCost}");
            
            // ForgeManger를 통해 강화 적용
            forgeManager.ForgeApply(forgeSO);
            Debug.Log($"<color=green>[Forge Applied Successfully!]</color> {forgeSO.upgradeName}");
            
            // UI 갱신 (후행 브랜치 언락 or 인덱스 변경)
            bool needsRefresh = false;
            
            // postSubBranches가 있으면 UI 재생성
            if (forgeSO.postSubBranches != null && forgeSO.postSubBranches.Length > 0)
            {
                Debug.Log($"<color=magenta>Unlocked SubBranches:</color>");
                foreach (var subBranch in forgeSO.postSubBranches)
                {
                    Debug.Log($"  - {subBranch.subBranchType}");
                }
                needsRefresh = true;
            }
            
            // 인덱스가 변경되었으므로 UI 갱신
            if (needsRefresh)
            {
                GenerateForgeUI(); // 전체 재생성
            }
            else
            {
                RefreshAllNodes(); // 잠금 상태만 업데이트
            }
        }
        else
        {
            Debug.LogError($"<color=red>[Forge Failed]</color> ConsumeOre returned false!");
        }
        
        Debug.Log("=================================================");
    }
    
    // 모든 노드의 잠금 상태 갱신
    private void RefreshAllNodes()
    {
        foreach (var nodeUI in forgeNodeUIObjects.Values)
        {
            if (nodeUI != null)
            {
                var nodeComponent = nodeUI.GetComponent<ForgeNodeUI>();
                if (nodeComponent != null)
                {
                    nodeComponent.RefreshUI();
                }
            }
        }
    }

    private void ClearExistingUI()
    {
        // 기존 UI 요소들 제거 (런타임 전용)
        foreach (var ui in mainBranchUIObjects.Values)
        {
            if (ui != null)
            {
                Destroy(ui);
            }
        }
        mainBranchUIObjects.Clear();
        
        // subBranchUIObjects.Clear();
        forgeNodeUIObjects.Clear();
    }

    void Start()
    {
        // InventoryManger 찾기
        if (inventoryManger == null)
        {
            inventoryManger = FindFirstObjectByType<InventoryManger>();
            if (inventoryManger == null)
            {
                Debug.LogWarning("InventoryManger not assigned and not found in scene!");
            }
        }
        
        // Tooltip 초기화
        if (tooltipUI != null)
        {
            ForgeNodeUI.SetTooltip(tooltipUI);
            tooltipUI.Hide(); // 시작 시 숨김
        }
        else
        {
            Debug.LogError("ForgeTooltipUI is not assigned in ForgeUI!");
        }

        // 런타임 시작 시 자동으로 UI 생성
        GenerateForgeUI();
    }
}
