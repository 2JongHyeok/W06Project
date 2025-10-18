using UnityEngine;
public class BaseUpgrade : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    public float baseValue;
    public LevelRecipe[] recipes;
}
