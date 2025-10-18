using UnityEngine;

public enum AutoAttackUpgradeType
{
    AutoAttackDamage,
    AutoAttackInterval
}
[CreateAssetMenu(fileName = "New Forcep Upgrade", menuName = "ScriptableObjects/AutoAttackUpgrade")]
public class AutoAttackUpgrade : BaseUpgrade
{
    public AutoAttackUpgradeType upgradeType;
}
