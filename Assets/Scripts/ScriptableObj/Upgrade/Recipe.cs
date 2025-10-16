using UnityEngine;
[System.Serializable]
public struct OreData
{
    public OreType oreType;
    public int amount;
}
[System.Serializable]
public struct Recipe
{
    public OreData[] oreType;
}
