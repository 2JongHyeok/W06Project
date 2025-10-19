using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMiningSpeedSO", menuName = "ScriptableObjects/Forge/SpaceShip/Mining/SpaceShipMiningSpeedSO", order = 1)]
public class SpaceShipMiningSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMiningSpeed;
    public int MiningSpeed;
    public override void Apply()
    {
        if (Managers.Instance?.spaceshipWeapon != null)
            Managers.Instance.spaceshipWeapon.AddMiningAttackSpeed((float)MiningSpeed);
    }
}
