using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMiningRadiusSO", menuName = "ScriptableObjects/Forge/SpaceShip/Mining/SpaceShipMiningRadiusSO", order = 1)]
public class SpaceShipMiningRadiusSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMiningRadius;
    public int MiningRadius;
    public override void Apply()
    {
        if (Managers.Instance?.spaceshipWeapon != null)
            Managers.Instance.spaceshipWeapon.AddMiningRadius((float)MiningRadius);
    }
}
