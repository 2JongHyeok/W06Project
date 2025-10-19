using UnityEngine;
[CreateAssetMenu(fileName = "PlanetShieldMaxHpSO", menuName = "ScriptableObjects/Forge/Planet/PlanetShieldMaxHpSO", order = 1)]
public class PlanetShieldMaxHpSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.PlanetShieldMaxHp;
    public int ShieldMaxHp;
    public override void Apply()
    {
        // TODO: 실드 시스템 연결
        Debug.Log("PlanetShieldMaxHpSO.Apply called (implement hook to shield system).");
    }
}
