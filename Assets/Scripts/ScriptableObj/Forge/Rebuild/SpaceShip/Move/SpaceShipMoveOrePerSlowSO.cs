using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveRotationSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveRotationSpeedSO", order = 1)]
public class SpaceShipMoveOrePerSlowSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMoveOrePerSlow;
    public int OrePerSlow;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
