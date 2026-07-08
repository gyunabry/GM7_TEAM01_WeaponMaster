using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("보스 프리팹")]
    [SerializeField] private GameObject bossPrefab;

    [Header("타겟 플레이어")]
    [SerializeField] private Transform targetTransform;

    [Header("보스 스폰 위치")]
    [SerializeField] private Transform bossSpawnPoint;
    
    // 선택된 스테이지로부터 보스 데이터 가져옴
    public BossController SpawnBoss()
    {
        StageData currentStage = GameSceneData.SelectedStage;
        BossData currentBossData = currentStage != null ? currentStage.bossData : null;

        return SpawnBoss(currentBossData);
    }

    public BossController SpawnBoss(BossData bossData)
    {
        if (bossPrefab == null || bossSpawnPoint == null || bossData == null)
        {
            return null;
        }

        GameObject bossObj = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);
        BossController currentBoss = bossObj.GetComponent<BossController>();
        if (currentBoss == null)
        {
            Destroy(bossObj);
            return null;
        }

        currentBoss.Initialize(bossData, targetTransform);

        if (InGameUIManager.Instance != null)
        {
            InGameUIManager.Instance.SetBossInfo(currentBoss.BossName, currentBoss.CurrentHp, currentBoss.MaxHp);
        }

        return currentBoss;
    }
}