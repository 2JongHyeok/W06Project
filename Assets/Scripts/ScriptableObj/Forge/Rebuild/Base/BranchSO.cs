using UnityEngine;
public enum ForgeBranchType
{
    SpaceShip,
    SpaceShipMovement,
    Cannon,
    GuidedMissile,
    AutoCannon,
    Planet,
    PlanetShield,
}
public class BranchSO : ScriptableObject
{
    public ForgeBranchType branchType;
    public BaseForgeSO[] upgrades;
}
