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
        PhaseTransition // 페이즈 전환 상태
    }

    [Header("보스 데이터")]
    [SerializeField] private BossData bossData;
    private float maxHp;

    [Header("현재 상태")]
    [SerializeField] private BossState currentState;
    [SerializeField] private float currentHp;
    [SerializeField] private int currentPhaseIndex = 0;
    private List<EnemyPatternData> currentPatterns;

    [Header("보스 이벤트")]
    [SerializeField] private VoidEventChannel bossDeadEvent;
    [SerializeField] private HpEventChannel bossDamagedEvent;

    [Header("컴포넌트 참조")]
    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private Transform target;
    private EnemyShooter bossShooter;
    private SpriteRenderer sr;
    private EnemyAnimationController animationController;

    [SerializeField] private LayerMask targetLayer;

    private float patternTimer = 0f;
    private int patternIndex;


    public string BossName => bossData != null ? bossData.bossName : string.Empty;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        bossShooter = GetComponent<EnemyShooter>();
        animationController = GetComponent<EnemyAnimationController>();
        sr = GetComponentInChildren<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        InitializeBoss();
        StartCoroutine(BossLoopCo());
    }

    private void Update()
    {
        FlipSprite();
        UpdateAnimation();
    }

    private void InitializeBoss()
    {
        if (bossData == null || bossData.phases.Count == 0) return;

        maxHp = bossData.maxHp;
        currentHp = maxHp;

        agent.speed = bossData.moveSpeed;

        // 1페이즈 세팅
        currentPhaseIndex = 0;
        currentPatterns = bossData.phases[currentPhaseIndex].phasePatterns;

        // 현재 상태를 추적 상태로 전환
        currentState = BossState.Chasing;
    }

    // 이동과 패턴 실행 메인 루프를 관리하는 코루틴
    private IEnumerator BossLoopCo()
    {
        while (!IsDead)
        {
            DetectPlayer();
            if (target == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            FaceToTarget();

            switch (currentState)
            {
                case BossState.Chasing:
                    if (!agent.enabled) agent.enabled = true;
                    agent.isStopped = false;
                    agent.SetDestination(target.position);

                    patternTimer += 0.15f;
                    if (patternTimer >= 2f)
                    {
                        SelectAndExecutePattern();
                    }
                    break;

                case BossState.ActionRunning:
                    //agent.enabled = false;
                    break;

                case BossState.PhaseTransition:
                    //agent.enabled = false;
                    break;
            }
            yield return new WaitForSeconds(0.15f);
        }
    }

    // 반경 20 안에 있는 플레이어를 찾아서 타겟으로 등록
    private void DetectPlayer()
    {
        if (target != null) return;
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 20f, targetLayer);
        if (collider != null) target = collider.transform;
    }

    private void FaceToTarget()
    {
        if (target == null) return;
        float directionX = target.position.x - transform.position.x;
        // SpriteRenderer 추가 후 flipX 여부
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
        StartCoroutine(ExecuteBossPatternCo(pattern));
    }

    private IEnumerator ExecuteBossPatternCo(EnemyPatternData pattern)
    {
        patternTimer = 0f;

        foreach (EnemyAttackData attackData in pattern.attackSequence)
        {
            yield return StartCoroutine(HandleAttackDataCo(attackData));
            yield return new WaitForSeconds(pattern.actionDelay);
        }

        // 패턴 종료 후 다시 추적 상태로 전환
        if (currentState == BossState.ActionRunning)
        {
            currentState = BossState.Chasing;
        }
    }

    private IEnumerator HandleAttackDataCo(EnemyAttackData attackData)
    {
        switch (attackData.attackType)
        {
            case AttackType.Dash:
                yield return StartCoroutine(BossDashCo(attackData));
                break;
            case AttackType.Range:
                yield return StartCoroutine(ExecuteRangeAttack(attackData));
                animationController.TriggerAttack();
                break;
        }
    }

    private IEnumerator BossDashCo(EnemyAttackData attackData)
    {
        // 대쉬 전 대기 동작
        agent.enabled = false;
        Vector3 lastPosition = target.position; // 대쉬 직전 타겟 위치 저장
        Vector3 dashDirection = (lastPosition - transform.position).normalized;
        yield return new WaitForSeconds(0.5f);

        // 대쉬 실행
        float dashSpeed = attackData.dashSpeed;
        float dashRange = attackData.dashRange;
        float dashDuration = dashRange / dashSpeed;
        float startTime = Time.time;

        while (Time.time - startTime < 0.3f)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            yield return null;
        }
        rb.linearVelocity = Vector2.zero;
        
        // NavMeshAgent를 껐다 다시 켰을 때 위치를 실제 위치와 동기화
        agent.Warp(transform.position);
        // 추적 재개
        agent.enabled = true;
    }

    private IEnumerator ExecuteRangeAttack(EnemyAttackData data)
    {
        agent.isStopped = true; // 원거리 공격 시 추적 일시정지
        yield return new WaitForSeconds(0.2f); // 발사 전 딜레이

        bossShooter.Fire(data, target);

        // TODO: 수치 조정 필요
        yield return new WaitForSeconds(currentPatterns[currentPhaseIndex].actionDelay); // 발사 후 딜레이
        agent.isStopped = false;
    }

    // 테스트 후 제거
    #region 탄막 패턴
    private void FireStraight(EnemyAttackData attackData)
    {
        // 타겟 방향으로 발사
        Vector2 direction = (target.position - transform.position).normalized;
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

        int nextPhaseIndex = currentPhaseIndex + 1;
        // index는 0부터 시작하기 때문에 Count - 1과 같음
        if (nextPhaseIndex < bossData.phases.Count)
        {
            // 다음 페이즈 진입 HP 저장
            float nextPhaseHp = bossData.maxHp * bossData.phases[nextPhaseIndex].hpThreshold;
            if (currentHp <= nextPhaseHp)
            {
                StopAllCoroutines();
                StartCoroutine(PhaseTransitionCo(nextPhaseIndex));
            }
        }

        if (IsDead) Die();
    }

    private IEnumerator PhaseTransitionCo(int targetPhaseIndex)
    {
        currentState = BossState.PhaseTransition;
        currentPhaseIndex = targetPhaseIndex; // 인덱스 갱신

        // 페이즈 전환 시 실행중인 공격, 이동 정지
        rb.linearVelocity = Vector2.zero;
        if (!agent.enabled) agent.enabled = true;
        agent.isStopped = false;

        // TODO: 2페이즈 연출
        Debug.Log($"{bossData.bossName} {currentPhaseIndex + 1}페이즈 돌입");

        //// 보스가 중앙으로 이동하도록 설정
        //Vector3 mapCenter = Vector3.zero;
        //agent.SetDestination(mapCenter);
        //while (Vector3.Distance(transform.position, mapCenter) > 0.5f)
        //{
        //    yield return null;
        //}

        // 새 페이즈의 패턴으로 교체
        currentPatterns = bossData.phases[currentPhaseIndex].phasePatterns;
        patternIndex = 0;
        currentState = BossState.Chasing;

        StartCoroutine(BossLoopCo());

        yield return null;
    }

    private void Die()
    {
        // 사망 시 보스의 모든 코루틴 종료
        StopAllCoroutines();

        animationController.TriggerDead();

        rb.linearVelocity = Vector2.zero;

        bossDeadEvent?.RaiseEvent();
        //Destroy(gameObject); // 보스 사망 연출을 위해 파괴 X
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

        bool isMoving = agent.velocity.sqrMagnitude > 0.001f;
        animationController.PlayMove(isMoving);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionHandler(collision.gameObject);
    }

    private void OnCollisionEnterStay2D(Collision2D collision)
    {
        CollisionHandler(collision.gameObject);
    }

    private void CollisionHandler(GameObject targetObj)
    {
        if (targetObj.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.TakeDamage(bossData.damage);
        }
    }
}
