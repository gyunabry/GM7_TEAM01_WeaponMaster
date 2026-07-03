using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // 몬스터가 가질 수 있는 패턴 목록을 받음
    private List<EnemyPatternData> enemyPattern;
    private EnemyController enemyController;
    private EnemyShooter enemyShooter;
    private PlayerController targetPlayer;

    [Header("공격 판정")]
    [SerializeField] private float attackCheckInterval = 0.1f;
    [SerializeField] private float playerHitRadius = 0.2f;
    [SerializeField] private float dashWindUpTime = 1f;
    [SerializeField] private float rangeWindUpTime = 1f;
    [SerializeField] private float afterActionDealy = 0.5f;

    private float attackTimer;  // 공격 쿨타임 타이머
    private float attackCheckTimer;
    private bool isAttacking;   // 중복 공격을 막기 위한 bool 값
    private bool hasHitDuringAction; // 중복 공격 판정을 막기 위한 bool 값

    private Coroutine attackRoutine;

    // 공격 딜레이
    private WaitForSeconds dashWindUpWait;
    private WaitForSeconds rangeWindUpWait;
    private WaitForSeconds afterActionWait;

    public float ContactDamage { get; private set; }

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        enemyShooter = GetComponent<EnemyShooter>();

        dashWindUpWait = new WaitForSeconds(dashWindUpTime);
        rangeWindUpWait = new WaitForSeconds(rangeWindUpTime);
        afterActionWait = new WaitForSeconds(afterActionDealy);
    }

    private void OnDisable()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        isAttacking = false;
        hasHitDuringAction = false;
        targetPlayer = null;

        if (enemyController != null)
        {
            enemyController.CanMove = true;
        }
    }

    private void Update()
    {
        // 패턴이 없거나, 타겟이 없거나, 이미 공격 중이라면 리턴
        if (enemyPattern == null || enemyPattern.Count == 0 || enemyController.target == null || isAttacking) return;

        attackTimer += Time.deltaTime;
        attackCheckTimer += Time.deltaTime;

        if (attackCheckTimer < attackCheckInterval) return;
        attackCheckTimer = 0f;

        // 첫 번째 패턴 조건을 검사
        EnemyPatternData currentPattern = enemyPattern[0];

        // 타겟까지의 거리와 패턴의 사거리를 비교해 공격 여부 판단
        Vector2 toTarget = enemyController.target.position - transform.position;
        float rangeSqr = currentPattern.triggerRange * currentPattern.triggerRange;

        if (toTarget.sqrMagnitude <= rangeSqr && attackTimer >= currentPattern.patternCooltime)
        {
            attackRoutine = StartCoroutine(ExecutePatternSequenceCo(currentPattern));
        }
    }

    public void Initialize(List<EnemyPatternData> patterns)
    {
        enemyPattern = patterns;
        attackTimer = 2f;
        attackCheckTimer = 0f;
        isAttacking = false;
        hasHitDuringAction = false;
        targetPlayer = null;
    }

    // 복합적인 패턴을 순차적으로 실행하는 코루틴
    private IEnumerator ExecutePatternSequenceCo(EnemyPatternData pattern)
    {
        isAttacking = true;
        attackTimer = 0f;

        // 패턴 안에 등록된 공격을 순서대로 실행
        foreach (EnemyAttackData actionData in pattern.attackSequence)
        {
            hasHitDuringAction = false;

            switch (actionData.attackType)
            {
                case AttackType.Dash:
                    yield return StartCoroutine(DashAttackCo(actionData));
                    break;

                case AttackType.Range:
                    yield return StartCoroutine(RangeAttackCo(actionData));
                    break;
            }

            yield return new WaitForSeconds(pattern.actionDelay);
        }

        isAttacking = false;
    }

    // 최적화 위해 별도의 함수를 작성
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    CollisionHandler(collision.gameObject);
    //}

    //private void OnCollisionEnterStay2D(Collision2D collision)
    //{
    //    CollisionHandler(collision.gameObject);
    //}

    //private void CollisionHandler(GameObject target)
    //{
    //    if (currentAttackData == null)
    //    {
    //        return;
    //    }

    //    // 공격 타입이 근접, 돌진인 공격만 충돌 허용
    //    if (currentAttackData.attackType == AttackType.Melee || currentAttackData.attackType == AttackType.Dash)
    //    {
    //        if (target.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
    //        {
    //            player.TakeDamage(currentAttackData.attackDamage);
    //        }
    //    }
    //}

    #region 공격타입 코루틴
    private IEnumerator MeleeAttackCo(EnemyAttackData data)
    {
        TryDamageTargetOnContact(data.attackDamage);

        yield return new WaitForSeconds(afterActionDealy);
    }

    private IEnumerator DashAttackCo(EnemyAttackData data)
    {
        if (enemyController.target == null)
        {
            yield break;
        }

        // 대쉬 전 대기 동작
        enemyController.CanMove = false;
        Vector3 targetLastPos = enemyController.target.position;
        yield return dashWindUpWait;

        Vector3 dashDir = targetLastPos - transform.position;
        dashDir.z = 0f;

        // 초근접이라면 대쉬 공격 취소
        if (dashDir.sqrMagnitude <= 0.001f)
        {
            enemyController.CanMove = true;
            yield break;
        }

        dashDir.Normalize();

        float dashSpeed = Mathf.Max(data.dashSpeed, 0.01f);
        float dashRange = Mathf.Max(data.dashRange, 0f);
        float dashDuration = dashRange / dashSpeed;
        float elapsed = 0;

        while (elapsed < dashDuration)
        {
            transform.position += dashDir * dashSpeed * Time.deltaTime;
            TryDamageTargetOnContact(data.attackDamage);

            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return afterActionWait;
        // 추적 재개
        enemyController.CanMove = true;
    }

    private IEnumerator RangeAttackCo(EnemyAttackData data)
    {
        enemyController.CanMove = false; // 원거리 공격 시 추적 일시정지

        yield return new WaitForSeconds(rangeWindUpTime);

        if (enemyShooter != null && enemyController.target != null)
        {
            enemyShooter.Fire(data, enemyController.target);
        }

        yield return new WaitForSeconds(afterActionDealy); // 발사 후 딜레이

        enemyController.CanMove = true;
    }
    #endregion

    // 테스트 후 삭제
    private void TryDamageTargetOnce(float damage)
    {
        if (hasHitDuringAction) return;
        if (enemyController.target == null) return;

        float totalRadius = enemyController.HitRadius + playerHitRadius;
        float totalRadiusSqr = totalRadius * totalRadius;

        Vector3 toTarget = enemyController.target.position - transform.position;

        // 타겟과의 거리가 사거리 안이라면
        if (toTarget.sqrMagnitude <= totalRadius)
        {
            PlayerController player = enemyController.target.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
                hasHitDuringAction = true;
            }
        }
    }

    private void TryDamageTargetOnContact(float damage)
    {
        if (enemyController == null || enemyController.target == null)
        {
            return;
        }

        PlayerController player = enemyController.target.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        float totalRadius = enemyController.HitRadius + playerHitRadius;
        float totalRadiusSqr = totalRadius * totalRadius;

        Vector3 toTarget = enemyController.target.position - transform.position;
        toTarget.z = 0f;

        if (toTarget.sqrMagnitude <= totalRadiusSqr)
        {
            player.TakeDamage(damage);
        }
    }

    // 테스트 후 삭제 예정
    #region 탄막 패턴
    //private void FireStraight(EnemyAttackData attackData)
    //{
    //    // 타겟 방향으로 발사
    //    Vector2 direction = (enemyController.target.position - transform.position).normalized;
    //    SpawnProjectile(attackData, direction);
    //}

    //private void FireCone(EnemyAttackData attackData)
    //{
    //    float angleRange = attackData.spreadAngle; // 데이터 상 발사각
    //    float startAngle = -angleRange * 0.5f; // 시작 각도
    //    float angleStep = angleRange / (attackData.projectileCount - 1); // 투사체 간 간격
        
    //    for (int i = 0; i < attackData.projectileCount; i++)
    //    {
    //        float angle = startAngle + angleStep * i;
    //        Vector2 direction = Quaternion.Euler(0, 0, angle) * transform.right;
    //        SpawnProjectile(attackData, direction);
    //    }
    //}

    //private void FireCircle(EnemyAttackData attackData)
    //{
    //    // 360도를 투사체 개수만큼 나누어 전방위로 발사
    //    float angleStep = 360f / attackData.projectileCount;
    //    float angle = 0f;

    //    for (int i = 0; i < attackData.projectileCount; i++)
    //    {
    //        float projectileDirXPosition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180);
    //        float projectileDirYPosition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180);

    //        Vector2 projectileVector = new Vector2(projectileDirXPosition, projectileDirYPosition);
    //        Vector2 projectileMoveDirection = (projectileVector - (Vector2)transform.position).normalized;

    //        SpawnProjectile(attackData, projectileMoveDirection);
    //        angle += angleStep;
    //    }
    //}

    //// 보스 전용 패턴
    //private void FireOrbit(EnemyAttackData attackData)
    //{
        
    //}

    //private void SpawnProjectile(EnemyAttackData attackData, Vector2 direction)
    //{
    //    EnemyBullet bullet = PoolManager.Instance.GetPool(attackData.projectilePrefab);
    //    bullet.InitBullet(attackData.attackDamage);

    //    // 투사체의 현재 위치를 몬스터의 위치로 설정
    //    bullet.transform.position = transform.position;
    //    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
    //    if (rb != null)
    //    {
    //        rb.linearVelocity = direction * attackData.projectileSpeed;
    //    }
    //}
    #endregion
}
