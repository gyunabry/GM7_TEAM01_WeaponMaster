using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    // 몬스터가 가질 수 있는 패턴 목록을 받음
    private List<EnemyPatternData> enemyPattern;
    private EnemyController enemyController;
    private NavMeshAgent agent;

    private float attackTimer;  // 공격 쿨타임 타이머
    private bool isAttacking;   // 중복 공격을 막기 위한 bool 값

    // 현재 실행 중인 단일 공격 데이터
    private EnemyAttackData currentAttackData;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // 패턴이 없거나, 타겟이 없거나, 이미 공격 중이라면 리턴
        if (enemyPattern == null || enemyPattern.Count == 0 || enemyController.target == null || isAttacking) return;

        attackTimer += Time.deltaTime;

        // 첫 번째 패턴 조건을 검사
        EnemyPatternData currentPattern = enemyPattern[0];
        float distanceToTarget = Vector2.Distance(transform.position, enemyController.target.position);

        if (distanceToTarget <= currentPattern.triggerRange && attackTimer >= currentPattern.patternCooltime)
        {
            StartCoroutine(ExecutePatternSequenceCo(currentPattern));
        }
    }

    public void Initialize(List<EnemyPatternData> patterns)
    {
        enemyPattern = patterns;

        attackTimer = 2f;
        isAttacking = false;
    }

    // 복합적인 패턴을 순차적으로 실행하는 코루틴
    private IEnumerator ExecutePatternSequenceCo(EnemyPatternData pattern)
    {
        isAttacking = true;
        attackTimer = 0f;

        // 패턴 안에 등록된 공격을 순서대로 실행
        foreach (EnemyAttackData actionData in pattern.attackSequence)
        {
            currentAttackData = actionData;

            yield return StartCoroutine(ExecuteSingleActionCo(actionData));

            yield return new WaitForSeconds(pattern.actionDelay);
        }

        // currentAttackData = null;
        isAttacking = false;
    }

    // 복합패턴 내 단일 공격을 실행하는 코루틴
    private IEnumerator ExecuteSingleActionCo(EnemyAttackData actionData)
    {
        switch (actionData.attackType)
        {
            case AttackType.Melee:
                yield return StartCoroutine(MeleeAttackCo());
                break;
            case AttackType.Dash:
                yield return StartCoroutine(DashAttackCo(actionData));
                break;
            case AttackType.Range:
                yield return StartCoroutine(RangeAttackCo(actionData));
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionHandler(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CollisionHandler(collision.gameObject);
    }

    private void CollisionHandler(GameObject target)
    {
        if (currentAttackData == null)
        {
            return;
        }

        // 공격 타입이 근접, 돌진인 공격만 충돌 허용
        if (currentAttackData.attackType == AttackType.Melee || currentAttackData.attackType == AttackType.Dash)
        {
            if (target.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
            {
                player.TakeDamage(currentAttackData.attackDamage);
            }
        }
    }

    #region 공격타입 코루틴
    private IEnumerator MeleeAttackCo()
    {
        // 코루틴이 바로 종료되어 공격을 하지 못 하는 현상 방지
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator DashAttackCo(EnemyAttackData data)
    {
        // 대쉬 전 대기 동작
        agent.isStopped = true;
        Vector3 lastPosition = enemyController.target.position; // 대쉬 직전 타겟 위치 저장
        Vector3 dashDirection = (lastPosition - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);

        // 대쉬 실행
        float dashSpeed = data.dashSpeed;
        float dashRange = data.dashRange;
        float dashDuration = dashRange / dashSpeed;
        float time = 0;

        while (time < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        // 추적 재개
        agent.isStopped = false;
    }

    private IEnumerator RangeAttackCo(EnemyAttackData data)
    {
        agent.isStopped = true; // 원거리 공격 시 추적 일시정지
        yield return new WaitForSeconds(1f); // 발사 전 딜레이

        switch (data.bulletPattern)
        {
            case BulletPattern.Straight:
                FireStraight(data);
                break;
            case BulletPattern.Cone:
                FireCone(data); 
                break;
            case BulletPattern.Circle:
                FireCircle(data);
                break;
            case BulletPattern.Orbit:
                FireOrbit(data);
                break;
        }

        yield return new WaitForSeconds(0.5f); // 발사 후 딜레이
        agent.isStopped = false;
    }
    #endregion

    #region 탄막 패턴
    private void FireStraight(EnemyAttackData attackData)
    {
        // 타겟 방향으로 발사
        Vector2 direction = (enemyController.target.position - transform.position).normalized;
        SpawnProjectile(attackData, direction);
    }

    private void FireCone(EnemyAttackData attackData)
    {
        float angleRange = attackData.spreadAngle; // 데이터 상 발사각
        float startAngle = -angleRange * 0.5f; // 시작 각도
        float angleStep = angleRange / (attackData.projectileCount - 1); // 투사체 간 간격
        
        for (int i = 0; i < attackData.projectileCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.right;
            SpawnProjectile(attackData, direction);
        }
    }

    private void FireCircle(EnemyAttackData attackData)
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

            SpawnProjectile(attackData, projectileMoveDirection);
            angle += angleStep;
        }
    }

    // 보스 전용 패턴
    private void FireOrbit(EnemyAttackData attackData)
    {
        
    }

    private void SpawnProjectile(EnemyAttackData attackData, Vector2 direction)
    {
        EnemyBullet bullet = PoolManager.Instance.GetPool(attackData.projectilePrefab);
        bullet.InitBullet(attackData.attackDamage);

        // 투사체의 현재 위치를 몬스터의 위치로 설정
        bullet.transform.position = transform.position;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * attackData.projectileSpeed;
        }
    }
    #endregion
}
