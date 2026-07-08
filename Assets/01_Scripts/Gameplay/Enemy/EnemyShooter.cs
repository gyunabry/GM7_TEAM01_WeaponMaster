using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
- 몬스터의 탄막 공격 종류를 정의하는 스크립트
- 투사체의 방향, 속도, 위치, 수명, 타겟만 지정
- 실제 충돌 판정은 EnemyBullet이 직접 처리
*/

public class EnemyShooter : MonoBehaviour
{
    public struct PendingAOE
    {
        public float executeTime;
        public Vector2 position;
        public EnemyAttackData data;
        public WarningMarker marker;
        public Transform target;
    }

    private List<PendingAOE> pendingAOEs = new List<PendingAOE>();

    // [추가됨] 대기 중인 AOE 공격을 처리하는 Update 루프
    private void Update()
    {
        if (pendingAOEs.Count == 0) return;

        float currentTime = Time.time;

        // 리스트에서 요소가 삭제되므로 역순으로 순회
        for (int i = pendingAOEs.Count - 1; i >= 0; i--)
        {
            if (currentTime >= pendingAOEs[i].executeTime)
            {
                ExecuteAOE(pendingAOEs[i]);
                pendingAOEs.RemoveAt(i);
            }
        }
    }

    public void Fire(EnemyAttackData attackData, Transform target)
    {
        if (target == null) return;

        switch (attackData.bulletPattern)
        {
            case BulletPattern.Straight:
                FireStraight(attackData, target);
                break;
            case BulletPattern.Cone:
                FireCone(attackData, target);
                break;
            case BulletPattern.Circle:
                FireCircle(attackData, target);
                break;
            case BulletPattern.Burst:
                StartCoroutine(FireBurst(attackData, target));
                break;
            case BulletPattern.BurstAround:
                StartCoroutine(FireBurstAround(attackData, target));
                break;
            case BulletPattern.AOE:
                StartCoroutine(FireAOE(attackData, target));
                break;
            case BulletPattern.Homing:
                StartCoroutine(FireHoming(attackData, target));
                break;
        }
    }

    private void FireStraight(EnemyAttackData attackData, Transform target)
    {
        // 타겟 방향으로 발사
        Vector2 direction = (target.position - transform.position).normalized;
        SpawnProjectile(attackData, direction, target);
    }

    private void FireCone(EnemyAttackData attackData, Transform target)
    {
        Vector2 targetDir = target.position - transform.position;
        float baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        float angleRange = attackData.spreadAngle; // 데이터 상 발사각
        float startAngle = baseAngle - angleRange * 0.5f; // 시작 각도

        // 투사체 간 간격
        float angleStep = attackData.projectileCount > 1
            ? angleRange / (attackData.projectileCount - 1)
            : 0f;

        for (int i = 0; i < attackData.projectileCount; i++)
        {
            float angle = startAngle + angleStep * i;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            SpawnProjectile(attackData, direction, target);
        }
    }

    private void FireCircle(EnemyAttackData attackData, Transform target)
    {
        if (attackData.projectileCount <= 0) return;

        // 360도를 투사체 개수만큼 나누어 전방위로 발사
        float angleStep = 360f / attackData.projectileCount;

        for (int i = 0; i < attackData.projectileCount; i++)
        {
            float angle = angleStep * i;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            SpawnProjectile(attackData, direction, target);
        }
    }

    private IEnumerator FireBurst(EnemyAttackData attackData, Transform target)
    {
        for (int i = 0; i < attackData.burstCount; i++)
        {
            if (target == null) yield break;

            FireStraight(attackData, target);

            yield return new WaitForSeconds(attackData.burstInterval);
        }
    }

    // 랜덤한 방향으로 버스트 공격
    private IEnumerator FireBurstAround(EnemyAttackData attackData, Transform target)
    {
        for (int i = 0; i < attackData.fireCount; i++)
        {
            if (target == null) yield break;

            // 한 번 공격마다 플레이어 위치 추적해 투사체 발사
            Vector2 targetDir = (target.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            for (int j = 0; j < attackData.burstCount; j++)
            {
                float angle = baseAngle + Random.Range(
                    -attackData.spreadAngle * 0.5f,
                    attackData.spreadAngle * 0.5f
                );

                Vector2 direction = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                SpawnProjectile(attackData, direction, target);

                yield return new WaitForSeconds(attackData.burstInterval);
            }
        }
    }

    // 타겟 근처에 투사체 잠깐 스폰해 닿으면 피해
    private IEnumerator FireAOE(EnemyAttackData attackData, Transform target)
    {
        if (target == null) yield break;

        Vector2 centerPos = target.position;

        for (int i = 0; i < attackData.aoeCount; i++)
        {
            // Random.insdeUnitCircle을 통해 원형 범위 내 랜덤한 오프셋 값 계산
            Vector2 randomOffset = Random.insideUnitCircle * attackData.aoeRadius;
            Vector2 spawnPos = centerPos + randomOffset;

            WarningMarker warning = PoolManager.Instance.GetPool(attackData.warningPrefab);
            warning.transform.position = spawnPos;
            // 람다식으로 WarningMarker 내의 onComplete 발생 시 SpawnProjectileAt 메서드가 실행되도록 넘겨줌
            warning.PlayWarningEffect(attackData.warningDuration);

            // 대기열에 스폰 작업 추가
            pendingAOEs.Add(new PendingAOE
            {
                executeTime = Time.time + attackData.warningDuration,
                position = spawnPos,
                data = attackData,
                marker = warning,
                target = target
            });

            yield return null;
        }
    }

    private void ExecuteAOE(PendingAOE task)
    {
        // 경고표시 반환
        if (task.marker != null)
        {
            PoolManager.Instance.ReturnPool(task.marker);
        }

        SpawnProjectileAt(task.data, task.position, task.target, 0.5f);
    }

    private IEnumerator FireHoming(EnemyAttackData data, Transform target)
    {
        int projectileCount = Mathf.Max(data.projectileCount, 1);
        float angleRange = data.spreadAngle;
        float angleStep = projectileCount > 1
            ? angleRange / (projectileCount - 1)
            : 0f;

        for (int i = 0; i < projectileCount; i++)
        {
            if (target == null) yield break;

            Vector2 targetDir = target.position - transform.position;
            float baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            float angle = baseAngle - angleRange * 0.5f + angleStep * i;

            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            SpawnHomingProjectile(data, direction, target);

            if (data.burstInterval > 0f)
            {
                yield return new WaitForSeconds(data.burstInterval);
            }
        }
    }

    private void SpawnProjectile(EnemyAttackData attackData, Vector2 direction, Transform target, float lifetime = 4f)
    {
        EnemyBullet bullet = PoolManager.Instance.GetPool(attackData.projectilePrefab);

        bullet.transform.position = transform.position;

        bullet.InitBullet(
            attackData.attackDamage,
            direction,
            attackData.projectileSpeed,
            target,
            lifetime
        );
    }

    private void SpawnProjectileAt(EnemyAttackData attackData, Vector2 spawnPosition, Transform target, float lifetime = 0.5f)
    {
        // 총알을 풀에서 꺼내고 잠깐의 공격을 구현하기 위해 0.5초로 지속시간 설정
        EnemyBullet bullet = PoolManager.Instance.GetPool(attackData.projectilePrefab);

        bullet.transform.position = spawnPosition;

        bullet.InitBullet(
            attackData.attackDamage,
            Vector2.zero,
            0f,
            target,
            lifetime
        );
    }

    private void SpawnHomingProjectile(EnemyAttackData attackData, Vector2 direction, Transform target, float lifetime = 4f)
    {
        EnemyBullet bullet = PoolManager.Instance.GetPool(attackData.projectilePrefab);

        bullet.transform.position = transform.position;

        bullet.InitHomingBullet(
            attackData.attackDamage,
            direction,
            attackData.projectileSpeed,
            target,
            lifetime,
            attackData.homingTurnSpeed,
            attackData.homingDuration
        );
    }
}
