using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonMoveSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonMoveSpeedSO", order = 1)]
public class MainCannonMoveSpeedSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.MainCannonMoveSpeed;
    public int MoveSpeed;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
