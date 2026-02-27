using UnityEngine;

[CreateAssetMenu(fileName = "EnemySettings", menuName = "Enemy/Data")]

public class EnemyDataSo : ScriptableObject
{
    [Header("General Configs")]
    public AudioClip EnemyDeadAudio;
    public EnemyType enemyType;
    public float attackCooldown;

    [Header("Enemy Sword Configs")]
    public float ESSpeed;
    public float ESBaseSpeed;
    public float ESCurrentSpeed;
    public float ESBaseDamage;
    public float ESCurrentDamage;
    public float ESKnockback;
    public float ESAttackRange;

    [Header("Enemy Sword Elite Configs")]
    public float ESEliteSpeed;
    public float ESEliteBaseSpeed;
    public float ESEliteCurrentSpeed;
    public float ESEliteBaseDamage;
    public float ESEliteCurrentDamage;
    public float ESEliteKnockback;
    public float ESEliteAttackRange;

    [Header("Enemy Hammer Configs")]
    public float EHSpeed;
    public float EHBaseSpeed;
    public float EHCurrentSpeed;
    public float EHBaseDamage;
    public float EHCurrentDamage;
    public float EHKnockback;
    public float EHAttackRange;

    [Header("Enemy Hammer Elite Configs")]
    public float EHEliteSpeed;
    public float EHEliteBaseSpeed;
    public float EHEliteCurrentSpeed;
    public float EHEliteBaseDamage;
    public float EHEliteCurrentDamage;
    public float EHEliteKnockback;
    public float EHEliteAttackRange;
}
