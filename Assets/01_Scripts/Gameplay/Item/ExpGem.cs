using UnityEngine;

public class ExpGem : MonoBehaviour, ICollectable
{
    [SerializeField] private float expAmount = 10f;
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

    public void Collect(PlayerController player)
    {
        // 플레이어 경험치 지급 로직
        // SFX 재생
        // 오브젝트 풀로 반환
        ReturnToPool();
    }

    public void Pull(Transform target, float pullSpeed)
    {
        isPulled = true;
        pullTarget = target;
        currentPullSpeed = pullSpeed;
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(this);
    }
}
