using System.Collections;
using UnityEngine;

/*
- 몬스터의 탄막 공격 종류를 정의하는 스크립트
*/

public class EnemyShooter : MonoBehaviour
{
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
                FireCircle(attackData);
                break;
            case BulletPattern.Orbit:
                FireOrbit(attackData);
                break;
            case BulletPattern.Burst:
                StartCoroutine(FireBurst(attackData, target));
                break;
            case BulletPattern.BurstAround:
                StartCoroutine(FireBurstAround(attackData, target));
                break;
        }
    }

    private void FireStraight(EnemyAttackData attackData, Transform target)
    {
        // 타겟 방향으로 발사
        Vector2 direction = (target.position - transform.position).normalized;
        SpawnProjectile(attackData, direction);
    }

    private void FireCone(EnemyAttackData attackData, Transform target)
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

    private void FireOrbit(EnemyAttackData attackData)
    {

    }

    private IEnumerator FireBurst(EnemyAttackData attackData, Transform target)
    {
        Transform lastTarget = target;

        for (int i = 0; i < attackData.burstCount; i++)
        {
            if (target == null) yield break;

            FireStraight(attackData, lastTarget);

            yield return new WaitForSeconds(attackData.burstInterval);
        }
    }

    // 랜덤한 방향으로 버스트 공격
    private IEnumerator FireBurstAround(EnemyAttackData attackData, Transform target)
    {
        for (int i = 0; i < attackData.fireCount; i++)
        {
            // 한 번 공격마다 플레이어 위치 추적해 투사체 발사
            Vector2 targetDir = (target.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

            for (int j = 0; j < attackData.burstCount; j++)
            {
                float angle = baseAngle + Random.Range(-attackData.spreadAngle / 2f, attackData.spreadAngle / 2f);

                float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
                float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);

                Vector2 randomDir = new Vector2(dirX, dirY).normalized;
                Debug.Log(randomDir);

                SpawnProjectile(attackData, randomDir);

                yield return new WaitForSeconds(attackData.burstInterval);
            }
        }
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
}
