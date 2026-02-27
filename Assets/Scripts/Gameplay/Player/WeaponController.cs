using System;
using System.Collections;
using CodeMonkey.Utils;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponDataSo weaponDataSo;
    [SerializeField] private PlayerDataSo playerDataSo;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private StaminaSystem staminaSystem;
    [SerializeField] private WeaponState weaponState = WeaponState.Idle;
    [SerializeField] private WeaponType weaponType;

    [Header("SoundClips")]
    [SerializeField] private AudioClip playerHit; 
    [SerializeField] private AudioClip playerDead; 

    public event Action <int> onPlayerIsBlocking;
    public event Action onPlayerIsNotBlocking;

    private GameObject chosenWeapon;
    private Transform aimTransform;
    private Animator weaponAnimator;
    private WeaponsEvents weaponEvents;

    private bool isAttacking = false;
    private bool isPlayerBlocking = false;
    private bool attackBuffered = false;
    private float bufferCounter = 0f;
    private bool canParry = false;

    private string weaponId;
    private float attackSpeedModifier = 1f;
    private float AttackSpeed => weaponDataSo.WeaponCurrentSpeed * attackSpeedModifier;

    private Coroutine resetComboCoroutine;
    private float parryTimer;
    private static readonly int State = Animator.StringToHash("State");

    private void Awake()
    {
        GetPlayerWeapon();
        weaponAnimator = chosenWeapon.GetComponent<Animator>();
        weaponEvents = chosenWeapon.GetComponent<WeaponsEvents>();
        weaponEvents.onAttackFinished += WeaponsEvents_onAttackFinished;
        weaponEvents.onAnimationFinished += WeaponsEvents_onAnimationFinished;
        StaminaSystem.onStaminaChanged += StaminaSystem_onStaminaChanged;
    }

    void Start()
    {
        weaponDataSo.ComboIndex = 0;
        aimTransform = transform.Find("Aim");
        weaponAnimator.SetInteger(State, (int)weaponState);
    }

    void Update()
    {
        HandleAiming();
        Timers();
    }

    private void OnDestroy()
    {
        weaponEvents.onAttackFinished -= WeaponsEvents_onAttackFinished;
        weaponEvents.onAnimationFinished -= WeaponsEvents_onAnimationFinished;
    }

    private void OnDisable()
    {
        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
            resetComboCoroutine = null;
        }
    }
    private void StaminaSystem_onStaminaChanged()
    {
        if (isPlayerBlocking && playerDataSo.currentStamina <= 0)
        {
            StopBlock();
        }   
    }

    private void WeaponsEvents_onAnimationFinished()
    {
        if (isAttacking) return;
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

    private void GetPlayerWeapon()
    {
        weaponId = weaponDataSo.weaponId;
        switch (weaponId)
        {
            case "Longsword":
                chosenWeapon = Instantiate(weaponDataSo.weaponPrefabs[0], weaponPivot);
                weaponDataSo.WeaponCurrentDamage = weaponDataSo.LongswordBaseDamage;
                weaponDataSo.WeaponCurrentSpeed = weaponDataSo.LongswordBaseSpeed;
                weaponDataSo.WeaponStaggerTime = weaponDataSo.LongswordStaggerTime;
                weaponDataSo.WeapopnKnockbackForce = weaponDataSo.LongswordKnockback;
                weaponDataSo.WeaponSwingSound = weaponDataSo.LongswordSwingSound;
                weaponDataSo.WeaponHitSound = weaponDataSo.LongswordHitSound;
                break;
            case "Hammer":
                chosenWeapon = Instantiate(weaponDataSo.weaponPrefabs[1], weaponPivot);
                weaponDataSo.WeaponCurrentDamage = weaponDataSo.HammerBaseDamage;
                weaponDataSo.WeaponCurrentSpeed = weaponDataSo.HammerBaseSpeed;
                weaponDataSo.WeaponStaggerTime = weaponDataSo.HammerStaggerTime;
                weaponDataSo.WeapopnKnockbackForce = weaponDataSo.HammerKnockback;
                weaponDataSo.WeaponSwingSound = weaponDataSo.HammerSwingSound;
                weaponDataSo.WeaponHitSound = weaponDataSo.HammerHitSound;
                break;
        }
        // (crear flechas para arco en caso de que el arco sea elegido) CreateBulletPool();
    }

    private void HandleAiming()
    {
        if (Time.timeScale == 1)
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

            Vector3 aimDirection = (mousePosition - transform.position).normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            aimTransform.eulerAngles = new Vector3(0f, 0f, angle - 90f);
        }
    }

    private void Timers()
    {
        if (attackBuffered)
        {
            bufferCounter -= Time.deltaTime;

            if (bufferCounter <= 0f)
            {
                attackBuffered = false;
            }
        }

        // Parry
        if (canParry)
        {
            parryTimer -= Time.deltaTime;
            if (parryTimer <= 0f)
            {
                canParry = false;
            }
        }
    }

    public void TryAttack()
    {
        if (!isAttacking && !isPlayerBlocking)
        {
            StartAttack();
        }
        else
        {
            attackBuffered = true;
            bufferCounter = weaponDataSo.bufferTime;
        }
    }
        
    private void StartAttack()
    {
        weaponDataSo.ComboIndex++;
        if (weaponDataSo.ComboIndex > weaponDataSo.maxCombo)
            weaponDataSo.ComboIndex = 1;
        isAttacking = true;
        audioManager.ReproduceClip(weaponDataSo.WeaponSwingSound);
        weaponAnimator.SetFloat("AttackSpeed", AttackSpeed);
        weaponAnimator.SetInteger(State, weaponDataSo.ComboIndex);
    }

    public void AttackBlocked(GameObject enemy)
    {
        if (isPlayerBlocking)
        {
            EnemyController enemyController = enemy.GetComponentInParent<EnemyController>();
            enemyController.Stagger(playerDataSo.staggerDurationOnBlock, "Block");

            if (canParry)
            {
                staminaSystem.RecoverStamina(playerDataSo.staminaRecoveredOnParry);
            
                if (enemyController != null)
                {
                    enemyController.Stagger(playerDataSo.staggerDurationOnParry, "Block");
                }
            }
            else
            {
                staminaSystem.ReduceStamina(playerDataSo.staminaReducedOnAttackBlocked);
            }
        }
    }

    public void StartBlock()
    {
        if (weaponState != WeaponState.Block)
        {
            if (playerDataSo.currentStamina > 0)
            {
                onPlayerIsBlocking?.Invoke(playerDataSo.currentStamina);
            
                isPlayerBlocking = true;
                isAttacking = false;
                canParry = true;
                parryTimer = weaponDataSo.parryWindow;

                float reducedSpeed = playerDataSo.BaseSpeed * (1 - playerDataSo.ReducedSpeedWhileBlocking / 100);
                playerDataSo.CurrentSpeed = reducedSpeed;
                weaponDataSo.ComboIndex = 0;
                weaponState = WeaponState.Block;
                weaponAnimator.SetInteger(State, (int)weaponState);
            
            }
        }
    }

    public void StopBlock()
    {
        onPlayerIsNotBlocking?.Invoke();
        isPlayerBlocking = false;
        playerDataSo.CurrentSpeed = playerDataSo.BaseSpeed;
        weaponState = WeaponState.BlockRelease;
        weaponAnimator.SetInteger(State, (int)weaponState);
    }


    private IEnumerator ResetCombo()
    {
        yield return new WaitForSeconds(weaponDataSo.timeForComboReset);
        if (!isAttacking)
        {
            weaponState = WeaponState.Idle;
            weaponAnimator.SetInteger(State, (int)weaponState);
            weaponDataSo.ComboIndex = 0;
        }
    }

    public void ModifyAttackSpeed(float amount)
    {
        attackSpeedModifier = amount;
    }

}
