using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonAtkSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonAtkSpeedSO", order = 1)]
public class MainCannonAtkDamageSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.MainCannonAtkDamage;
    public int AtkDamage;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
