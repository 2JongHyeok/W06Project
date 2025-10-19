using UnityEngine;

public enum SubBranchType
{
    None,
    // SpaceShip Move
    SpaceShipMoveMaxSpeed,
    SpaceShipMoveOrePerSlow,
    // SpaceShip Mining
    SpaceShipMiningDamage,
    SpaceShipMiningRadius,
    SpaceShipMiningSpeed,
    // Attacks GuidedMissile
    GuidedMissileUnlock,
    GuidedMissileAtkDamage,
    GuidedMissileAtkSpeed,
    // Attacks MainCannon
    MainCannonAtkDamage,
    MainCannonAtkSpeed,
    MainCannonBulletNumber,
    MainCannonMoveSpeed,
    //Planet 
    PlanetCoreMaxHp,
    PlanetHpRegenAmount,
    PlanetShieldMaxHp,
    PlanetShieldRegenSpeed
}
[CreateAssetMenu(fileName = "SubBranchSO", menuName = "ScriptableObjects/Forge/Branch/SubBranchSO", order = 1)]
public class SubBranchSO : BranchSO
{
    public SubBranchType subBranchType;
    public BaseForgeSO[] baseForgeSOs;
}
