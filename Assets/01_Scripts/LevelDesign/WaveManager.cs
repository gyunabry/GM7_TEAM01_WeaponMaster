using UnityEngine;
using System.Collections.Generic;
using System;

public enum Difficulty { Normal, Hard, Hell}
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public event Action<int> OnWaveStarted;
    public event Action<float> OnTimeChanged;
    public event Action OnShopOpened;

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

    public int CurrentWave => currentWaveIndex + 1;
    public float WaveTime => stageTime;

    public bool WaveActive => isWaveActive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(GameSceneData.SelectedStage != null && GameSceneData.SelectedStage.waveDataList != null)
        {
            this.stageWaves = GameSceneData.SelectedStage.waveDataList;
        }
        this.currentDifficulty = GameSceneData.SelectedDifficulty;

        StartStage();
        
    }

    public void StartStage()
    {
        stageTime = 0f;
        currentWaveIndex = 0;
        isWaveActive = true;

        StartWave(currentWaveIndex);
    }
    private void Update()
    {
        if (!isWaveActive) return;
        stageTime += Time.deltaTime;

        OnTimeChanged?.Invoke(stageTime);

        CheckWaveTimeLine();
    }

    public void CheckWaveTimeLine()
    {
        if(currentWaveIndex<0 || currentWaveIndex >= stageWaves.Count) return;
        WaveData currentWave = stageWaves[currentWaveIndex];
        if(stageTime>=currentWave.waveDuration)
        {
            currentWaveIndex++;
            if(currentWaveIndex < stageWaves.Count)
            {
                stageTime = 0f;
                StartWave(currentWaveIndex);
            }
            else
            {
                isWaveActive = false;

                OnShopOpened?.Invoke();
            }
        }
    }

    private void StartWave(int index)
    {
        if(stageWaves==null ||  stageWaves.Count==0 || index>=stageWaves.Count)
        {
            return;
        }
        
        EnemySpawner.Instance.SetupNextWave(stageWaves[index]);

        OnWaveStarted?.Invoke(CurrentWave);
    }
}
