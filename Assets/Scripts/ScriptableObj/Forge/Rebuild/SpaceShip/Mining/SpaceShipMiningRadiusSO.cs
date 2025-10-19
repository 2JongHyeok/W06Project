using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveRotationSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveRotationSpeedSO", order = 1)]
public class SpaceShipMiningRadiusSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMiningRadius;
    public int MiningRadius;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
