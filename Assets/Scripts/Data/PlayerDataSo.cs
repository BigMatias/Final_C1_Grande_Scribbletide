using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Data")]
public class PlayerDataSo : ScriptableObject
{
    [Header("Attack Sounds")]
    public AudioClip swooshAttackSound;
    public AudioClip slashAttackConnected;
    public AudioClip heavySwingAttackSound;
    public AudioClip hammerAttackConnected;

    [Header("Controls")]
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;
    public KeyCode Down = KeyCode.S;
    public KeyCode Up = KeyCode.W;
    public KeyCode NormalAttack = KeyCode.Mouse0;
    public KeyCode Block = KeyCode.Mouse1;
    public KeyCode Dash = KeyCode.Space;

    [Header("Configs")]
    [Header("Movement")]
    public float CurrentSpeed;
    public float BaseSpeed;
    public float ReducedSpeedWhileBlocking;
    public float InvulnerabilityAfterHit;

    [Header("Stamina")]
    public int maxStamina;
    public int currentStamina;
    public int staminaReducedOnAttackBlocked;

    [Header("Dash")]
    public float dashMultiplier;
    public float dashDuration;
    public float dashCooldown;

    [Header("Block")]
    public float staggerDurationOnBlock;

    [Header("Parry")]
    public float staggerDurationOnParry;
    public int staminaRecoveredOnParry;
}