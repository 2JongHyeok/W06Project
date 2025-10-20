using UnityEngine;
[CreateAssetMenu(fileName = "PlanetHpRegenAmountSO", menuName = "ScriptableObjects/Forge/Planet/PlanetHpRegenAmountSO", order = 1)]
public class PlanetHpRegenAmountSO : BaseForgeSO
{
    public float HpRegenAmount;
    
    protected override ForgeId GetForgeId() => ForgeId.PlanetHpRegenAmount;
    
    public override void Apply()
    {
        if (Managers.Instance != null)
        {
            Managers.Instance.HealCoreHP((int)HpRegenAmount);
        }
        else
        {
            Debug.LogWarning("PlanetHpRegenAmountSO.Apply: Managers.Instance is null");
        }
    }
}
