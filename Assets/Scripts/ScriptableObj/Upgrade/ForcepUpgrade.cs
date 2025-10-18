using UnityEngine;

public enum ForcepUpgradeType
{
    ForcepCapacity,
    ForcepBasicSpeed,
    ForcepMaxSpeed
}
[CreateAssetMenu(fileName = "New Forcep Upgrade", menuName = "ScriptableObjects/ForcepUpgrade")]
public class ForcepUpgrade : BaseUpgrade
{
    public ForcepUpgradeType upgradeType;
}
