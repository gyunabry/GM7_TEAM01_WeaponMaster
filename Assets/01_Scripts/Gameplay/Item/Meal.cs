using UnityEngine;

public class Meal : DropItemBase, ICollectable
{
    [SerializeField] private float healAmount = 10f;
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
        DropItemBase spawnedItem = PoolManager.Instance.GetPool(this);
        spawnedItem.transform.position = position;

        return spawnedItem;
    }

    public override void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(this);
    }
}
