using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonMoveSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonMoveSpeedSO", order = 1)]
public class MainCannonAtkSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.MainCannonAtkSpeed;
    public int AtkSpeed;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
