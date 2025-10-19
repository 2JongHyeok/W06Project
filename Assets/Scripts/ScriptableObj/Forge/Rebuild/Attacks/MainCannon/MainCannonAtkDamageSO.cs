using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonAtkDamageSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonAtkDamageSO", order = 1)]
public class MainCannonAtkDamageSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.MainCannonAtkDamage;
    public int AtkDamage;

    public override void Apply()
    {
        if (Managers.Instance?.weapon == null) return;
        Managers.Instance.weapon.AddDamage(AtkDamage);
    }
}
