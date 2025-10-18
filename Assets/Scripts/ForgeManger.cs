using Unity.VisualScripting;
using UnityEngine;

public class ForgeManger : MonoBehaviour
{
    public TurretActivationManager turretActivationManager;
    public InventoryManger inventoryManger;
    public SpaceshipUpgrade[] spaceshipSOList;
    public AttackUpgrade[] weaponUpSOList;
    public AutoAttackUpgrade[] autoAttackSOList;
    [SerializeField] private ForgeUI forgeUI;
    public int[] weaponLevelList;
    public int[] spaceshipLevelList;
    public int[] autoAttackLevelList;

    private void Start()
    {
        weaponLevelList = new int[weaponUpSOList.Length];
        spaceshipLevelList = new int[spaceshipSOList.Length];
        autoAttackLevelList = new int[autoAttackSOList.Length];

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
        for (int i = 0; i < spaceshipLevelList.Length; i++)
        {
            spaceshipLevelList[i] = 0;
        }
        for (int i = 0; i < autoAttackLevelList.Length; i++)
        {
            autoAttackLevelList[i] = 0;
        }

        forgeUI.ClearUpgradeInfo();

        Weapon.Instance.SetDamage((int)weaponUpSOList[(int)WeaponUpgradeType.CannonAtkDamage].baseValue);
        Weapon.Instance.SetAttackSpeed(weaponUpSOList[(int)WeaponUpgradeType.CannonAtkSpeed].baseValue);
        Weapon.Instance.SetExplosionRange(weaponUpSOList[(int)WeaponUpgradeType.CannonAtkRange].baseValue);
        Weapon.Instance.SetCannonSpeed(weaponUpSOList[(int)WeaponUpgradeType.CannonMoveSpeed].baseValue);

        SpaceshipMotor.Instance.SetThrustPower(spaceshipSOList[(int)SpaceshipUpgradeType.SpaceshipSpeed].baseValue);
        SpaceshipMotor.Instance.SetThrustReductionPerOre(spaceshipSOList[(int)SpaceshipUpgradeType.SpaceshipMassReduceRate].baseValue);
        SpaceshipWeapon.Instance.SetDamage((int)spaceshipSOList[(int)SpaceshipUpgradeType.SpaceshipDamage].baseValue);
        SpaceshipWeapon.Instance.SetExplosionRadius(spaceshipSOList[(int)SpaceshipUpgradeType.SpaceshipRadius].baseValue);
        SpaceshipWeapon.Instance.SetAttackSpeed(spaceshipSOList[(int)SpaceshipUpgradeType.SpaceshipAtkSpeed].baseValue);

        turretActivationManager.SetMissileDamage(autoAttackSOList[(int)AutoAttackUpgradeType.AutoAttackDamage].baseValue);
        turretActivationManager.SetMissileInterval(autoAttackSOList[(int)AutoAttackUpgradeType.AutoAttackInterval].baseValue);
    }

    public void UpgradeWeapon(int index)
    {
        if (index < 0 || index >= weaponUpSOList.Length) return;
        if (weaponLevelList[index] >= weaponUpSOList[index].recipes.Length) return;
        
        float levelValue = weaponUpSOList[index].recipes[weaponLevelList[index]].levelValue;
        Cost[] cost = weaponUpSOList[index].recipes[weaponLevelList[index]].cost;
        if(inventoryManger.CheckOre(cost))
        {
            foreach (var item in cost)
            {
                inventoryManger.RemoveOre(item.oreType, item.amount);
            }
            if (index == (int)WeaponUpgradeType.CannonAtkDamage)
            {
                Weapon.Instance.AddDamage((int)levelValue);
            }
            else if (index == (int)WeaponUpgradeType.CannonAtkSpeed)
            {
                Weapon.Instance.AddAttackSpeed(levelValue);
            }
            else if (index == (int)WeaponUpgradeType.CannonAtkRange)
            {
                Weapon.Instance.AddExplosionRange(levelValue);
            }
            else if (index == (int)WeaponUpgradeType.CannonMoveSpeed)
            {
                Weapon.Instance.AddCannonSpeed(levelValue);
            }
            weaponLevelList[index]++;
            forgeUI.ClearUpgradeInfo();
        }
        
    }
    public void UpgradeSpaceship(int index)
    {
        if (index < 0 || index >= spaceshipSOList.Length) return;
        if (spaceshipLevelList[index] >= spaceshipSOList[index].recipes.Length) return;

        float levelValue = spaceshipSOList[index].recipes[spaceshipLevelList[index]].levelValue;
        Cost[] cost = spaceshipSOList[index].recipes[spaceshipLevelList[index]].cost;
        if (inventoryManger.CheckOre(cost))
        {
            foreach (var item in cost)
            {
                inventoryManger.RemoveOre(item.oreType, item.amount);
            }

            if (index == (int)SpaceshipUpgradeType.SpaceshipSpeed)
            {
                SpaceshipMotor.Instance.AddThrustPower(levelValue);
            }
            if (index == (int)SpaceshipUpgradeType.SpaceshipMassReduceRate)
            {
                SpaceshipMotor.Instance.AddThrustReductionPerOre(levelValue);
            }
            if (index == (int)SpaceshipUpgradeType.SpaceshipDamage)
            {
                SpaceshipWeapon.Instance.AddDamage((int)levelValue);
            }
            if (index == (int)SpaceshipUpgradeType.SpaceshipRadius)
            {
                SpaceshipWeapon.Instance.AddExplosionRadius(levelValue);
            }
            if (index == (int)SpaceshipUpgradeType.SpaceshipAtkSpeed)
            {
                SpaceshipWeapon.Instance.AddAttackSpeed(levelValue);
            }
            spaceshipLevelList[index]++;
            forgeUI.ClearUpgradeInfo();
        }
    }
    
    public void UpgradeAutoAttack(int index)
    {
        if (index < 0 || index >= autoAttackSOList.Length) return;
        if (autoAttackLevelList[index] >= autoAttackSOList[index].recipes.Length) return;
        
        float levelValue = autoAttackSOList[index].recipes[autoAttackLevelList[index]].levelValue;
        Cost[] cost = autoAttackSOList[index].recipes[autoAttackLevelList[index]].cost;
        if (inventoryManger.CheckOre(cost))
        {
            foreach (var item in cost)
            {
                inventoryManger.RemoveOre(item.oreType, item.amount);
            }
            if (index == (int)AutoAttackUpgradeType.AutoAttackDamage)
            {
                turretActivationManager.AddMissileDamage(levelValue);
                turretActivationManager.AddMissileTurret();
            }
            if (index == (int)AutoAttackUpgradeType.AutoAttackInterval)
            {
                turretActivationManager.AddMissileInterval(levelValue);
            }
            autoAttackLevelList[index]++;
            forgeUI.ClearUpgradeInfo();
        }
    }
}
