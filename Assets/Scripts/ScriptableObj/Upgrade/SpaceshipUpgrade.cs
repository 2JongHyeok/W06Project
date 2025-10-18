using UnityEngine;

public enum SpaceshipUpgradeType
{
    SpaceshipSpeed,
    SpaceshipMassReduceRate,
    SpaceshipDamage,
    SpaceshipRadius,
    SpaceshipAtkSpeed
}
[CreateAssetMenu(fileName = "New Forcep Upgrade", menuName = "ScriptableObjects/SpaceshipUpgrade")]
public class SpaceshipUpgrade : BaseUpgrade
{
    public SpaceshipUpgradeType upgradeType;
}
