using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeUI : MonoBehaviour
{
    public GameObject SkilGrid;
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
    private List<List<Button>> spaceshipButtons = new List<List<Button>>();
    private List<List<Color>> spaceshipButtonDefaultColors = new List<List<Color>>();
    private List<List<Button>> autoAttackButtons = new List<List<Button>>();
    private List<List<Color>> autoAttackButtonDefaultColors = new List<List<Color>>();

    [Header("ForgeInfo")]
    public BaseUpgrade CurrentSelectUpgrade;
    public TMP_Text ForgeTitleText;
    public TMP_Text ForgeDescText;
    public GameObject HasGrid;
    public GameObject CostGrid;
    public Button ForgeApplyButton;
    public Color32 ActiveColor;
    public Color32 DeactiveColor;
    public bool isForge = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
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
            isForge = !isActive;

            Time.timeScale = isActive ? 1f : 0f; // 포지 UI 열 때 시간 정지, 닫을 때 시간 재개
        }
    }
    public void ClearUpgradeInfo(InventoryManger _inven)
    {
        _inven.AddOre(OreType.Coal, 0);
        ForgeTitleText.text = "";
        ForgeDescText.text = "";
        for (int i = 0; i < HasGrid.transform.childCount; i++)
        {
            // HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
            HasGrid.transform.GetChild(i).GetComponent<TMP_Text>().text = _inven.OreList[i].ToString();
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
        ClearUpgradeInfo(_inven);
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
                            Debug.Log(nextLevel);
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
        if (_Upgrade is SpaceshipUpgrade)
        {
            nextLevel = forgeManger.spaceshipLevelList[(int)(_Upgrade as SpaceshipUpgrade).upgradeType];
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
                        forgeManger.UpgradeSpaceship((int)(_Upgrade as SpaceshipUpgrade).upgradeType);
                        if (_inven.CheckOre(levelRecipe.cost))
                        {
                            int nextLevel = forgeManger.spaceshipLevelList[(int)(_Upgrade as SpaceshipUpgrade).upgradeType];
                            Debug.Log(nextLevel);
                            var currentBtn = spaceshipButtons[(int)(_Upgrade as SpaceshipUpgrade).upgradeType][nextLevel-1];
                            var currnetImg = currentBtn.GetComponent<Image>();
                            currentBtn.interactable = false;
                            if (currnetImg != null) currnetImg.color = DeactiveColor;
                            if (nextLevel < spaceshipButtons[(int)(_Upgrade as SpaceshipUpgrade).upgradeType].Count)
                            {
                                var nextBtn = spaceshipButtons[(int)(_Upgrade as SpaceshipUpgrade).upgradeType][nextLevel];
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
        if (_Upgrade is AutoAttackUpgrade)
        {
            nextLevel = forgeManger.autoAttackLevelList[(int)(_Upgrade as AutoAttackUpgrade).upgradeType];
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
                        forgeManger.UpgradeAutoAttack((int)(_Upgrade as AutoAttackUpgrade).upgradeType);
                        if (_inven.CheckOre(levelRecipe.cost))
                        {
                            int nextLevel = forgeManger.autoAttackLevelList[(int)(_Upgrade as AutoAttackUpgrade).upgradeType];
                            var currentBtn = autoAttackButtons[(int)(_Upgrade as AutoAttackUpgrade).upgradeType][nextLevel-1];
                            var currnetImg = currentBtn.GetComponent<Image>();
                            currentBtn.interactable = false;
                            if (currnetImg != null) currnetImg.color = DeactiveColor;
                            if (nextLevel < autoAttackButtons[(int)(_Upgrade as AutoAttackUpgrade).upgradeType].Count)
                            {
                                var nextBtn = autoAttackButtons[(int)(_Upgrade as AutoAttackUpgrade).upgradeType][nextLevel];
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
        spaceshipButtons.Clear();
        spaceshipButtonDefaultColors.Clear();
        autoAttackButtons.Clear();
        autoAttackButtonDefaultColors.Clear();
        int MaxSubTreeCount = forgeManger.weaponUpSOList.Length + forgeManger.spaceshipSOList.Length + forgeManger.autoAttackSOList.Length;
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
        // Spaceship
        for (int i = 0; i < forgeManger.spaceshipSOList.Length; i++)
        {
            if (MaxNodeCount < forgeManger.spaceshipSOList[i].recipes.Length)
                MaxNodeCount = forgeManger.spaceshipSOList[i].recipes.Length;
            int spaceshipIndex = i; // 캡처용 로컬 복사
            GameObject titleText = Instantiate(ForgeTitleTextPrefab, UITextGrid.transform);
            titleText.GetComponent<TMP_Text>().text = forgeManger.spaceshipSOList[spaceshipIndex].upgradeName;
            GameObject subTree = Instantiate(SubTreePrefab, UIMainGrid.transform);

            var btnList2 = new List<Button>();
            var colorList = new List<Color>();

            for (int j = 0; j < forgeManger.spaceshipSOList[spaceshipIndex].recipes.Length; j++)
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
                    btnList2.Add(btn);

                    // 초기 활성화: 현재 레벨과 같으면 활성화, 아니면 비활성화
                    bool shouldBeInteractable = (forgeManger.spaceshipLevelList.Length > spaceshipIndex && forgeManger.spaceshipLevelList[spaceshipIndex] == levelIndex);
                    btn.interactable = shouldBeInteractable;
                    if (!shouldBeInteractable && img != null)
                        img.color = DeactiveColor;

                    // 리스너: 클릭 시 매니저 호출 후 UI 업데이트
                    btn.onClick.AddListener(() =>
                    {
                        // 실제 업그레이드 시도
                        // forgeManger.UpgradeWeapon(spaceshipIndex);

                        // // 비활성화 및 색 변경 (클릭한 버튼)
                        btn.interactable = false;
                        if (img != null) img.color = DeactiveColor;

                        // 다음 레벨 버튼이 있으면 활성화(데이터 존재 확인)
                        int nextLevel = forgeManger.spaceshipLevelList[spaceshipIndex];
                        if (nextLevel < btnList2.Count)
                        {
                            var nextBtn = btnList2[nextLevel];
                            var nextImg = nextBtn.GetComponent<Image>();
                            nextBtn.interactable = true;
                            if (nextImg != null) nextImg.color = ActiveColor;
                        }
                        SelectUpgrade(forgeManger.spaceshipSOList[spaceshipIndex], forgeManger.inventoryManger, forgeManger);
                    });
                }
            }

            spaceshipButtons.Add(btnList2);
            spaceshipButtonDefaultColors.Add(colorList);
        }
        //autoAttack
        for (int i = 0; i < forgeManger.autoAttackSOList.Length; i++)
        {
            if (MaxNodeCount < forgeManger.autoAttackSOList[i].recipes.Length)
                MaxNodeCount = forgeManger.autoAttackSOList[i].recipes.Length;
            int autoAttackIndex = i; // 캡처용 로컬 복사
            GameObject titleText = Instantiate(ForgeTitleTextPrefab, UITextGrid.transform);
            titleText.GetComponent<TMP_Text>().text = forgeManger.autoAttackSOList[autoAttackIndex].upgradeName;
            GameObject subTree = Instantiate(SubTreePrefab, UIMainGrid.transform);

            var btnList3 = new List<Button>();
            var colorList = new List<Color>();

            for (int j = 0; j < forgeManger.autoAttackSOList[autoAttackIndex].recipes.Length; j++)
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
                    btnList3.Add(btn);

                    // 초기 활성화: 현재 레벨과 같으면 활성화, 아니면 비활성화
                    bool shouldBeInteractable = (forgeManger.autoAttackLevelList.Length > autoAttackIndex && forgeManger.autoAttackLevelList[autoAttackIndex] == levelIndex);
                    btn.interactable = shouldBeInteractable;
                    if (!shouldBeInteractable && img != null)
                        img.color = DeactiveColor;

                    // 리스너: 클릭 시 매니저 호출 후 UI 업데이트
                    btn.onClick.AddListener(() =>
                    {
                        // 실제 업그레이드 시도
                        // forgeManger.UpgradeWeapon(autoAttackIndex);

                        // // 비활성화 및 색 변경 (클릭한 버튼)
                        btn.interactable = false;
                        if (img != null) img.color = DeactiveColor;

                        // 다음 레벨 버튼이 있으면 활성화(데이터 존재 확인)
                        int nextLevel = forgeManger.autoAttackLevelList[autoAttackIndex];
                        if (nextLevel < btnList3.Count)
                        {
                            var nextBtn = btnList3[nextLevel];
                            var nextImg = nextBtn.GetComponent<Image>();
                            nextBtn.interactable = true;
                            if (nextImg != null) nextImg.color = ActiveColor;
                        }
                        SelectUpgrade(forgeManger.autoAttackSOList[autoAttackIndex], forgeManger.inventoryManger, forgeManger);
                    });
                }
            }

            autoAttackButtons.Add(btnList3);
            autoAttackButtonDefaultColors.Add(colorList);
        }
        
        SkilGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(
            1344,
            (subTreeSpacing + subTreeHeight) * MaxSubTreeCount
        );
    }
}
