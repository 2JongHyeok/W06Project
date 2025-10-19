using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonAtkSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonAtkSpeedSO", order = 1)]
public class MainCannonAtkSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.MainCannonAtkSpeed;
    public int AtkSpeed;
    public override void Apply()
    {
        if (Managers.Instance?.weapon == null) return;
        Managers.Instance.weapon.AddAttackSpeed((float)AtkSpeed);
    }
}
