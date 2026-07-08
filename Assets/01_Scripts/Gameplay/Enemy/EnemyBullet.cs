using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("충돌 판정")]
    [SerializeField] private float hitRadius = 0.25f;
    [SerializeField] private float playerHitRadius = 0.5f;

    private float bulletDamage;
    private float speed;
    private float lifeTimer;
    private Vector3 direction;
    private Transform target;
    private bool initialized;

    private bool isHoming;
    private float homingTurnSpeed;
    private float homingTimer;

    public float AoeLifetime { get; set; } = 4f;

    private void OnEnable()
    {
        lifeTimer = AoeLifetime;
    }

    public void InitBullet(float damage, Vector2 moveDirection, float moveSpeed, Transform targetTransform, float lifetime)
    {
        bulletDamage = damage;
        direction = moveDirection.normalized;
        speed = moveSpeed;
        target = targetTransform;
        lifeTimer = lifetime;
        isHoming = false;
        homingTurnSpeed = 0f;
        homingTimer = 0f;
        initialized = true;
    }

    public void InitHomingBullet(float damage, Vector2 moveDirection, float moveSpeed, Transform targetTransform, float lifetime, float turnSpeed, float homingDuration)
    {
        InitBullet(damage, moveDirection, moveSpeed, targetTransform, lifetime);

        if (direction.sqrMagnitude <= 0.001f && target != null)
        {
            direction = (target.position - transform.position).normalized;
        }

        isHoming = true;
        homingTurnSpeed = Mathf.Max(0f, turnSpeed);
        homingTimer = homingDuration > 0f ? homingDuration : lifetime;
    }

    private void Update()
    {
        if (!initialized) return;

        if (speed > 0f)
        {
            UpdateHomingDirection();
            transform.position += direction * speed * Time.deltaTime;
        }

        CheckHitTarget();

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    private void UpdateHomingDirection()
    {
        if (!isHoming || target == null || homingTimer <= 0f)
        {
            return;
        }

        Vector3 targetDirection = target.position - transform.position;
        targetDirection.z = 0f;

        if (targetDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        float maxRadiansDelta = homingTurnSpeed * Mathf.Deg2Rad * Time.deltaTime;
        direction = Vector3.RotateTowards(direction, targetDirection.normalized, maxRadiansDelta, 0f).normalized;
        homingTimer -= Time.deltaTime;
    }

    private void CheckHitTarget()
    {
        if (target == null) return;

        float totalRadius = hitRadius + playerHitRadius;
        float totalRadiusSqr = totalRadius * totalRadius;

        Vector3 toTarget = target.position - transform.position;

        if (toTarget.sqrMagnitude <= totalRadiusSqr)
        {
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(bulletDamage);
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        initialized = false;
        isHoming = false;
        target = null;
        PoolManager.Instance.ReturnPool(this);
    }
}