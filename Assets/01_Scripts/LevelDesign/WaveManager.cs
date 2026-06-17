using UnityEngine;
using System.Collections.Generic;

public enum Difficulty { Normal, Hard, Hell}
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("난이도 설정")]
    [SerializeField] private Difficulty currentDifficulty = Difficulty.Normal;

    [Header("웨이브 데이터")]
    [SerializeField] private List<WaveData> stageWaves;

    private float stageTime = 0;
    private int currentWaveIndex = 0;
    private bool isWaveActive = false;

    public float SpawnCountMultiplier => currentDifficulty == Difficulty.Hard ? 1.2f : (currentDifficulty == Difficulty.Hell ? 1.5f : 1.0f);
    public float EnemyHpMultiplier => currentDifficulty == Difficulty.Hard ? 1.2f : (currentDifficulty == Difficulty.Hell ? 1.5f : 1.0f);
    public float EnemyMoveSpeedMultiplier => currentDifficulty == Difficulty.Hard?1.2f : (currentDifficulty == Difficulty.Hell?1.5f : 1.0f);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartStage();
    }


    public void SetupStageWaves(List<WaveData> newstageWaves)
    {
        this.stageWaves = newstageWaves;
    }
    public void StartStage()
    {
        stageTime = 0f;
        
        isWaveActive = true;

        int listIndex = currentWaveIndex % stageWaves.Count;

        StartWave(listIndex);
    }
    private void Update()
    {
        if (!isWaveActive) return;
        stageTime += Time.deltaTime;
        
        CheckWaveTimeLine();

    }
    public void CheckWaveTimeLine()
    {
        int listIndex = currentWaveIndex%stageWaves.Count;
        WaveData currentWave = stageWaves[currentWaveIndex];
        if(stageTime>=currentWave.waveDuration)
        {
            
            currentWaveIndex++;
            if(currentWaveIndex % stageWaves.Count == 0)
            {
                isWaveActive=false;
                Debug.Log($"스테이지 끝남 확인용");
                //상점 켜기
            }
            else
            {
                stageTime = 0;
                int nextListIndex = currentWaveIndex%stageWaves.Count;
                StartWave(nextListIndex);
            }
        }
    }
    private void StartWave(int targetIndex)
    {
        if(stageWaves==null ||  stageWaves.Count==0)
        {
            return;
        }
        
        
        EnemySpawner.Instance.SetupNextWave(stageWaves[targetIndex]);
    }
    private void CloseShopToNextStage()
    {
        if (isWaveActive) return;
        //상점끄기
        StartStage();
    }
}
