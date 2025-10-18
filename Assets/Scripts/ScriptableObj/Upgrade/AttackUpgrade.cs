using UnityEngine;
public enum WeaponUpgradeType
{
    CannonAtkDamage,
    CannonAtkRange,
    CannonAtkSpeed,
    CannonMoveSpeed
}
[CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "ScriptableObjects/AttackUpgrades")]
public class AttackUpgrade : ScriptableObject
{
    public WeaponUpgradeType upgradeType;
    public string upgradeName;
    public string upgradeDescription;
    public float baseValue;
    public LevelRecipe[] recipes;
}
