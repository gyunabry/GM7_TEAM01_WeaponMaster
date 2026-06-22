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

    private float currentHp;
    public Transform target;

    private Rigidbody2D rb;
    private NavMeshAgent agent;
    private EnemyAttack enemyAttack;

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
            FaceToTarget();

            if (chaseCoroutine == null)
            {
                chaseCoroutine = StartCoroutine(ChaseTargetCo());
            }
        }
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

    public void TakeDamage(float damage)
    {
        if (IsDead) return; // 이미 죽은 상태면 추가 데미지 연산 무시

        currentHp -= damage;

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
        GameManager.Instance.AddKillCount();
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        PoolManager.Instance.ReturnPool(this);
    }

    private void DropItem()
    {
        // 확정적으로 경험치 드랍
        // 확률에 따라 음식 드랍
        // if ()
    }

    private void FaceToTarget()
    {
        if (target == null) return;

        // 플레이어 방향 벡터 계산
        Vector2 direction = (target.position - transform.position).normalized;

        // 방향을 각도로 변환
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 해당 각도를 향하는 쿼터니언 값 저장
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // 부드러운 회전 적용
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 360f);
    }
}