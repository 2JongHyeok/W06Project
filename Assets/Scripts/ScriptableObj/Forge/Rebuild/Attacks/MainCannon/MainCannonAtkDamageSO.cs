using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonAtkDamageSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonAtkDamageSO", order = 1)]
public class MainCannonAtkDamageSO : BaseForgeSO
{
    public float AtkDamage;

    protected override ForgeId GetForgeId() => ForgeId.MainCannonAtkDamage;

    public override void Apply()
    {
        if (Managers.Instance?.weapon == null) return;
        Managers.Instance.weapon.AddDamage((int)AtkDamage);
    }
}
