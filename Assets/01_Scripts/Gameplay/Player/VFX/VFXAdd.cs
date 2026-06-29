using UnityEngine;

public class VFXAdd : MonoBehaviour
{
    public void ReturnToPool()
    {
        CancelInvoke(nameof(ReturnToPool));
        PoolManager.Instance.ReturnPool(this);
    }
}
