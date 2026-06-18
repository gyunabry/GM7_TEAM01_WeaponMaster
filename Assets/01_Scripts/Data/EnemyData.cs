using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "GamePlay/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string id;
    public string enemyName;
    public int maxHp;
    public int armor;
    public float moveSpeed;

    [Header("공격 패턴")]
    public List<EnemyPatternData> enemyPattern;
}