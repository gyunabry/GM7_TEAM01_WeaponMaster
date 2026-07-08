using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BossPhaseInfo
{
    [Range(0, 1)]
    [Tooltip("해당 페이즈가 시작되는 HP 비율")]
    public float hpThreshold;

    [Tooltip("해당 페이즈에서 사용할 공격 패턴 목록")]
    public List<EnemyPatternData> phasePatterns;
}

[CreateAssetMenu(fileName = "New Boss Data", menuName = "Scriptable Objects/BossData")]
public class BossData : ScriptableObject
{
    [Header("기본 정보")]
    public string bossId;
    public string bossName;

    [Header("애니메이션 설정")]
    public RuntimeAnimatorController runtimeAnimator;

    [Header("기본 스탯")]
    public float maxHp;
    public float moveSpeed;
    public float damage = 10f;

    [Header("페이즈 구성")]
    public List<BossPhaseInfo> phases;
}
