using System.Collections.Generic;
using UnityEngine;

public class InventoryManger : MonoBehaviour
{
    [SerializeField] private OreSO[] orePools;
    [HideInInspector] public int[] OreList;
    [SerializeField] private InventoryUI inventoryUI;

    private void Start()
    {
        OreList = new int[orePools.Length];
        foreach (var item in orePools)
        {
            OreList[(int)item.oreType] = 0;
        }
        inventoryUI.CreateOreUI(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddOre(OreType.Coal, 100);
            AddOre(OreType.Iron, 100);
            AddOre(OreType.Gold, 100);
            AddOre(OreType.Diamond, 100);
        }
    }
    public void AddOre(OreType oreType, int amount)
    {
        OreList[(int)oreType] += amount;
        inventoryUI.UpdateOreUI(oreType, amount);
    }
    public bool RemoveOre(OreType oreType, int amount)
    {
        if (OreList[(int)oreType] >= amount)
        {
            OreList[(int)oreType] -= amount;
            return true;
        }
        return false;
    }
}
