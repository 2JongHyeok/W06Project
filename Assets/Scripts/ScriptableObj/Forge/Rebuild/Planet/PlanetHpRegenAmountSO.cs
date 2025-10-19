using UnityEngine;
[CreateAssetMenu(fileName = "PlanetHpRegenAmountSO", menuName = "ScriptableObjects/Forge/Planet/PlanetHpRegenAmountSO", order = 1)]
public class PlanetHpRegenAmountSO : BaseForgeSO
{
    public float HpRegenAmount;
    
    protected override ForgeId GetForgeId() => ForgeId.PlanetHpRegenAmount;
    
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
