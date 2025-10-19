using UnityEngine;
[CreateAssetMenu(fileName = "GuidedMissileAtkSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/GuidedMissile/GuidedMissileAtkSpeedSO", order = 1)]
public class GuidedMissileAtkDamageSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.GuidedMissileAtkDamage;
    public int AtkDamage;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
