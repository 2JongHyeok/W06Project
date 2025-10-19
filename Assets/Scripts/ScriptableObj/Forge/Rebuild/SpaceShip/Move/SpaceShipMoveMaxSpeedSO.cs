using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveRotationSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveRotationSpeedSO", order = 1)]
public class SpaceShipMoveMaxSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMoveMaxSpeed;
    public int MaxSpeed;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
