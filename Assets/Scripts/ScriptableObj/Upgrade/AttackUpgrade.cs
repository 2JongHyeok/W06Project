using UnityEngine;
public enum WeaponUpgradeType
{
    CannonAtkDamage,
    CannonAtkRange,
    CannonAtkSpeed,
    CannonMoveSpeed
}
[CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "ScriptableObjects/AttackUpgrades")]
public class AttackUpgrade : BaseUpgrade
{
    public WeaponUpgradeType upgradeType;
}
