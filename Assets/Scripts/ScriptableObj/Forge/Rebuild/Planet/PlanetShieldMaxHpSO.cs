using UnityEngine;
[CreateAssetMenu(fileName = "PlanetShieldMaxHpSO", menuName = "ScriptableObjects/Forge/Planet/PlanetShieldMaxHpSO", order = 1)]
public class PlanetShieldMaxHpSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.PlanetShieldMaxHp;
    public int ShieldMaxHp;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
