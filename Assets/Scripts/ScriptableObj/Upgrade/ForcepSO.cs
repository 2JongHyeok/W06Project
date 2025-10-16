using UnityEngine;
[System.Serializable]
public struct ForcepUpgrade
{
    public ForcepUpgradeType upgradeType;
    public float baseValue;
    public LevelRecipe[] recipes;
}
public enum ForcepUpgradeType
{
    ForcepCapacity,
    ForcepBasicSpeed,
    ForcepMaxSpeed
}
[CreateAssetMenu(fileName = "New Forcep Upgrade", menuName = "ScriptableObjects/Forcep Upgrade")]
public class ForcepSO : ScriptableObject
{
    public ForcepUpgrade forcepCapacity;
    public ForcepUpgrade forcepBasicSpeepd;
    public ForcepUpgrade forcepMaxSpeed;
}
