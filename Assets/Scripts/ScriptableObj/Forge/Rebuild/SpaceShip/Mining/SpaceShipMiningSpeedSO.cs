using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveRotationSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveRotationSpeedSO", order = 1)]
public class SpaceShipMiningSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMiningSpeed;
    public int MiningSpeed;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
