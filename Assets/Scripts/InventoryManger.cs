using System.Collections.Generic;
using UnityEngine;

public class InventoryManger : MonoBehaviour
{
    public InventoryManger Instance { get; private set; }
    [SerializeField] private OreSO[] orePools;
    public int[] OreList;
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
            Debug.Log("Added 100 of each ore type.");
        }
    }
    public void AddOre(OreType oreType, int amount)
    {
        OreList[(int)oreType] += amount;
        inventoryUI.UpdateOreUI(oreType, OreList[(int)oreType]);
    }
    public bool RemoveOre(OreType oreType, int amount)
    {
        if (OreList[(int)oreType] >= amount)
        {
            OreList[(int)oreType] -= amount;
            inventoryUI.UpdateOreUI(oreType, OreList[(int)oreType]);
            return true;
        }
        return false;
    }

    public bool CheckOre(Cost[] costs)
    {
        foreach (var item in costs)
        {
            if (OreList[(int)item.oreType] < item.amount)
            {
                return false;
            }
        }
        return true;
    }
}
