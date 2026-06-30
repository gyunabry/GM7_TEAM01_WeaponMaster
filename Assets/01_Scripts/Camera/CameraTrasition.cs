using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraTrasition : MonoBehaviour
{
    [Header("구독할 이벤트")]
    [SerializeField] private VoidEventChannel bossEncounterEvent;
    [SerializeField] private VoidEventChannel bossDeadEvent;
    [SerializeField] private VoidEventChannel bossClearEvent;

    [Header("시네머신 카메라")]
    [SerializeField] private CinemachineCamera playerCamera;
    [SerializeField] private CinemachineCamera bossCamera;

    [Header("보스 스폰")]
    [SerializeField] private BossSpawner bossSpawner;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("연출 시간 설정")]
    [SerializeField] private float preSpawnFocusDuration = 2f;
    [SerializeField] private float postSpawnGraceDuration = 2f;
    [SerializeField] private float deathFocusDuration = 3f;

    private Coroutine spawnRoutine;
    private Coroutine deathRoutine;

    private void Start()
    {
        if (bossEncounterEvent != null)
        {
            bossEncounterEvent.OnEventRaised += PlayBossSpawn;
        }
        if (bossDeadEvent != null)
        {
            bossDeadEvent.OnEventRaised += PlayBossDead;
        }
    }

    private void OnDisable()
    {
        if (bossEncounterEvent != null)
        {
            bossEncounterEvent.OnEventRaised -= PlayBossSpawn;
        }
        if (bossDeadEvent != null)
        {
            bossDeadEvent.OnEventRaised -= PlayBossDead;
        }
    }

    private void PlayBossSpawn()
    {
        if (spawnRoutine != null) return;
        spawnRoutine = StartCoroutine(BossSpawnCameraCo());
    }

    private IEnumerator BossSpawnCameraCo()
    {
        GameManager.Instance.PauseGame();

        if (bossCamera != null)
        {
            // 보스 스폰 위치를 카메라 타겟으로 설정
            bossCamera.Follow = bossSpawnPoint;
            bossCamera.Priority = 20;
        }

        if (playerCamera != null)
        {
            playerCamera.Priority = 10;
        }

        GameManager.Instance.PauseGame();

        // 일시정지 상태이기 때문에 Realtime 사용
        // 스폰 전 포커스 시간
        yield return new WaitForSecondsRealtime(preSpawnFocusDuration);

        BossController spawnedBoss = null;
        if (bossSpawner != null)
        {
            spawnedBoss = bossSpawner.SpawnBoss();
        }

        // 보스가 있다면 보스 카메라가 보스를 따라가도록 설정
        if (spawnedBoss != null && bossCamera != null)
        {
            bossCamera.Follow = spawnedBoss.transform;
        }

        yield return new WaitForSecondsRealtime(postSpawnGraceDuration);

        if (bossCamera != null)
        {
            bossCamera.Priority = 0;
        }
        
        GameManager.Instance.ResumeGame();
        spawnRoutine = null;
    }

    private void PlayBossDead()
    {
        if (deathRoutine != null) return;
        deathRoutine = StartCoroutine(BossDeadCameraCo());
    }

    private IEnumerator BossDeadCameraCo()
    {
        // GameManager.Instance.PauseGame();
        Time.timeScale = 0.3f; // 슬로우 연출

        BossController boss = FindFirstObjectByType<BossController>();

        if (boss != null && bossCamera != null)
        {
            bossCamera.Follow = boss.transform;
            bossCamera.Priority = 20;
        }

        if (playerCamera != null)
        {
            playerCamera.Priority = 10;
        }

        yield return new WaitForSeconds(deathFocusDuration);

        bossCamera.Priority = 0;

        bossClearEvent?.RaiseEvent();
        GameManager.Instance.ResumeGame();
        deathRoutine = null;
    }
}
