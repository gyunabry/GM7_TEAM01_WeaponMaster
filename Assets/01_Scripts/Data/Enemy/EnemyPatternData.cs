using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPatternData", menuName = "GamePlay/Enemy Pattern Data")]
public class EnemyPatternData : ScriptableObject
{
    [Header("패턴 기본 설정")]
    public float triggerRange = 5f;
    public float patternCooltime = 5f;

    [Header("공격 시퀀스")]
    [Tooltip("각 EnemyAttackData를 넣어 콤보 패턴 구현 가능")]
    public List<EnemyAttackData> attackSequence;

    [Tooltip(("공격 패턴 간 지연시간"))]
    public float actionDelay = 0.5f;
}
