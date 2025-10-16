using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public struct OreUI
{
    public Image oreIcon;
    public TextMeshProUGUI oreAmountText;
}

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject OriGirdUIPrefab;
    [SerializeField] private RectTransform OreUIParent;
    [SerializeField] private int GridWidth = 100;
    [SerializeField] private int GridSpacing = 10;
    List<OreUI> OreUIList = new List<OreUI>();

    public void CreateOreUI(InventoryManger inventoryManger)
    {
        OreUIParent.sizeDelta = new Vector2((GridWidth + GridSpacing) * inventoryManger.OreList.Length, OreUIParent.sizeDelta.y);
        OreUIList = new List<OreUI>();
        foreach (var ore in inventoryManger.OreList)
        {
            GameObject oreUI = Instantiate(OriGirdUIPrefab, OreUIParent);
            oreUI.transform.SetParent(OreUIParent);
            OreUI oreUIComp = new OreUI
            {
                oreIcon = oreUI.transform.GetChild(1).GetComponent<Image>(),
                oreAmountText = oreUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>()
            };
            OreUIList.Add(oreUIComp);
        }
    }
    public void UpdateOreUI(OreType oreType, int amount)
    {
        OreUIList[(int)oreType].oreAmountText.text = amount.ToString();
    }
}
