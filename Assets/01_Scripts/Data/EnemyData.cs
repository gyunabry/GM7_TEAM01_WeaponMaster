using System;
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
    public int contactDamage;

    [Header("공격 패턴")]
    public List<EnemyPatternData> enemyPattern;

    [Header("드랍 아이템")]
    [SerializeField] private List<DropItemEntry> dropItems;

    [Header("애니메이션")]
    public RuntimeAnimatorController runtimeAnimator;

    public void DropItem(Vector3 dropPosition)
    {
        if (dropItems == null) return;

        foreach (DropItemEntry dropItem in dropItems)
        {
            if (dropItem == null || dropItem.item == null) continue;

            if (UnityEngine.Random.value <= dropItem.dropChance)
            {
                dropItem.item.SpawnFromPool(dropPosition);
            }
        }
    }
}

[Serializable]
public class DropItemEntry
{
    public DropItemBase item;

    [Range(0f, 1f)]
    public float dropChance = 1f;
}
