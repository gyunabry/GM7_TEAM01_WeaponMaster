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
    [SerializeField] private float afterActionDealy = 0.25f;

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
        }

        isAttacking = false;
    }


    #region 공격타입 코루틴

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
}
