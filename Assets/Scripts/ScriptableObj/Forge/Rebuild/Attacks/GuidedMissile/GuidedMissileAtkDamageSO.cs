using UnityEngine;
[CreateAssetMenu(fileName = "GuidedMissileAtkDamageSO", menuName = "ScriptableObjects/Forge/Attacks/GuidedMissile/GuidedMissileAtkDamageSO", order = 1)]
public class GuidedMissileAtkDamageSO : BaseForgeSO
{
    public int AtkDamage;
    
    protected override ForgeId GetForgeId() => ForgeId.GuidedMissileAtkDamage;
    
    public override void Apply()
    {
        if (Managers.Instance?.turretActivationManager == null) return;
        Managers.Instance.turretActivationManager.AddMissileDamage((float)AtkDamage);
    }
}
