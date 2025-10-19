using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonMoveSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonMoveSpeedSO", order = 1)]
public class MainCannonBulletNumberSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.MainCannonBulletNumber;
    public int BulletNumber;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
