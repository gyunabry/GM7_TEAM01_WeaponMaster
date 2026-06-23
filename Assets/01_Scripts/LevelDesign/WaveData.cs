using UnityEngine;
using System;

[System.Serializable]
public struct SpawnInfo
{
    public EnemyData enemyData; // 적 프리팹
    public int spawnCount; //스폰되는 적 수
    public float spawnInterval; //스폰 간격
    public float spawnDelay; //다음 몹 소환 딜레이

}
[CreateAssetMenu(fileName ="WaveData", menuName = "ScriptableObject/WaveData")]
public class WaveData : ScriptableObject
{
    [Header("웨이브 시간구조")]
    public float waveStartTiming; //라운드 시작 후 웨이브가 시작될 시간
    public float waveDuration; //웨이브 지속시간

    [Header("스폰할 적 배열")]
    public SpawnInfo[] spawnList; //웨이브에 들어갈 적 정보
}