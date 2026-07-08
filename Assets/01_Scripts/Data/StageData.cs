using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage Data", menuName = "Stage/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("스테이지 정보")]
    public string stageName;
    public GameObject mapPrefab;

    [Header("웨이브 데이터")]
    public List<WaveData> waveDataList;

    [Header("스테이지 보스 데이터")]
    public BossData bossData;
}
