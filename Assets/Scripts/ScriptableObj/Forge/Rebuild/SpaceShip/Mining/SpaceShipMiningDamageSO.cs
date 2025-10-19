using UnityEngine;
[CreateAssetMenu(fileName = "SpaceShipMiningDamageSO", menuName = "ScriptableObjects/Forge/SpaceShip/Mining/SpaceShipMiningDamageSO", order = 1)]
public class SpaceShipMiningDamageSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.SpaceShipMiningDamage;
    public int MiningDamage;
    public override void Apply()
    {
        if (Managers.Instance?.spaceshipWeapon != null)
            Managers.Instance.spaceshipWeapon.AddDamage(MiningDamage);
    }
}
