using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("보스 프리팹")]
    [SerializeField] private GameObject bossPrefab;

    [Header("보스 스폰 위치")]
    [SerializeField] private Transform bossSpawnPoint;

    public BossController SpawnBoss()
    {
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            return null;
        }

        GameObject bossObj = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

        BossController currentBoss = bossObj.GetComponent<BossController>();
        if (currentBoss == null)
        {
            return null;
        }

        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.SetBossInfo(currentBoss.BossName, currentBoss.CurrentHp, currentBoss.MaxHp);
        }

        return currentBoss;
    }
}