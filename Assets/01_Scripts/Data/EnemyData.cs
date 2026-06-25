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

    [Header("드랍 아이템")]
    public List<DropItemBase> dropItem;

    [Header("애니메이션 설정")]
    public RuntimeAnimatorController runtimeAnimator;

    // 추후 드랍 확률 반영
    public void DropItem(Vector3 dropPosition)
    {
        foreach (DropItemBase item in dropItem)
        {
            if (item == null) continue;
            item.SpawnFromPool(dropPosition);
        }
    }
}