using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MyTileData
{
    public TileBase tile;       // 원본 타일
    public int maxHP = 3;       // 최대 체력
    [HideInInspector] public int currentHP;
}