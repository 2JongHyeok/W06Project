using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeUI : MonoBehaviour
{
    public float nodeSize = 100f;
    public float nodeSpacing = 20f;
    public float subTreeSpacing = 50f;
    public float subTreeHeight = 100f;
    private int MaxNodeCount;
    [SerializeField] private GameObject ForgePanel;
    [SerializeField] private GameObject UIMainGrid;
    [SerializeField] private GameObject UITextGrid;
    [SerializeField] private GameObject SubTreePrefab;
    [SerializeField] private GameObject NodePrefab;
    [SerializeField] private GameObject ForgeTitleTextPrefab;

    // 버튼 리스트 저장 (각 업그레이드별 레벨 버튼들)
    private List<List<Button>> weaponButtons = new List<List<Button>>();
    private List<List<Color>> weaponButtonDefaultColors = new List<List<Color>>();
    private List<List<Button>> forcepButtons = new List<List<Button>>();
    private List<List<Color>> forcepButtonDefaultColors = new List<List<Color>>();

    [Header("ForgeInfo")]
    public BaseUpgrade CurrentSelectUpgrade;
    public TMP_Text ForgeTitleText;
    public TMP_Text ForgeDescText;
    public GameObject HasGrid;
    public GameObject CostGrid;
    public Button ForgeApplyButton;
    public Color32 ActiveColor;
    public Color32 DeactiveColor;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            ToggleForgePanel();
        }
    }
    public void ToggleForgePanel()
    {
        if (ForgePanel != null)
        {
            bool isActive = ForgePanel.activeSelf;
            ForgePanel.SetActive(!isActive);

            Time.timeScale = isActive ? 1f : 0f; // 포지 UI 열 때 시간 정지, 닫을 때 시간 재개
        }
    }
    public void ClearUpgradeInfo()
    {
        ForgeTitleText.text = "";
        ForgeDescText.text = "";
        for (int i = 0; i < HasGrid.transform.childCount; i++)
        {
        // HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
        CostGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = "0";
        }
        for (int i = 0; i < CostGrid.transform.childCount; i++)
        {
            // HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
            CostGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = "0";
        }
        ForgeApplyButton.interactable = false;
        Image img = ForgeApplyButton.GetComponent<Image>();
        img.color = DeactiveColor;
    }
    public void SelectUpgrade(BaseUpgrade _Upgrade, InventoryManger _inven, ForgeManger forgeManger)
    {
        ForgeTitleText.text = _Upgrade.upgradeName;
        ForgeDescText.text = _Upgrade.upgradeDescription;
        int nextLevel;
        LevelRecipe levelRecipe;
        if (_Upgrade is AttackUpgrade)
        {
            nextLevel = forgeManger.weaponLevelList[(int)(_Upgrade as AttackUpgrade).upgradeType];
            levelRecipe = _Upgrade.recipes[nextLevel];
            for (int i = 0; i < _inven.OreList.Length; i++)
            {
                HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
                // CostGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = levelRecipe.cost[i].amount.ToString();
            }

            for (int i = 0; i < levelRecipe.cost.Length; i++)
            {
                // HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
                CostGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = levelRecipe.cost[i].amount.ToString();
            }
            Image img = ForgeApplyButton.GetComponent<Image>();
            ForgeApplyButton.onClick.RemoveAllListeners();
            ForgeApplyButton.onClick.AddListener(() =>
                    {
                        // 실제 업그레이드 시도
                        forgeManger.UpgradeWeapon((int)(_Upgrade as AttackUpgrade).upgradeType);
                        if (_inven.CheckOre(levelRecipe.cost))
                        {
                            int nextLevel = forgeManger.weaponLevelList[(int)(_Upgrade as AttackUpgrade).upgradeType];
                            var currentBtn = weaponButtons[(int)(_Upgrade as AttackUpgrade).upgradeType][nextLevel-1];
                            var currnetImg = currentBtn.GetComponent<Image>();
                            currentBtn.interactable = false;
                            if (currnetImg != null) currnetImg.color = DeactiveColor;
                            if (nextLevel < weaponButtons[(int)(_Upgrade as AttackUpgrade).upgradeType].Count)
                            {
                                var nextBtn = weaponButtons[(int)(_Upgrade as AttackUpgrade).upgradeType][nextLevel];
                                var nextImg = nextBtn.GetComponent<Image>();
                                nextBtn.interactable = true;
                                if (nextImg != null) nextImg.color = ActiveColor;
                            }
                        } 
                    });
            if (_inven.CheckOre(levelRecipe.cost))
            {
                ForgeApplyButton.interactable = true;
                if (img != null) img.color = ActiveColor;
            } else
            {
                ForgeApplyButton.interactable = false;
                if (img != null) img.color = DeactiveColor;
            }
           
        }
        if(_Upgrade is ForcepUpgrade)
        {
            nextLevel = forgeManger.weaponLevelList[(int)(_Upgrade as ForcepUpgrade).upgradeType];
            levelRecipe = _Upgrade.recipes[nextLevel];
            for (int i = 0; i < _inven.OreList.Length; i++)
            {

                HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
                CostGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = levelRecipe.cost[i].amount.ToString();
            }
            Image img = ForgeApplyButton.GetComponent<Image>();
            ForgeApplyButton.onClick.RemoveAllListeners();
            ForgeApplyButton.onClick.AddListener(() =>
                    {
                        forgeManger.UpgradeForcep((int)(_Upgrade as ForcepUpgrade).upgradeType);
                        if (_inven.CheckOre(levelRecipe.cost))
                        {
                            int nextLevel = forgeManger.forcepLevelList[(int)(_Upgrade as ForcepUpgrade).upgradeType];
                            var currentBtn = weaponButtons[(int)(_Upgrade as ForcepUpgrade).upgradeType][nextLevel];
                            var currnetImg = currentBtn.GetComponent<Image>();
                            currentBtn.interactable = true;
                            if (currnetImg != null) currnetImg.color = ActiveColor;
                            if (nextLevel < forcepButtons[(int)(_Upgrade as ForcepUpgrade).upgradeType].Count)
                            {
                                var nextBtn = forcepButtons[(int)(_Upgrade as ForcepUpgrade).upgradeType][nextLevel];
                                var nextImg = nextBtn.GetComponent<Image>();
                                nextBtn.interactable = true;
                                if (nextImg != null) nextImg.color = ActiveColor;
                            }
                        } 
                        
                    });
            if (_inven.CheckOre(levelRecipe.cost))
            {
                ForgeApplyButton.interactable = true;
                if (img != null) img.color = ActiveColor;
            } else
            {
                ForgeApplyButton.interactable = false;
                if (img != null) img.color = DeactiveColor;
            }
        }
    }
    public void CreateForgeUI(ForgeManger forgeManger)
    {
        weaponButtons.Clear();
        weaponButtonDefaultColors.Clear();
        forcepButtons.Clear();
        forcepButtonDefaultColors.Clear();
        int MaxSubTreeCount = forgeManger.weaponUpSOList.Length + forgeManger.forcepSOList.Length;
        Debug.Log("Creating Forge UI with " + MaxSubTreeCount + " subtrees.");
        // Weapons
        for (int i = 0; i < forgeManger.weaponUpSOList.Length; i++)
        {
            if (MaxNodeCount < forgeManger.weaponUpSOList[i].recipes.Length)
                MaxNodeCount = forgeManger.weaponUpSOList[i].recipes.Length;
            int weaponIndex = i; // 캡처용 로컬 복사
            GameObject titleText = Instantiate(ForgeTitleTextPrefab, UITextGrid.transform);
            titleText.GetComponent<TMP_Text>().text = forgeManger.weaponUpSOList[weaponIndex].upgradeName;
            GameObject subTree = Instantiate(SubTreePrefab, UIMainGrid.transform);

            var btnList = new List<Button>();
            var colorList = new List<Color>();

            for (int j = 0; j < forgeManger.weaponUpSOList[weaponIndex].recipes.Length; j++)
            {
                int levelIndex = j; // 캡처용
                GameObject node = Instantiate(NodePrefab, subTree.transform);

                // 버튼은 노드 자신이나 자식에 있을 수 있으므로 안전하게 검색
                var btn = node.GetComponent<Button>() ?? node.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();

                    // 기본 색상 저장 (이미지 색 사용)
                    Image img = btn.GetComponent<Image>();
                    Color defaultColor = img != null ? ActiveColor : ActiveColor;
                    colorList.Add(defaultColor);
                    btnList.Add(btn);

                    // 초기 활성화: 현재 레벨과 같으면 활성화, 아니면 비활성화
                    bool shouldBeInteractable = (forgeManger.weaponLevelList.Length > weaponIndex && forgeManger.weaponLevelList[weaponIndex] == levelIndex);
                    btn.interactable = shouldBeInteractable;
                    if (!shouldBeInteractable && img != null)
                        img.color = DeactiveColor;

                    // 리스너: 클릭 시 매니저 호출 후 UI 업데이트
                    btn.onClick.AddListener(() =>
                    {
                        // 실제 업그레이드 시도
                        // forgeManger.UpgradeWeapon(weaponIndex);

                        // // 비활성화 및 색 변경 (클릭한 버튼)
                        btn.interactable = false;
                        if (img != null) img.color = DeactiveColor;

                        // 다음 레벨 버튼이 있으면 활성화(데이터 존재 확인)
                        int nextLevel = forgeManger.weaponLevelList[weaponIndex];
                        if (nextLevel < btnList.Count)
                        {
                            var nextBtn = btnList[nextLevel];
                            var nextImg = nextBtn.GetComponent<Image>();
                            nextBtn.interactable = true;
                            if (nextImg != null) nextImg.color = ActiveColor;
                        }
                        SelectUpgrade(forgeManger.weaponUpSOList[weaponIndex], forgeManger.inventoryManger, forgeManger);
                    });
                }
            }

            weaponButtons.Add(btnList);
            weaponButtonDefaultColors.Add(colorList);
        }

        // Forcep
        for (int i = 0; i < forgeManger.forcepSOList.Length; i++)
        {
            if (MaxNodeCount < forgeManger.forcepSOList[i].recipes.Length)
                MaxNodeCount = forgeManger.forcepSOList[i].recipes.Length;
            int forcepIndex = i;
            GameObject titleText = Instantiate(ForgeTitleTextPrefab, UITextGrid.transform);
            titleText.GetComponent<TMP_Text>().text = forgeManger.forcepSOList[forcepIndex].upgradeType.ToString();
            GameObject subTree = Instantiate(SubTreePrefab, UIMainGrid.transform);

            var btnList = new List<Button>();
            var colorList = new List<Color>();

            for (int j = 0; j < forgeManger.forcepSOList[forcepIndex].recipes.Length; j++)
            {
                int levelIndex = j;
                GameObject node = Instantiate(NodePrefab, subTree.transform);

                var btn = node.GetComponent<Button>() ?? node.GetComponentInChildren<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();

                    Image img = btn.GetComponent<Image>();
                    Color defaultColor = img != null ? img.color : Color.white;
                    colorList.Add(defaultColor);
                    btnList.Add(btn);

                    bool shouldBeInteractable = (forgeManger.forcepLevelList.Length > forcepIndex && forgeManger.forcepLevelList[forcepIndex] == levelIndex);
                    btn.interactable = shouldBeInteractable;
                    if (!shouldBeInteractable && img != null)
                        img.color = Color.red;

                    // btn.onClick.AddListener(() =>
                    // {
                    //     forgeManger.UpgradeForcep(forcepIndex);

                    //     btn.interactable = false;
                    //     if (img != null) img.color = Color.red;

                    //     int nextLevel = forgeManger.forcepLevelList[forcepIndex];
                    //     if (nextLevel < btnList.Count)
                    //     {
                    //         var nextBtn = btnList[nextLevel];
                    //         var nextImg = nextBtn.GetComponent<Image>();
                    //         nextBtn.interactable = true;
                    //         if (nextImg != null) nextImg.color = colorList[nextLevel];
                    //     }
                    // });
                    SelectUpgrade(forgeManger.forcepSOList[forcepIndex], forgeManger.inventoryManger, forgeManger);
                }
            }

            forcepButtons.Add(btnList);
            forcepButtonDefaultColors.Add(colorList);
        }
        
        UIMainGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(
            (nodeSize + nodeSpacing) * MaxNodeCount,
            (subTreeSpacing + subTreeHeight) * MaxSubTreeCount
        );
    }
}
