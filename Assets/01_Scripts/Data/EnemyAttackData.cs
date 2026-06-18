using UnityEngine;

public enum AttackType
{
    Melee,
    Dash,
    Range,
}

public enum BulletPattern
{
    Straight,
    Cone,
    Circle,
    Orbit
}

[CreateAssetMenu(fileName = "Enemy Attack Data", menuName = "GamePlay/EnemyAttackData")]
public class EnemyAttackData : ScriptableObject
{
    [Header("기본 공격 설정")]
    public AttackType attackType;
    public int attackDamage;
    public float attackCooltime;

    [Header("대쉬 공격 설정")]
    public float dashSpeed;
    public float dashRange;

    [Header("원거리 공격 설정")]
    public BulletPattern bulletPattern;
    public EnemyBullet projectilePrefab;
    public float projectileSpeed;
    [Tooltip("투사체 개수")]
    public int projectileCount;
    [Tooltip("투사체 발사 각도")]
    public float spreadAngle;

    [Header("궤도 공격")]
    [Tooltip("궤도 반지름")]
    public float orbitRadius;
    [Tooltip("궤도 회전 속도")]
    public float orbitSpeed;
}
