using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Cost
{
    public OreType oreType;
    public int amount;
}
[System.Serializable]
public struct LevelRecipe
{
    public float levelValue;
    public Cost[] oreType;
}
