using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ForgeUI : MonoBehaviour
{
    [SerializeField] private GameObject UIMainGrid;
    [SerializeField] private GameObject UITextGrid;
    [SerializeField] private GameObject SubTreePrefab;
    [SerializeField] private GameObject NodePrefab;
    [SerializeField] private GameObject ForgeTitleTextPrefab;

    public void CreateForgeUI(ForgeManger forgeManger)
    {
        for (int i = 0; i < forgeManger.weaponUpSOList.Length; i++)
        {
            int weaponIndex = i; // ← 로컬 복사본 캡처
            GameObject titleText = Instantiate(ForgeTitleTextPrefab, UITextGrid.transform);
            titleText.GetComponent<TMP_Text>().text = forgeManger.weaponUpSOList[weaponIndex].upgradeType.ToString();
            GameObject subTree = Instantiate(SubTreePrefab, UIMainGrid.transform);
            for (int j = 0; j < forgeManger.weaponUpSOList[weaponIndex].recipes.Length; j++)
            {
                int levelIndex = j; // 필요하면 사용
                GameObject node = Instantiate(NodePrefab, subTree.transform);
                Debug.Log($"Created node for {forgeManger.weaponUpSOList[weaponIndex].upgradeType} level {levelIndex}");
                var btn = node.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => forgeManger.UpgradeWeapon(weaponIndex));
                }
            }
        }

        for (int i = 0; i < forgeManger.forcepSOList.Length; i++)
        {
            int forcepIndex = i; // ← 로컬 복사본 캡처
            GameObject titleText = Instantiate(ForgeTitleTextPrefab, UITextGrid.transform);
            titleText.GetComponent<TMP_Text>().text = forgeManger.forcepSOList[forcepIndex].upgradeType.ToString();
            GameObject subTree = Instantiate(SubTreePrefab, UIMainGrid.transform);
            for (int j = 0; j < forgeManger.forcepSOList[forcepIndex].recipes.Length; j++)
            {
                int levelIndex = j;
                GameObject node = Instantiate(NodePrefab, subTree.transform);
                var btn = node.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => forgeManger.UpgradeForcep(forcepIndex));
                }
            }
        }
    }
}
