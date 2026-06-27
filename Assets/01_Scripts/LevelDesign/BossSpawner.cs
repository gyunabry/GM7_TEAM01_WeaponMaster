using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("구독할 이벤트")]
    [SerializeField] private VoidEventChannel BossEncounterEvent;

    [Header("보스 프리팹")]
    [SerializeField] private GameObject bossPrefab;

    private void Start()
    {
        BossEncounterEvent.OnEventRaised += OnBossEncounter;
    }

    private void OnDestroy()
    {
        BossEncounterEvent.OnEventRaised -= OnBossEncounter;
    }

    // 보스 출현 이벤트 발행 시 호출할 메서드
    private void OnBossEncounter()
    {
        Debug.Log("보스 출현!");

        if (bossPrefab == null) return;

        BossController currentBoss = Instantiate(bossPrefab, Vector2.zero, Quaternion.identity).GetComponent<BossController>();
        string bossName = currentBoss.GetComponent<BossData>().bossName;
        InGameUIManager.Instance.SetBossName(bossName);
        Debug.Log($"{bossName} 인게임 UI에 전달");
    }
}