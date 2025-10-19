using UnityEngine;
[CreateAssetMenu(fileName = "GuidedMissileAtkDamageSO", menuName = "ScriptableObjects/Forge/Attacks/GuidedMissile/GuidedMissileAtkDamageSO", order = 1)]
public class GuidedMissileAtkDamageSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.GuidedMissileAtkDamage;
    public int AtkDamage;
    public override void Apply()
    {
        if (Managers.Instance?.turretActivationManager == null) return;
        Managers.Instance.turretActivationManager.AddMissileDamage((float)AtkDamage);
    }
}
