using UnityEngine;
[CreateAssetMenu(fileName = "PlanetShieldRegenSpeedSO", menuName = "ScriptableObjects/Forge/Planet/PlanetShieldRegenSO", order = 1)]
public class PlanetShieldRegenSO : BaseForgeSO
{
    public float ShieldRegen;
    
    protected override ForgeId GetForgeId() => ForgeId.PlanetShieldRegenSpeed;
    
    public override void Apply()
    {
        // TODO: 실드 재생 시스템 연결
        Debug.Log("PlanetShieldRegenSO.Apply called (implement hook to shield system).");
    }
}
