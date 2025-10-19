using UnityEngine;
[CreateAssetMenu(fileName = "PlanetShieldRegenSO", menuName = "ScriptableObjects/Forge/Planet/PlanetShieldRegenSO", order = 1)]
public class PlanetShieldRegenSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.PlanetShieldRegenSpeed;
    public float ShieldRegen;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
