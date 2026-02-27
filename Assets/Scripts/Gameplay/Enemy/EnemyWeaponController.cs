using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    [SerializeField] private EnemyDataSo enemyDataSO;
    [SerializeField] private WeaponDataSo weaponDataSo;
    [SerializeField] private Transform player;
    [SerializeField] private EnemyType enemyType;

    private bool isAttacking = false;
    private bool isBlocking = false;
    private int comboIndex = 0;
    private bool attackBuffered = false;
    private float bufferTime = 0.2f;
    private float bufferCounter = 0f;

    private WeaponState tempWeaponState;

    private Animator weaponAnimator;
    private WeaponsEvents weaponEvents;

    private HashSet<HealthSystem> playerHitThisAttack = new HashSet<HealthSystem>();

    private static readonly int State = Animator.StringToHash("State");
    [SerializeField] private WeaponState weaponState = WeaponState.Idle;

    private void Awake()
    {
        weaponEvents = GetComponent<WeaponsEvents>();
        weaponAnimator = GetComponent<Animator>();
        weaponEvents.onAttackFinished += WeaponsEvents_onAttackFinished;
        weaponEvents.onAnimationFinished += WeaponsEvents_onAnimationFinished;
        weaponEvents.onAttackStarted += WeaponEvents_onAttackStarted;
    }

    void Start()
    {
        tempWeaponState = WeaponState.StaggerHit2;
        enemyDataSO.EHCurrentDamage = enemyDataSO.EHBaseDamage;
        enemyDataSO.EHCurrentSpeed  = enemyDataSO.EHBaseSpeed;
        enemyDataSO.ESCurrentDamage = enemyDataSO.ESBaseDamage;
        enemyDataSO.ESCurrentSpeed  = enemyDataSO.ESBaseSpeed;

        enemyDataSO.EHEliteCurrentDamage = enemyDataSO.EHEliteBaseDamage;
        enemyDataSO.EHEliteCurrentSpeed = enemyDataSO.EHEliteBaseSpeed;
        enemyDataSO.ESEliteCurrentDamage = enemyDataSO.ESEliteBaseDamage;
        enemyDataSO.ESEliteCurrentSpeed = enemyDataSO.ESEliteBaseSpeed;
        weaponAnimator.SetInteger(State, (int)weaponState);
    }

    void Update()
    {
        if (attackBuffered)
        {
            bufferCounter -= Time.deltaTime;

            if (bufferCounter <= 0f)
            {
                attackBuffered = false;
            }
        }
    }

    private void OnDestroy()
    {
        weaponEvents.onAttackFinished -= WeaponsEvents_onAttackFinished;
        weaponEvents.onAnimationFinished -= WeaponsEvents_onAnimationFinished;
        weaponEvents.onAttackStarted -= WeaponEvents_onAttackStarted;
    }

    private void WeaponsEvents_onAnimationFinished()
    {
        if (isAttacking) return;
        weaponState = WeaponState.Idle;
        weaponAnimator.SetInteger(State, (int)weaponState);
        StartCoroutine(ResetCombo());
    }

    private void WeaponsEvents_onAttackFinished()
    {
        isAttacking = false;
        if (attackBuffered)
        {
            attackBuffered = false;
            StartAttack();
        }
    }

    private void WeaponEvents_onAttackStarted()
    {
        playerHitThisAttack.Clear();
    }
    public void TryAttack()
    {
        if (!isAttacking && !isBlocking)
        {
            StartAttack();
        }
        else
        {
            attackBuffered = true;
            bufferCounter = bufferTime;
        }

    }

    private void StartAttack()
    {
        comboIndex++;
        if (comboIndex > weaponDataSo.maxCombo)
            comboIndex = 1;
        isAttacking = true;

        switch (enemyType)
        {
            case EnemyType.EnemySword:
                {
                    weaponAnimator.SetFloat("AttackSpeed", enemyDataSO.ESCurrentSpeed);
                    break;
                }
            case EnemyType.EnemyHammer:
                {
                    weaponAnimator.SetFloat("AttackSpeed", enemyDataSO.EHCurrentSpeed);
                    break;
                }
            case EnemyType.EnemySwordElite:
                {
                    weaponAnimator.SetFloat("AttackSpeed", enemyDataSO.ESEliteCurrentSpeed);
                    break;
                }
            case EnemyType.EnemyHammerElite:
                {
                    weaponAnimator.SetFloat("AttackSpeed", enemyDataSO.EHEliteCurrentSpeed);
                    break;
                }
        }
        weaponAnimator.SetInteger(State, comboIndex);
    }

    public void Block()
    {
        if (weaponState == WeaponState.Block) return;
        isBlocking = true;
        isAttacking = false;
        comboIndex = 0;
        weaponState = WeaponState.Block;
        weaponAnimator.SetInteger(State, (int)weaponState);
    }

    public void ReleaseBlock()
    {
        isBlocking = false;
        weaponState = WeaponState.BlockRelease;
        weaponAnimator.SetInteger(State, (int)weaponState);
    }

    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(weaponDataSo.timeForComboReset);
        comboIndex = 0;
    }

    public void StaggerAnim(string staggerType)
    {
        switch (staggerType)
        {
            case "Block":
                {
                    isAttacking = false;
                    weaponState = WeaponState.StaggerBlock;
                    weaponAnimator.SetInteger(State, (int)weaponState);
                    break;
                }
            case "Attack":
                {
                    isAttacking = false;
                    if (tempWeaponState == WeaponState.StaggerHit2)
                    {
                        weaponState = WeaponState.StaggerHit1;
                    }
                    else if (tempWeaponState == WeaponState.StaggerHit1)
                    {
                        weaponState = WeaponState.StaggerHit2;
                    }
                    weaponAnimator.SetInteger(State, (int)weaponState);
                    tempWeaponState = weaponState;
                    break;
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out HealthSystem healthSystem) && other.gameObject.layer == (int)Layers.Player)
        {
            if (playerHitThisAttack.Contains(healthSystem))
                return;

            playerHitThisAttack.Add(healthSystem);
            switch (enemyType)
            {
                case EnemyType.EnemySword:
                    {
                        healthSystem.DoDamage(enemyDataSO.ESCurrentDamage);
                        break;
                    }
                case EnemyType.EnemyHammer:
                    {
                        healthSystem.DoDamage(enemyDataSO.EHCurrentDamage);
                        break;
                    }
                case EnemyType.EnemySwordElite:
                    {
                        healthSystem.DoDamage(enemyDataSO.ESEliteCurrentDamage);
                        break;
                    }
                case EnemyType.EnemyHammerElite:
                    {
                        healthSystem.DoDamage(enemyDataSO.EHEliteCurrentDamage);
                        break;
                    }
            }
        }
    }   
}
