using UnityEngine;
[CreateAssetMenu(fileName = "PlanetCoreMaxHpSO", menuName = "ScriptableObjects/Forge/Planet/PlanetCoreMaxHpSO", order = 1)]
public class PlanetCoreMaxHpSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.PlanetCoreMaxHp;
    public int CoreMaxHp;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
