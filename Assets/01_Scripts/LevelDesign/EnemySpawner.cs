using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("스폰 위치 배열")]
    [SerializeField] private Transform[] spawnPoints;

    private List<Coroutine> activeSpawnCoroutines = new List<Coroutine>();

    private void Awake()
    {
        Instance = this;
    }
    public void SetupNextWave(WaveData waveData)
    {
        StopAllSpawnCoroutines();
        for(int i = 0;i< waveData.spawnList.Length;i++)
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
        while(true)
        {
            for(int i =0; i<theSpawnCount; i++)
            {
                GameObject enemy = Instantiate(info.enemyPrefab);

                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                enemy.transform.position = randomPoint.position;
                enemy.transform.rotation = randomPoint.rotation;

                enemy.SetActive(true);
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
}
