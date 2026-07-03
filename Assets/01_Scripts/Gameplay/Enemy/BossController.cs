using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
- 보스 몬스터는 개체마다 크기가 다를 수 있기 때문에 직접 프리팹을 만들어 스폰하기 
- Start 함수에서 BossData 초기화
*/

public class BossController : MonoBehaviour, IDamageable
{
    public enum BossState {            
        Chasing,        // 추적 상태
        ActionRunning,  // 공격 상태
        PhaseTransition, // 페이즈 전환 상태
        Dead
    }

    [Header("보스 데이터")]
    [SerializeField] private BossData bossData;
    private float maxHp;

    [Header("현재 상태")]
    [SerializeField] private BossState currentState;
    [SerializeField] private float currentHp;
    [SerializeField] private int currentPhaseIndex = 0;
    private List<EnemyPatternData> currentPatterns;

    [Header("충돌 판정")]
    [SerializeField] private float hitRadius = 1.0f;
    [SerializeField] private float playerHitRadius = 0.5f;

    [Header("공격 설정")]
    [SerializeField] private float patternCheckInterval = 0.15f;
    [SerializeField] private float patternInterval = 2f;
    [SerializeField] private float dashWindUpTime = 0.5f;
    [SerializeField] private float rangeWindUpTime = 0.2f;
    [SerializeField] private float afterActionDelay = 0.2f;

    [Header("보스 이벤트")]
    [SerializeField] private VoidEventChannel bossDeadEvent;
    [SerializeField] private HpEventChannel bossDamagedEvent;

    [Header("컴포넌트 참조")]
    private Transform target;
    private PlayerController targetPlayer;
    private EnemyShooter bossShooter;
    private SpriteRenderer sr;
    private EnemyAnimationController animationController;

    private Collider2D[] bossColliders;

    private static Transform cachedPlayerTarget;

    private Coroutine bossLoopRoutine;
    private Coroutine visualRoutine;
    private Coroutine attackRoutine;

    private WaitForSeconds patternCheckWait;
    private WaitForSeconds visualWait;

    private float patternTimer;
    private int patternIndex;
    private bool canMove = true;
    private bool hasHitDuringAction;
    private bool isDeadHandled;

    public string BossName => bossData != null ? bossData.bossName : string.Empty;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;
    public float HitRadius => hitRadius;
    public Transform Target => target;

    public bool IsDead => currentHp <= 0 || currentState == BossState.Dead;

    private void Awake()
    {
        bossShooter = GetComponent<EnemyShooter>();
        animationController = GetComponent<EnemyAnimationController>();
        sr = GetComponentInChildren<SpriteRenderer>();

        bossColliders = GetComponentsInChildren<Collider2D>();

        patternCheckWait = new WaitForSeconds(patternCheckInterval);
        visualWait = new WaitForSeconds(0.1f);
    }

    private void Start()
    {
        InitializeBoss();

        ConnectTargetOnce();

        if (bossLoopRoutine == null)
        {
            bossLoopRoutine = StartCoroutine(BossLoopCo());
        }

        if (visualRoutine == null)
        {
            visualRoutine = StartCoroutine(VisualUpdateCo());
        }
    }

    private void Update()
    {
        MoveToTarget();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        cachedPlayerTarget = newTarget;
        targetPlayer = newTarget != null ? newTarget.GetComponent<PlayerController>() : null;
    }

    public void Initialize(BossData newBossData, Transform newTarget)
    {
        bossData = newBossData;
        SetTarget(newTarget);
        InitializeBoss();
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

        target = cachedPlayerTarget;
        targetPlayer = target != null ? target.GetComponent<PlayerController>() : null;
    }

    private void InitializeBoss()
    {
        if (bossData == null) return;

        if (animationController != null)
        {
            animationController.SetupAnimator(bossData.runtimeAnimator);
        }

        maxHp = bossData.maxHp;
        currentHp = maxHp;

        // 1페이즈 세팅
        currentPhaseIndex = 0;
        currentPatterns = bossData.phases != null && bossData.phases.Count > 0
            ? bossData.phases[currentPhaseIndex].phasePatterns
            : null;

        patternTimer = 0f;
        patternIndex = 0;
        canMove = true;
        isDeadHandled = false;

        // 현재 상태를 추적 상태로 전환
        currentState = BossState.Chasing;
    }

    private void MoveToTarget()
    {
        if (!canMove || IsDead || currentState != BossState.Chasing || target == null)
        {
            return;
        }

        Vector3 direction = target.position - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        transform.position += direction.normalized * bossData.moveSpeed * Time.deltaTime;
    }

    // 이동과 패턴 실행 메인 루프를 관리하는 코루틴
    private IEnumerator BossLoopCo()
    {
        while (!IsDead)
        {
            ConnectTargetOnce();

            if (target == null || currentState != BossState.Chasing)
            {
                yield return patternCheckWait;
                continue;
            }

            patternTimer += patternCheckInterval;

            if (patternTimer >= patternInterval)
            {
                SelectAndExecutePattern();
            }

            yield return patternCheckWait;
        }

        bossLoopRoutine = null;
    }

    private IEnumerator VisualUpdateCo()
    {
        while (!IsDead)
        {
            FlipSprite();
            UpdateAnimation();

            yield return visualWait;
        }

        visualRoutine = null;
    }

    // 패턴 데이터의 공격 시퀀스를 순회하며 실행
    private void SelectAndExecutePattern()
    {
        if (currentPatterns == null || currentPatterns.Count == 0) return;

        // 순차적으로 패턴 실행 및 순회
        EnemyPatternData pattern = currentPatterns[patternIndex];
        patternIndex = (patternIndex + 1) % currentPatterns.Count;

        // 상태 전환 및 코루틴 실행
        currentState = BossState.ActionRunning;
        patternTimer = 0f;

        attackRoutine = StartCoroutine(ExecuteBossPatternCo(pattern));
    }

    private IEnumerator ExecuteBossPatternCo(EnemyPatternData pattern)
    {
        foreach (EnemyAttackData attackData in pattern.attackSequence)
        {
            hasHitDuringAction = false;

            yield return StartCoroutine(HandleAttackDataCo(attackData));
            yield return new WaitForSeconds(pattern.actionDelay);
        }

        // 패턴 종료 후 다시 추적 상태로 전환
        if (!IsDead && currentState == BossState.ActionRunning)
        {
            currentState = BossState.Chasing;
            canMove = true;
        }

        attackRoutine = null;
    }

    private IEnumerator HandleAttackDataCo(EnemyAttackData attackData)
    {
        switch (attackData.attackType)
        {
            case AttackType.Melee:
                animationController?.TriggerAttack();
                yield return StartCoroutine(BossMeleeCo(attackData));
                break;

            case AttackType.Dash:
                yield return StartCoroutine(BossDashCo(attackData));
                break;

            case AttackType.Range:
                yield return StartCoroutine(ExecuteRangeAttack(attackData));
                break;
        }
    }

    private IEnumerator BossMeleeCo(EnemyAttackData attackData)
    {
        TryDamageTargetOnContact(attackData.attackDamage);

        yield return new WaitForSeconds(afterActionDelay);
    }

    private IEnumerator BossDashCo(EnemyAttackData attackData)
    {
        if (target == null)
        {
            currentState = BossState.Chasing;
            yield break;
        }

        // 대쉬 전 대기 동작
        canMove = false;

        Vector3 dashDirection = target.position - transform.position;
        dashDirection.z = 0f;

        if (dashDirection.sqrMagnitude <= 0.001f)
        {
            canMove = true;
            yield break;
        }

        dashDirection.Normalize();

        if (animationController != null)
        {
            animationController.TriggerAttack();
        }

        yield return new WaitForSeconds(dashWindUpTime);

        // 대쉬 실행
        float dashSpeed = Mathf.Max(attackData.dashSpeed, 0.01f);
        float dashRange = Mathf.Max(attackData.dashRange, 0f);
        float dashDuration = dashRange / dashSpeed;
        float elpased = 0f;

        while (elpased < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;

            TryDamageTargetOnce(attackData.attackDamage);

            elpased += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(afterActionDelay);

        // 추적 재개
        canMove = true;
    }

    private IEnumerator ExecuteRangeAttack(EnemyAttackData data)
    {
        canMove = false; // 원거리 공격 시 추적 일시정지

        if (animationController != null)
        {
            animationController.TriggerAttack();
        }

        yield return new WaitForSeconds(rangeWindUpTime); // 발사 전 딜레이

        if (bossShooter != null && target != null)
        {
            bossShooter.Fire(data, target);
        }

        yield return new WaitForSeconds(afterActionDelay); // 발사 후 딜레이
        canMove = true;
    }

    #region 공격 판정
    private void TryDamageTargetOnce(float damage)
    {
        if (hasHitDuringAction || target == null)
        {
            return;
        }

        if (targetPlayer == null)
        {
            targetPlayer = target.GetComponent<PlayerController>();
        }

        if (targetPlayer == null)
        {
            return;
        }

        float totalRadius = hitRadius + playerHitRadius;
        float totalRadiusSqr = totalRadius * totalRadius;

        Vector3 toTarget = target.position - transform.position;
        toTarget.z = 0f;

        if (toTarget.sqrMagnitude <= totalRadiusSqr)
        {
            targetPlayer.TakeDamage(damage);
            hasHitDuringAction = true;
        }
    }

    private void TryDamageTargetOnContact(float damage)
    {
        if (target == null)
        {
            return;
        }

        PlayerController player = target.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }

        float totalRadius = hitRadius + playerHitRadius;
        float totalRadiusSqr = totalRadius * totalRadius;

        Vector3 toTarget = target.position - transform.position;
        toTarget.z = 0f;

        if (toTarget.sqrMagnitude <= totalRadiusSqr)
        {
            player.TakeDamage(damage);
        }
    }
    #endregion

    public void TakeDamage(float damage, bool isCrit = false)
    {
        // 페이즈 전환 중이라면 데미지를 받지 않도록 설정
        if (IsDead || currentState == BossState.PhaseTransition) return;

        // 체력 감소 및 데미지 텍스트 표시
        currentHp = Mathf.Max(0f, currentHp - damage);

        HitText hitText = PoolManager.Instance.GetPool<HitText>();
        hitText.ShowDamage(damage, transform.position, isCrit, false);

        // 인게임 UI 내 보스 체력 UI 갱신
        bossDamagedEvent?.RaiseEvent(currentHp, maxHp);

        if (currentHp > 0f)
        {
            TryEnterNextPhase();
        }

        if (IsDead) Die();
    }
    public int ReturnTakeDamage(float damage)
    {
        int takeDamage;
        if (currentHp < damage)
        {
            takeDamage = (int)currentHp;
        }
        else
        {
            takeDamage = (int)damage;
        }
        return takeDamage;
    }

    private void TryEnterNextPhase()
    {
        if (bossData == null || bossData.phases == null)
        {
            return;
        }

        int nextPhaseIndex = currentPhaseIndex + 1;
        if (nextPhaseIndex >= bossData.phases.Count)
        {
            return;
        }

        float nextPhaseHp = bossData.maxHp * bossData.phases[nextPhaseIndex].hpThreshold;
        if (currentHp <= nextPhaseHp)
        {
            if (attackRoutine != null)
            {
                StopCoroutine(attackRoutine);
                attackRoutine = null;
            }

            StartCoroutine(PhaseTransitionCo(nextPhaseIndex));
        }
    }

    private IEnumerator PhaseTransitionCo(int targetPhaseIndex)
    {
        currentState = BossState.PhaseTransition;
        canMove = false;

        currentPhaseIndex = targetPhaseIndex; // 인덱스 갱신

        // TODO: 2페이즈 연출
        Debug.Log($"{bossData.bossName} {currentPhaseIndex + 1}페이즈 돌입");

        yield return new WaitForSeconds(0.5f);

        // 새 페이즈의 패턴으로 교체
        currentPatterns = bossData.phases[currentPhaseIndex].phasePatterns;
        patternIndex = 0;
        patternTimer = 0f;

        canMove = true;
        currentState = BossState.Chasing;
    }

    private void Die()
    {
        if (isDeadHandled)
        {
            return;
        }

        isDeadHandled = true;
        currentState = BossState.Dead;
        canMove = false;
        // 플레이어에게 공격받지 않기 위해 모든 콜라이더 비활성화
        foreach (Collider2D bossCollider in bossColliders)
        {
            bossCollider.enabled = false;
        }

        if (bossLoopRoutine != null)
        {
            StopCoroutine(bossLoopRoutine);
            bossLoopRoutine = null;
        }

        if (visualRoutine != null)
        {
            StopCoroutine(visualRoutine);
            visualRoutine = null;
        }

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        if (animationController != null)
        {
            animationController.TriggerDead();
        }

        bossDeadEvent?.RaiseEvent();

        // 보스 사망 연출을 위해 Destroy(gameObject)는 호출하지 않음
    }

    private void FlipSprite()
    {
        if (sr == null) return;

        // 타겟의 위치 x값과 비교해 좌우 전환
        if (target != null)
        {
            sr.flipX = target.position.x > transform.position.x;
        }
    }
    
    private void UpdateAnimation()
    {
        if (animationController == null) return;

        bool isMoving = canMove && currentState == BossState.Chasing && target != null && !IsDead;
        animationController.PlayMove(isMoving);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    CollisionHandler(collision.gameObject);
    //}

    //private void OnCollisionEnterStay2D(Collision2D collision)
    //{
    //    CollisionHandler(collision.gameObject);
    //}

    //private void CollisionHandler(GameObject targetObj)
    //{
    //    if (targetObj.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
    //    {
    //        player.TakeDamage(bossData.damage);
    //    }
    //}
}
