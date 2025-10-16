using UnityEngine;

public enum ForcepUpgradeType
{
    ForcepCapacity,
    ForcepBasicSpeed,
    ForcepMaxSpeed
}
[CreateAssetMenu(fileName = "New Forcep Upgrade", menuName = "ScriptableObjects/ForcepUpgrade")]
public class ForcepUpgrade : ScriptableObject
{
    public ForcepUpgradeType upgradeType;
    public float baseValue;
    public LevelRecipe[] recipes;
}
