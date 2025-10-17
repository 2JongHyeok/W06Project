using Unity.VisualScripting;
using UnityEngine;

public class ForgeManger : MonoBehaviour
{
    [SerializeField] InventoryManger inventoryManger;
    public ForcepUpgrade[] forcepSOList;
    public AttackUpgrade[] weaponUpSOList;
    [SerializeField] private ForgeUI forgeUI;
    public int[] weaponLevelList;
    public int[] forcepLevelList;

    private void Start()
    {
        weaponLevelList = new int[weaponUpSOList.Length];
        forcepLevelList = new int[forcepSOList.Length];

        // 안전하게 할당: 인스펙터에 할당되어 있지 않다면 같은 GameObject에서 찾기
        if (forgeUI == null)
            forgeUI = GetComponent<ForgeUI>();

        if (forgeUI != null)
            forgeUI.CreateForgeUI(this);
        else
            Debug.LogError("ForgeUI not assigned and not found on the same GameObject.");

        for (int i = 0; i < weaponLevelList.Length; i++)
        {
            weaponLevelList[i] = 0;
        }
        for (int i = 0; i < forcepLevelList.Length; i++)
        {
            forcepLevelList[i] = 0;
        }
        
        Weapon.Instance.SetAttackSpeed(weaponUpSOList[(int)WeaponUpgradeType.AttackDamage].baseValue);
        Weapon.Instance.SetDamage((int)weaponUpSOList[(int)WeaponUpgradeType.AttackNumber].baseValue);
        Weapon.Instance.SetExplosionRange((int)weaponUpSOList[(int)WeaponUpgradeType.AttackRange].baseValue);
        Weapon.Instance.SetCannonSpeed((int)weaponUpSOList[(int)WeaponUpgradeType.CannonSpeed].baseValue);
    }

    public void UpgradeWeapon(int index)
    {
        if (index < 0 || index >= weaponUpSOList.Length) return;
        if (weaponLevelList[index] >= weaponUpSOList[index].recipes.Length) return;
        
        float levelValue = weaponUpSOList[index].recipes[weaponLevelList[index]].levelValue;
        Cost[] cost = weaponUpSOList[index].recipes[weaponLevelList[index]].cost;
        Debug.Log("Attempting to upgrade weapon at index " + index + " with levelValue " + levelValue);
        if(inventoryManger.CheckOre(cost))
        {
            foreach (var item in cost)
            {
                inventoryManger.RemoveOre(item.oreType, item.amount);
            }
            if (index == (int)WeaponUpgradeType.AttackDamage)
            {
                Weapon.Instance.AddDamage((int)levelValue);
            }
            else if (index == (int)WeaponUpgradeType.AttackNumber)
            {
                Weapon.Instance.AddAttackSpeed(levelValue);
            }
            else if (index == (int)WeaponUpgradeType.AttackRange)
            {
                Weapon.Instance.AddExplosionRange((int)levelValue);
            }
            else if (index == (int)WeaponUpgradeType.CannonSpeed)
            {
                Weapon.Instance.AddCannonSpeed((int)levelValue);
            }
            weaponLevelList[index]++;
        }
        
    }

    public void UpgradeForcep(int index)
    {
        if (index < 0 || index >= forcepSOList.Length) return;
        if (forcepLevelList[index] >= forcepSOList[index].recipes.Length) return;
        
        float levelValue = forcepSOList[index].recipes[forcepLevelList[index]].levelValue;
        Cost[] cost = forcepSOList[index].recipes[forcepLevelList[index]].cost;
        Debug.Log("Attempting to upgrade forcep at index " + index + " with levelValue " + levelValue);
        if (inventoryManger.CheckOre(cost))
        {
            foreach (var item in cost)
            {
                inventoryManger.RemoveOre(item.oreType, item.amount);
            }

            if (index == (int)ForcepUpgradeType.ForcepBasicSpeed)
            {
                WinchController.Instance.AddRopeSpeed(levelValue);
            }
            forcepLevelList[index]++;
        }
        
    }
}
