using UnityEngine;

public abstract class DropItemBase : MonoBehaviour, ICollectable
{
    public abstract void Collect(PlayerController player);
    public abstract void Pull(Transform target, float pullSpeed);

    public abstract DropItemBase SpawnFromPool(Vector3 position);

    public abstract void ReturnToPool();
}
