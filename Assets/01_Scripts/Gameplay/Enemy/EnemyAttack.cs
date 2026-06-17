using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyAttackData attackData;
    private EnemyController enemyController;
    private NavMeshAgent agent;

    private float attackTimer;  // 공격 쿨타임 타이머
    private bool isAttacking;   // 중복 공격을 막기 위한 bool 값

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
    }

    // 임시 초기화
    private void Start()
    {
        Initialize(attackData);
    }

    public void Initialize(EnemyAttackData data)
    {
        attackData = data;
        attackTimer = data.attackCooltime;
    }

    private void Update()
    {
        if (attackData == null || enemyController.target == null || isAttacking) return;

        attackTimer += Time.deltaTime;

        float currentAttackRange = (attackData.attackType == AttackType.Dash) ? attackData.dashRange : 4f;
        float distanceToTarget = Vector2.Distance(transform.position, enemyController.target.position);

        if (distanceToTarget <= currentAttackRange && attackTimer >= attackData.attackCooltime)
        {
            ExecuteAttack();
        }
    }

    // AttackType에 따라 공격 방식 스위칭
    private void ExecuteAttack()
    {
        isAttacking = true;
        attackTimer = 0f;

        switch (attackData.attackType)
        {
            case AttackType.Melee:
                StartCoroutine(MeleeAttackCo());
                break;
            case AttackType.Dash:
                StartCoroutine(DashAttackCo());
                break;
            case AttackType.Range:
                StartCoroutine(RangeAttackCo());
                break;
        }
    }

    #region 공격타입 코루틴
    private IEnumerator MeleeAttackCo()
    {
        yield return null;
    }

    private IEnumerator DashAttackCo()
    {
        // 대쉬 전 대기 동작
        agent.isStopped = true;
        Vector3 lastPosition = enemyController.target.position; // 대쉬 직전 타겟 위치 저장
        Vector3 dashDirection = (lastPosition - transform.position).normalized;
        yield return new WaitForSeconds(0.3f);

        // 대쉬 실행
        float dashSpeed = 10f;
        float dashDuration = 0.2f;
        float time = 0;

        while (time < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // 추적 재개
        agent.isStopped = false;
        isAttacking = false;
    }

    private IEnumerator RangeAttackCo()
    {
        agent.isStopped = true; // 원거리 공격 시 추적 일시정지

        switch (attackData.bulletPattern)
        {
            case BulletPattern.Straight:
                FireStraight();
                break;
            case BulletPattern.Circle:
                FireCircle();
                break;
                // TODO: 원뿔, 궤도형 공격 추가 구현
        }

        yield return new WaitForSeconds(0.5f); // 발사 후 딜레이

        agent.isStopped = false;
        isAttacking = false;
    }
    #endregion

    #region 탄막 패턴
    private void FireStraight()
    {
        // 타겟 방향으로 발사
        Vector2 direction = (enemyController.target.position - transform.position).normalized;
        SpawnProjectile(direction);
    }

    private void FireCircle()
    {
        // 360도를 투사체 개수만큼 나누어 전방위로 발사
        float angleStep = 360f / attackData.projectileCount;
        float angle = 0f;

        for (int i = 0; i < attackData.projectileCount; i++)
        {
            float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
            float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

            Vector2 projectileVector = new Vector2(projectileDirXPosition, projectileDirYPosition);
            Vector2 projectileMoveDirection = (projectileVector - (Vector2)transform.position).normalized;

            SpawnProjectile(projectileMoveDirection);
            angle += angleStep;
        }
    }

    private void SpawnProjectile(Vector2 direction)
    {
        // TODO: 오브젝트 풀링 적용
        GameObject projectile = Instantiate(attackData.projectilePrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * attackData.projectileSpeed;
        }
    }
    #endregion
}
