using UnityEngine;
[CreateAssetMenu(fileName = "MainCannonMoveSpeedSO", menuName = "ScriptableObjects/Forge/Attacks/MainCannon/MainCannonMoveSpeedSO", order = 1)]
public class MainCannonMoveSpeedSO : BaseForgeSO
{
    public int MoveSpeed;
    
    protected override ForgeId GetForgeId() => ForgeId.MainCannonMoveSpeed;
    
    public override void Apply()
    {
        if (Managers.Instance?.weapon == null) return;
        Managers.Instance.weapon.AddCannonSpeed((float)MoveSpeed);
    }
}
