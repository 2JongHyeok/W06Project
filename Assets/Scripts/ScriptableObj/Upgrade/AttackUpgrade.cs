using UnityEngine;
public enum WeaponUpgradeType
{
    AttackDamage,
    AttackRange,
    AttackNumber,
    CannonSpeed
}
[CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "ScriptableObjects/AttackUpgrades")]
public class AttackUpgrade : ScriptableObject
{
    public WeaponUpgradeType upgradeType;
    public float baseValue;
    public LevelRecipe[] recipes;
}
