using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveMaxSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveMaxSpeedSO", order = 1)]
public class SpaceShipMoveMaxSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMoveMaxSpeed;
    public int MaxSpeed;
    public override void Apply()
    {
        if (Managers.Instance?.spaceshipMotor == null) return;
        Managers.Instance.spaceshipMotor.AddThrustPower((float)MaxSpeed);
    }
}
