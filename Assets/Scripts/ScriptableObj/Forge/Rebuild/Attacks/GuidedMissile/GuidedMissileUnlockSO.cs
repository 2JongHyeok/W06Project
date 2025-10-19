using UnityEngine;
[CreateAssetMenu(fileName = "GuidedMissileUnlockSO", menuName = "ScriptableObjects/Forge/Attacks/GuidedMissile/GuidedMissileUnlockSO", order = 1)]
public class GuidedMissileUnlockSO : BaseForgeSO, IFirstActivation
{
    protected override ForgeId GetForgeId() => ForgeId.GuidedMissileUnlock;

    public override void Apply()
    {
        // 레벨 상승 시 추가 효과가 있으면 여기에
    }

    // 처음 해금 시 1회
    public void OnFirstUnlock()
    {
        Managers.Instance?.turretActivationManager?.AddMissileTurret();
    }
}
