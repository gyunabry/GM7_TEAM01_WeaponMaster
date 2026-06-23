using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.Rendering;


public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("스폰 영역")]
    [SerializeField] private float minX = -10.0f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -7.9f;
    [SerializeField] private float maxY = 6.3f;

    [SerializeField] private EnemyController genericEnemyPrefab;

    private List<Coroutine> activeSpawnCoroutines = new List<Coroutine>();

    private void Awake()
    {
        Instance = this;
    }
    public void SetupNextWave(WaveData waveData)
    {
        StopAllSpawnCoroutines();
        for(int i = 0; i< waveData.spawnList.Length;i++)
        {
            SpawnInfo currentinfo = waveData.spawnList[i];

            Coroutine co = StartCoroutine(SpawnRoutine(currentinfo));
            activeSpawnCoroutines.Add(co);
        }
       
    }
    private IEnumerator SpawnRoutine(SpawnInfo info)
    {
        float countMultiplier = WaveManager.Instance.SpawnCountMultiplier;
        float hpMultiplier = WaveManager.Instance.EnemyHpMultiplier;
        float speedMultiplier = WaveManager.Instance.EnemyMoveSpeedMultiplier;

        int theSpawnCount = Mathf.RoundToInt(info.spawnCount * countMultiplier);

        yield return new WaitForSeconds(info.spawnDelay);
        while(true)
        {
            for(int i = 0; i<theSpawnCount; i++)
            {
                //GameObject enemy = Instantiate(info.enemyPrefab);
                EnemyController enemy = PoolManager.Instance.GetPool(genericEnemyPrefab);
                enemy.Initialize(info.enemyData);

                float randomX = Random.Range(minX, maxX);
                float randomY = Random.Range(minY, maxY);

                enemy.transform.position = new Vector3(randomX, randomY, 0.0f);
                enemy.transform.rotation = Quaternion.identity;
            }
            yield return new WaitForSeconds(info.spawnInterval);
        }
    }
        
    private void StopAllSpawnCoroutines()
    {
        foreach (var co in activeSpawnCoroutines)
        {
            if(co!=null) StopCoroutine(co);
        }
        activeSpawnCoroutines.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 center = Vector3.zero;
        Vector3 size = new Vector3(maxX - minX, maxY - minY, 0.0f);

        Gizmos.DrawWireCube(center, size);
    }
}
