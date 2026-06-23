using UnityEngine;

public class PoolPreLoader : MonoBehaviour
{
    [Header("생성할 프리팹")]
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private EnemyBullet enemyBulletPrefab;
    [SerializeField] private ExpGem expPrefab;
    [SerializeField] private HitText hitTextPrefab;
    // 필요시 여기에 추가

    [Header("초기 생성 수")]
    [SerializeField] private int enemyCount;
    [SerializeField] private int enemyBulletCount;
    [SerializeField] private int expCount = 100;
    [SerializeField] private int hitTextCount = 30;

    private void Start()
    {
        PoolManager.Instance.PreLoadPool(enemyPrefab, enemyCount);
        PoolManager.Instance.PreLoadPool(enemyBulletPrefab, enemyBulletCount);
        PoolManager.Instance.PreLoadPool(expPrefab, expCount);
        PoolManager.Instance.PreLoadPool(hitTextPrefab, hitTextCount);
    }
}