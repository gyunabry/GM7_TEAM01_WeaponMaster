using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("기본 설정")]
    [SerializeField] private float maxHp;
    [SerializeField] private float moveSpeed;
    private int contactDamage;

    [Header("시각 갱신 주기")]
    [SerializeField] private float visualUpdateInterval = 0.1f;

    [Header("충돌 판정")]
    [SerializeField] private float hitRadius = 0.3f;

    [Header("피격 연출")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("공간해싱 설정")]
    [SerializeField] private float separationRadius = 0.7f;
    [SerializeField] private float separationWeight = 1.2f;

    private EnemyData currentEnemy;
    private EnemyAttack enemyAttack;
    private EnemyVisualController visualController;

    private float currentHp;

    private static Transform cachedPlayerTarget;
    private Coroutine visualCoroutine;
    private WaitForSeconds visualWait;

    public int ContactDamage => contactDamage;
    public float HitRadius => hitRadius;
    public Transform target;
    public bool CanMove { get; set; } = true;
    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        visualController = GetComponent<EnemyVisualController>();
        if (visualController == null)
        {
            visualController = gameObject.AddComponent<EnemyVisualController>();
        }

        visualWait = new WaitForSeconds(visualUpdateInterval);
    }

    private void OnEnable()
    {
        CanMove = true;

        if (visualCoroutine == null)
        {
            visualCoroutine = StartCoroutine(VisualUpdateCo());
        }

        SpatialHashGrid.Instance?.Register(this);
    }

    private void OnDisable()
    {
        target = null;
        CanMove = true;

        if (visualCoroutine != null)
        {
            StopCoroutine(visualCoroutine);
            visualCoroutine = null;
        }

        visualController?.StopAllVisuals();

        SpatialHashGrid.Instance?.Unregister(this);
    }

    private void Start()
    {
        ConnectTargetOnce();
    }

    private void Update()
    {
        MoveToTarget();
    }

    public void Initialize(EnemyData data)
    {
        currentEnemy = data;

        gameObject.name = data.enemyName;

        maxHp = data.maxHp;
        moveSpeed = data.moveSpeed;
        contactDamage = data.contactDamage;
        currentHp = maxHp;

        CanMove = true;

        visualController?.SetupAnimator(data.runtimeAnimator);

        if (enemyAttack != null)
        {
            enemyAttack.Initialize(currentEnemy.enemyPattern);
        }

        if (visualCoroutine == null)
        {
            visualCoroutine = StartCoroutine(VisualUpdateCo());
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        cachedPlayerTarget = newTarget;
    }

    private void ConnectTargetOnce()
    {
        if (target != null) return;

        if (cachedPlayerTarget == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                cachedPlayerTarget = player.transform;
            }
        }

        SetTarget(cachedPlayerTarget);
    }

    private void MoveToTarget()
    {
        if (!CanMove || IsDead || target == null) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position;

        Vector2 chaseDir = targetPos - currentPos;
        if (chaseDir.sqrMagnitude <= 0.001f) return;

        Vector2 separation = Vector2.zero;

        if (SpatialHashGrid.Instance != null)
        {
            SpatialHashGrid.Instance.UpdateEnemyCell(this);
            separation = SpatialHashGrid.Instance.GetSeparationForce(this, separationRadius);
        }

        Vector2 finalDir = chaseDir.normalized + separation * separationWeight;

        if (finalDir.sqrMagnitude <= 0.0001f) return;

        transform.position += (Vector3)(finalDir.normalized * moveSpeed * Time.deltaTime);

        SpatialHashGrid.Instance?.UpdateEnemyCell(this);
    }

    public void TakeDamage(float damage, bool isCrit = false)
    {
        if (IsDead) return;

        currentHp -= damage;

        HitText hitText = PoolManager.Instance.GetPool<HitText>();
        hitText.ShowDamage(damage, transform.position, isCrit);

        visualController?.PlayHitFlash(flashMaterial);

        if (IsDead)
        {
            Die();
        }
    }

    public int ReturnTakeDamage(float damage)
    {
        if (currentHp < damage)
        {
            return (int)currentHp;
        }

        return (int)damage;
    }

    private void Die()
    {
        CanMove = false;

        currentEnemy.DropItem(transform.position);
        GameManager.Instance.AddKillCount();

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        visualController?.RestoreMaterial();

        PoolManager.Instance.ReturnPool(this);
    }

    private IEnumerator VisualUpdateCo()
    {
        while (!IsDead)
        {
            bool isMoving = CanMove && target != null && !IsDead;
            visualController?.UpdateVisual(target, isMoving);

            yield return visualWait;
        }

        visualCoroutine = null;
    }
}
