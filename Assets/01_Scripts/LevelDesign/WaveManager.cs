using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public enum Difficulty { Normal, Hard, Hell }
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("이벤트")]
    [SerializeField] private VoidEventChannel bossEncounterEvent;

    public event Action<int> OnWaveStarted;
    public event Action<float> OnTimeChanged;
    public event Action OnBossWarningStarted; // 보스 등장 대기 이벤트

    [Header("난이도 설정")]
    [SerializeField] private Difficulty currentDifficulty = Difficulty.Normal;

    [Header("웨이브 데이터")]
    [SerializeField] private List<WaveData> stageWaves;

    [Header("보스 연출 시간")]
    [SerializeField] private float bossWaitTime = 3f;

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
        if (GameSceneData.SelectedStage != null && GameSceneData.SelectedStage.waveDataList != null)
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
        if (currentWaveIndex<0 || currentWaveIndex >= stageWaves.Count) return;

        WaveData currentWave = stageWaves[currentWaveIndex];
        if (stageTime >= currentWave.waveDuration)
        {
            currentWaveIndex++;
            if (currentWaveIndex < stageWaves.Count)
            {
                stageTime = 0f;
                StartWave(currentWaveIndex);
            }
            else // 모든 웨이브가 끝나면 보스 등장
            {
                isWaveActive = false;

                StartCoroutine(BossEncounterCo());
            }
        }
    }

    private void StartWave(int index)
    {
        if (stageWaves==null || stageWaves.Count==0 || index>=stageWaves.Count)
        {
            return;
        }
        
        EnemySpawner.Instance.SetupNextWave(stageWaves[index]);

        OnWaveStarted?.Invoke(CurrentWave);
    }

    private void UnlockWeapons(int clearedIndex)
    {
        if (stageWaves == null || stageWaves.Count == 0 || clearedIndex >= stageWaves.Count) return;

        if(clearedIndex == 4)
        {
            //웨폰매니저 가져오기
        }

    }

    // 잠깐의 대기 시간 후 보스 이벤트 발행
    private IEnumerator BossEncounterCo()
    {
        // 대기 시간
        // TODO: 보스 등장 연출
        OnBossWarningStarted?.Invoke();

        yield return new WaitForSeconds(bossWaitTime);

        if (bossEncounterEvent != null)
        {
            bossEncounterEvent.RaiseEvent();
        }
    }
}
