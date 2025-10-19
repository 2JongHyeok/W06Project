using UnityEngine;
[CreateAssetMenu(fileName = "PlanetCoreMaxHpSO", menuName = "ScriptableObjects/Forge/Planet/PlanetCoreMaxHpSO", order = 1)]
public class PlanetCoreMaxHpSO : BaseForgeSO
{
    public int CoreMaxHp;
    
    protected override ForgeId GetForgeId() => ForgeId.PlanetCoreMaxHp;
    
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
