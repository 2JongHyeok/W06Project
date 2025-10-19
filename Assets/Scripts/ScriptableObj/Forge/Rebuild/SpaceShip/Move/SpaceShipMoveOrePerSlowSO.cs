using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveOrePerSlowSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveOrePerSlowSO", order = 1)]
public class SpaceShipMoveOrePerSlowSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMoveOrePerSlow;
    public int OrePerSlow;
    public override void Apply()
    {
        if (Managers.Instance?.spaceshipMotor == null) return;
        Managers.Instance.spaceshipMotor.AddThrustReductionPerOre((float)OrePerSlow);
    }
}
