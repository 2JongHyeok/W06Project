using System;
using UnityEngine;
public enum EnemyType
{
    Ranger,
    Kamikaze
}

public abstract class EnemyBaseSO : ScriptableObject
{
    public EnemyType enemyType;
    public int enemyHP;
    public float enemySpeed;

    public abstract void PerformAttack(Enemy enemy);
}
