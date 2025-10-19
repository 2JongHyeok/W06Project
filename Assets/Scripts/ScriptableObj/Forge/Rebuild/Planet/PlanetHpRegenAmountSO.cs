using UnityEngine;
[CreateAssetMenu(fileName = "PlanetHpRegenAmountSO", menuName = "ScriptableObjects/Forge/Planet/PlanetHpRegenAmountSO", order = 1)]
public class PlanetHpRegenAmountSO : BaseForgeSO
{
    public ForgeId ForgeId = ForgeId.PlanetHpRegenAmount;
    public float HpRegenAmount;
    public override void Apply()
    {
        throw new System.NotImplementedException();
    }
}
