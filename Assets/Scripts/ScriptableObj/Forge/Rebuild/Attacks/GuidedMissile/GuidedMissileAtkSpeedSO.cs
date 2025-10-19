using UnityEngine;
[CreateAssetMenu(fileName = "GuidedMissileAtkSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/GuidedMissile/GuidedMissileAtkSpeedSO", order = 1)]
public class GuidedMissileAtkSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.GuidedMissileAtkSpeed;
    public int AtkSpeed;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
