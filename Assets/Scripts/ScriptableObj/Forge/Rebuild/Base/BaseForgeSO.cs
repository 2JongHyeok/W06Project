using Unity.VisualScripting;
using UnityEngine;
public enum ForgeId
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
public abstract class BaseForgeSO : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;

    // 고정 4종 광석 비용(필요 시 LevelRecipe로 대체 가능)
    public Cost[] cost;
    [System.Serializable]
    public struct Cost { public OreType oreType; public int amount; }
    public SubBranchSO[] postSubBranches;

    // 핵심: SO가 효과를 적용하는 훅(최초 해금 1회/레벨 적용 공용)
    public abstract void Apply();
}
