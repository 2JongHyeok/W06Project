using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonBulletNumberSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonBulletNumberSO", order = 1)]
public class MainCannonBulletNumberSO : BaseForgeSO
{
    public int BulletNumber = 1;
    
    protected override ForgeId GetForgeId() => ForgeId.MainCannonBulletNumber;
    
    public override void Apply()
    {
        if (Managers.Instance?.weapon == null) return;
        for (int i = 0; i < Mathf.Max(1, BulletNumber); i++)
            Managers.Instance.weapon.UpgradeTurretBulletCount();
    }
}
