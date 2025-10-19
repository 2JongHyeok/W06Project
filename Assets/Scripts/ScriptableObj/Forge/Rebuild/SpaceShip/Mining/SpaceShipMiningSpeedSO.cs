using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMiningSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Mining/SpaceShipMiningSpeedSO", order = 1)]
public class SpaceShipMiningSpeedSO : BaseForgeSO
{
    public float MiningSpeed;
    
    protected override ForgeId GetForgeId() => ForgeId.SpaceShipMiningSpeed;
    
    public override void Apply()
    {
        if (Managers.Instance?.spaceshipWeapon != null)
            Managers.Instance.spaceshipWeapon.AddMiningAttackSpeed(MiningSpeed);
    }
}
