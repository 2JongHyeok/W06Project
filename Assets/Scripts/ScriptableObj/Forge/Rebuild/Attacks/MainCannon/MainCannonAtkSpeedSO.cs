using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonAtkSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonAtkSpeedSO", order = 1)]
public class MainCannonAtkSpeedSO : BaseForgeSO
{
    public int AtkSpeed;
    
    protected override ForgeId GetForgeId() => ForgeId.MainCannonAtkSpeed;
    
    public override void Apply()
    {
        if (Managers.Instance?.weapon == null) return;
        Managers.Instance.weapon.AddAttackSpeed((float)AtkSpeed);
    }
}
