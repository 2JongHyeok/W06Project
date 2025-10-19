using UnityEngine;
[CreateAssetMenu(fileName = "GuidedMissileUnlockSO", menuName = "ScriptableObjects/Forge/Attacks/GuidedMissile/GuidedMissileUnlockSO", order = 1)]
public class GuidedMissileUnlockSO : BaseForgeSO, IFirstActivation
{
    public ForgeId ForgeId = ForgeId.GuidedMissileUnlock;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }

    public void OnFirstUnlock()
    {
        throw new System.NotImplementedException();
    }
}
