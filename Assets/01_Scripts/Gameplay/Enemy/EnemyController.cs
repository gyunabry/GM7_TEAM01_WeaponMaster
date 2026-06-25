using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/*
- 스포너에서 데이터를 주입할 수 있도록 메서드 정의
- 플레이어 추적 기능
 */

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("기본 설정")]
    [SerializeField] private float maxHp;
    [SerializeField] private int armor;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask targetLayer;
    
    private EnemyData currentEnemy;
    private SpriteRenderer sr;
    private EnemyAnimationController animationController;

    private float currentHp;
    public Transform target;

    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private EnemyAttack enemyAttack;
    private Animator animator;

    private Coroutine chaseCoroutine;
    // 코루틴 내에서 SetDestination 호출 딜레이
    private WaitForSeconds chaseInterval = new WaitForSeconds(0.5f);

    // 현재 체력이 0보다 작거나 같다면 true 반환
    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
        animationController = GetComponent<EnemyAnimationController>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // Z축 회전 방지
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        DetectTarget();

        // 타겟을 찾았다면 추적 코루틴 실행
        if (target != null)
        {
            if (chaseCoroutine == null)
            {
                chaseCoroutine = StartCoroutine(ChaseTargetCo());
            }
        }

        FlipSprite();
        UpdateAnimation();
    }

    // 비활성화시 남아있는 코루틴과 타겟 초기화
    private void OnDisable()
    {
        target = null;
        if (chaseCoroutine != null)
        {
            StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
    }

    public void Initialize(EnemyData data)
    {
        currentEnemy = data;

        // 주입 확인용
        gameObject.name = data.enemyName;

        maxHp = data.maxHp;
        armor = data.armor;
        moveSpeed = data.moveSpeed;
        currentHp = maxHp;

        animationController.SetupAnimator(data.runtimeAnimator);

        // enemyAttack이 있다면 현재 몬스터의 공격 패턴 주입
        if (enemyAttack != null)
        {
            enemyAttack.Initialize(currentEnemy.enemyPattern);
        }

        ApplyStatusAgent();
    }

    private void DetectTarget()
    {
        if (target != null) return;

        Collider2D hits = Physics2D.OverlapCircle(transform.position, 100f, targetLayer);
        if (hits != null)
        {
            target = hits.transform;
        }
    }

    // 주입받은 데이터를 NavMeshAgent의 필드에 할당
    private void ApplyStatusAgent()
    {
        agent.speed = moveSpeed;
    }

    // NavMeshAgent의 이동 목적지로 타겟의 위치를 설정
    // 최적화를 위해 코루틴에서 SetDestination 호출에 딜레이
    private IEnumerator ChaseTargetCo()
    {
        while (target != null)
        {
            agent.SetDestination(target.position);
            
            yield return chaseInterval;
        }
        chaseCoroutine = null;
    }

    public void TakeDamage(float damage, bool isCrit = false)
    {
        if (IsDead) return; // 이미 죽은 상태면 추가 데미지 연산 무시

        currentHp -= damage;

        HitText hitText = PoolManager.Instance.GetPool<HitText>();
        hitText.ShowDamage(damage, transform.position, isCrit);

        // TODO: 하얗게 반짝이는 효과 추가

        // 피해를 입은 직후 체력을 검사하여 사망 처리
        if (IsDead)
        {
            Die();
        }
    }

    private void Die()
    {
        // 몬스터 데이터에 있는 드랍 아이템을 활성화
        currentEnemy.DropItem(transform.position);
        // 킬카운트 증가
        GameManager.Instance.AddKillCount();

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(this);
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
}