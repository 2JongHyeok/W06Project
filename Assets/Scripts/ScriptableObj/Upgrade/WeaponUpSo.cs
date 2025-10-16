using UnityEngine;
// [System.Serializable]
// public struct AttackSpeed
// {
//     public float baseValue;
//     public float[] levelValue;
//     public Recipe[] recipes;
// }
// [System.Serializable]
// public struct AttackRange
// {
//     public float baseValue;
//     public float[] levelValue;
//     public Recipe[] recipes;
// }
// [System.Serializable]
// public struct AttackNumber
// {
//     public float baseValue;
//     public float[] levelValue;
//     public Recipe[] recipes;
// }
// [System.Serializable]
// public struct CannonSpeed
// {
//     public float baseValue;
//     public float[] levelValue;
//     public Recipe[] recipes;
// }
public enum WeaponUpgradeType
{
    AttackSpeed,
    AttackRange,
    AttackNumber,
    CannonSpeed
}
[System.Serializable]
public struct AttackUpgrade
{
    public float baseValue;
    public float[] levelValue;
    public Recipe[] recipes;
}
[CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "ScriptableObjects/Weapon Upgrade")]
public class WeaponUpSo : ScriptableObject
{
    public AttackUpgrade attackSpeed;
    public AttackUpgrade attackRange;
    public AttackUpgrade attackNumber;
    public AttackUpgrade cannonSpeed;
}
