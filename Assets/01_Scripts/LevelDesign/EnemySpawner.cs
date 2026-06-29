using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// 스폰 작업 대기열을 만들기 위한 구조체 (GC 없애기용)
public struct PendingSpawn
{
    public float executeTime;
    public Vector2 position;
    public EnemyData enemyData;
    public WarningMarker marker;
}

public class EnemySpawner : MonoBehaviour
{

    public static EnemySpawner Instance { get; private set; }

    [Header("스폰 영역")]
    [SerializeField] private float minX = -10.0f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -7.9f;
    [SerializeField] private float maxY = 6.3f;

    [Header("경고 표시")]
    [SerializeField] private float warningDuration = 1f;

    [Header("스폰할 오브젝트 프리팹")]
    [SerializeField] private WarningMarker markerPrefab;
    [SerializeField] private EnemyController enemyPrefab;

    private List<Coroutine> activeSpawnCoroutines = new List<Coroutine>();

    // 스폰 대기열 리스트
    private List<PendingSpawn> pendingSpawns = new List<PendingSpawn>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (pendingSpawns.Count == 0) return;

        float currentTime = Time.time;

        // 리스트 역순 순회
        for (int i = pendingSpawns.Count - 1; i >= 0; i--)
        {
            if (currentTime >= pendingSpawns[i].executeTime)
            {
                // 시간이 다 되면 스폰 실행
                ExecuteSpawn(pendingSpawns[i]);

                // 대기열에서 제거
                pendingSpawns.RemoveAt(i);
            }
        }
    }

    public void SetupNextWave(WaveData waveData)
    {
        StopAllSpawnCoroutines();
        for(int i = 0; i< waveData.spawnList.Length;i++)
        {
            SpawnInfo currentinfo = waveData.spawnList[i];

            Coroutine co = StartCoroutine(SpawnRoutine(currentinfo,waveData.waveDuration));
            activeSpawnCoroutines.Add(co);
        }
    }

    private IEnumerator SpawnRoutine(SpawnInfo info, float duration)
    {
        float countMultiplier = WaveManager.Instance.SpawnCountMultiplier;
        float hpMultiplier = WaveManager.Instance.EnemyHpMultiplier;
        float speedMultiplier = WaveManager.Instance.EnemyMoveSpeedMultiplier;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float curveValue = 1f;
            if(info.spawnDensityCurve != null && info.spawnDensityCurve.keys.Length>0)
            {
                curveValue = info.spawnDensityCurve.Evaluate(progress);
            }

            int finalSpawnCount = Mathf.RoundToInt(info.spawnCount * countMultiplier * curveValue);

            for(int i = 0;i< finalSpawnCount;i++)
            {
                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);
                Vector2 randomSpawnPos = new Vector2(randomX, randomY);

                // 스폰 위치에 미리 표식으로 경고 표시
                WarningMarker warningMarker = PoolManager.Instance.GetPool(markerPrefab);
                warningMarker.transform.position = randomSpawnPos;

                // 대기열에 스폰 작업 등록
                pendingSpawns.Add(new PendingSpawn
                {
                    executeTime = Time.time + warningDuration,
                    position = randomSpawnPos,
                    enemyData = info.enemyData,
                    marker = warningMarker
                });
            }
            yield return new WaitForSeconds(info.spawnInterval);
            elapsed += info.spawnInterval;
        }
    }

    // 실행 시간이 다 된 스폰 작업을 처리하는 메서드
    private void ExecuteSpawn(PendingSpawn task)
    {
        // 경고 표시 반환
        if (task.marker != null)
        {
            PoolManager.Instance.ReturnPool(task.marker);
        }
        EnemyController enemy = PoolManager.Instance.GetPool(enemyPrefab);
        // 몬스터 정보 주입 후 태스크에 전달된 랜덤 위치로 설정
        enemy.Initialize(task.enemyData);
        enemy.transform.position = task.position;
        enemy.transform.rotation = Quaternion.identity;
    }
        
    private void StopAllSpawnCoroutines()
    {
        foreach (var co in activeSpawnCoroutines)
        {
            if(co!=null) StopCoroutine(co);
        }
        activeSpawnCoroutines.Clear();
    }

    public void ClearAllEnemies()
    {
        StopAllSpawnCoroutines();

        // 남아있는 대기열 반환
        foreach (var task in pendingSpawns)
        {
            if (task.marker != null)
            {
                PoolManager.Instance.ReturnPool(task.marker);
            }
        }
        pendingSpawns.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.0f);

        Gizmos.DrawWireCube(center, size);
    }
}
