using System.Collections;
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
    [SerializeField] private float moveSpeed;

    [Header("시각 갱신 주기")]
    [SerializeField] private float visualUpdateInterval = 0.1f;

    [Header("충돌 판정")]
    [SerializeField] private float hitRadius = 0.5f;

    [Header("피격 연출")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("공간해싱 설정")]
    [SerializeField] private float separationRadius = 0.7f;
    [SerializeField] private float separationWeight = 1.2f;

    private Material originMaterial;
    private EnemyData currentEnemy;
    private SpriteRenderer sr;
    private EnemyAnimationController animationController;
    private EnemyAttack enemyAttack;

    private float currentHp;

    // 플레이어 트랜스폼을 한 번 찾고 모든 EnemyController가 공유할 수 있도록 전역 변수로 선어
    private static Transform cachedPlayerTarget;

    private Coroutine visualCoroutine;
    private Coroutine flashCoroutine;
    private WaitForSeconds visualWait;

    public float HitRadius => hitRadius;
    public Transform target;
    public bool CanMove { get; set; } = true;
    public bool IsDead => currentHp <= 0;

    private void Awake()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        animationController = GetComponent<EnemyAnimationController>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
        {
            originMaterial = sr.material;
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

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        SpatialHashGrid.Instance?.Unregister(this);
    }

    private void Start()
    {
        ConnectTargetOnce();
    }

    private void Update()
    {
        // 기존 플레이어 탐지 및 스프라이트 렌더러 조작 제거
        MoveToTarget();
    }

    public void Initialize(EnemyData data)
    {
        currentEnemy = data;

        // 주입 확인용
        gameObject.name = data.enemyName;

        maxHp = data.maxHp;
        moveSpeed = data.moveSpeed;
        currentHp = maxHp;

        CanMove = true;

        if (animationController != null)
        {
            animationController.SetupAnimator(data.runtimeAnimator);
        }

        // enemyAttack이 있다면 현재 몬스터의 공격 패턴 주입
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

    // 타겟이 비어있을 때만 호출해 플레이어를 연결
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
            // 이동 전 셀 위치 갱신
            SpatialHashGrid.Instance.UpdateEnemyCell(this);

            separation = SpatialHashGrid.Instance.GetSeparationForce(this, separationRadius);
        }

        // 플레이어 추적 방향 + 주변 적으로부터 밀리는 힘
        Vector2 finalDir = chaseDir.normalized + separation * separationWeight;

        if (finalDir.sqrMagnitude <= 0.0001f) return;

        transform.position += (Vector3)(finalDir.normalized * moveSpeed * Time.deltaTime);

        SpatialHashGrid.Instance?.UpdateEnemyCell(this);

        //Vector3 dir = targetPos - currentPos;
        //dir.z = 0f;

        // 타겟에 근접했을 땐 리턴
        //if (dir.sqrMagnitude <= 0.001f) return;

        //transform.position += dir.normalized * moveSpeed * Time.deltaTime;
    }

    // 기존 Update에서 매 프레임 호출되던 구조를 코루틴으로 변경해 최적화
    private IEnumerator VisualUpdateCo()
    {
        while (!IsDead)
        {
            FlipSprite();
            UpdateAnimation();

            yield return visualWait;
        }

        visualCoroutine = null;
    }

    public void TakeDamage(float damage, bool isCrit = false)
    {
        if (IsDead) return; // 이미 죽은 상태면 추가 데미지 연산 무시

        currentHp -= damage;

        HitText hitText = PoolManager.Instance.GetPool<HitText>();
        hitText.ShowDamage(damage, transform.position, isCrit);

        // TODO: 하얗게 반짝이는 효과 추가
        OnDamaged();

        // 피해를 입은 직후 체력을 검사하여 사망 처리
        if (IsDead)
        {
            Die();
        }
    }public int ReturnTakeDamage(float damage)
    {
        int takeDamage;
        if(currentHp < 0)
        {
            takeDamage = (int)damage + (int)currentHp;
        }
        else
        {
            takeDamage = (int)damage;
        }
        return takeDamage;
    }

    private void Die()
    {
        CanMove = false;

        // 몬스터 데이터에 있는 드랍 아이템을 활성화
        currentEnemy.DropItem(transform.position);
        // 킬카운트 증가
        GameManager.Instance.AddKillCount();

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        // 반짝일 때 죽은 적을 원상복구하기 위해 해당 함수에서 다시 설정
        if (sr != null)
        {
            sr.material = originMaterial;
        }

        PoolManager.Instance.ReturnPool(this);
    }

    private void FlipSprite()
    {
        if (sr == null || target == null) return;

        // 타겟의 위치 x값과 비교해 좌우 전환
        sr.flipX = target.position.x > transform.position.x;
    }

    private void UpdateAnimation()
    {
        if (animationController == null) return;

        bool isMoving = CanMove && target != null && !IsDead;
        animationController.PlayMove(isMoving);
    }

    private void OnDamaged()
    {
        if (sr == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashSprite());
    }

    // 피격됐을 때 잠깐 번쩍이는 효과
    private IEnumerator FlashSprite()
    {
        sr.material = flashMaterial;

        yield return new WaitForSeconds(flashDuration);

        sr.material = originMaterial;
        flashCoroutine = null;
    }
}