using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMoveRotationSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Move/SpaceShipMoveRotationSpeedSO", order = 1)]
public class SpaceShipMiningDamageSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMiningDamage;
    public int MiningDamage;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
