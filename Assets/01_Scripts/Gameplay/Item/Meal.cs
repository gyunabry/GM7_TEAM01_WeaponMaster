using UnityEngine;

public class Meal : DropItemBase, ICollectable
{
    [SerializeField] private float healAmount = 5f;
    private bool isPulled = false;
    private Transform pullTarget;
    private float currentPullSpeed;

    private void Update()
    {
        if (isPulled && pullTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, pullTarget.position, currentPullSpeed * Time.deltaTime);
        }
    }

    public override void Collect(PlayerController player)
    {
        // 플레이어 회복 로직
        player.RestoreHp(healAmount);
        // SFX 재생
        // 오브젝트 풀로 반환
        ReturnToPool();
    }

    public override void Pull(Transform target, float pullSpeed)
    {
        isPulled = true;
        pullTarget = target;
        currentPullSpeed = pullSpeed;
    }

    public override DropItemBase SpawnFromPool(Vector3 position)
    {
        Meal spawnedItem = PoolManager.Instance.GetPool(this);
        spawnedItem.transform.position = position;
        spawnedItem.isPulled = false;
        spawnedItem.pullTarget = null;
        spawnedItem.currentPullSpeed = 0f;

        return spawnedItem;
    }

    public override void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(this);
    }
}