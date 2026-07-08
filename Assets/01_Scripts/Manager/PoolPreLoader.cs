using UnityEngine;

public class PoolPreLoader : MonoBehaviour
{
    [Header("생성할 프리팹")]
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private EnemyBullet enemyBulletPrefab;
    [SerializeField] private ExpGem expPrefab;
    [SerializeField] private Meal mealPrefab;
    [SerializeField] private HitText hitTextPrefab;
    [SerializeField] private WarningMarker warningPrefab;
    [SerializeField] private VFXAdd attackVfxPrefab;
    // 필요시 여기에 추가

    [Header("초기 생성 수")]
    [SerializeField] private int enemyCount;
    [SerializeField] private int enemyBulletCount;
    [SerializeField] private int expCount = 100;
    [SerializeField] private int mealCount = 20;
    [SerializeField] private int hitTextCount = 30;
    [SerializeField] private int warningCount = 30;
    [SerializeField] private int attackVfxCount = 30;

    private void Start()
    {
        PoolManager.Instance.PreLoadPool(enemyPrefab, enemyCount);
        PoolManager.Instance.PreLoadPool(enemyBulletPrefab, enemyBulletCount);
        PoolManager.Instance.PreLoadPool(expPrefab, expCount);
        PoolManager.Instance.PreLoadPool(mealPrefab, mealCount);
        PoolManager.Instance.PreLoadPool(hitTextPrefab, hitTextCount);
        PoolManager.Instance.PreLoadPool(warningPrefab, warningCount);
        PoolManager.Instance.PreLoadPool(attackVfxPrefab, attackVfxCount);
    }
}