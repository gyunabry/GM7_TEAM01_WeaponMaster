using System.Collections.Generic;
using UnityEngine;

public class EnemyContactDamageManager : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float playerHitRadius = 0.2f;
    [SerializeField] private float checkRadius = 2f;
    [SerializeField] private float checkInterval = 0.1f;

    private readonly List<EnemyController> nearEnemies = new();
    private float checkTimer;

    private void Awake()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
    }

    private void Update()
    {
        checkTimer += Time.deltaTime;

        // 0.1초에 한 번씩 조회 후 접촉 확인
        if (checkTimer < checkInterval) return;
        checkTimer = 0;

        CheckContactDamage();
    }

    private void CheckContactDamage()
    {
        if (player == null || SpatialHashGrid.Instance == null) return;

        Vector2 playerPos = player.transform.position;

        SpatialHashGrid.Instance.GetEnemiesInRadius(playerPos, checkRadius, nearEnemies);

        foreach (EnemyController enemy in nearEnemies)
        {
            float totalRadius = enemy.HitRadius + playerHitRadius;
            float totalRadiusSqr = totalRadius * totalRadius;

            Vector2 diff = (Vector2)enemy.transform.position - playerPos;

            if (diff.sqrMagnitude <= totalRadiusSqr)
            {
                player.TakeDamage(enemy.ContactDamage);
            }
        }
    }
}
