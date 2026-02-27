using System;
using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerDataSo playerDataSO;
    [SerializeField] private PlayerStatsSo playerStatsSO;
    [SerializeField] private WeaponDataSo weaponDataSO;
    [SerializeField] private AudioClip playerHitAudio;
    [SerializeField] private AudioClip playerDeadAudio;
    [SerializeField] private GameObject aim;

    public static event Action onPlayerDied;
    public event Action onExpObtained;

    private bool isDashing;
    private float dashDurationTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;

    private Rigidbody2D rb;
    private CircleCollider2D playerCollider;
    private HealthSystem healthSystem;
    private AudioSource audioSource;

    private WeaponController weaponController;

    private Coroutine powerUpPickedUp;

    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        weaponController = GetComponent<WeaponController>();
        playerCollider = GetComponent<CircleCollider2D>();
        healthSystem.onDie += HealthSystem_onDie;
    }

    private void Start()
    {
        playerDataSO.CurrentSpeed = playerDataSO.BaseSpeed;
    }


    private void Update()
    {
        HandleDashInput();
        CombatActions();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            PerformDash();
        }
        else
        {
            MovePlayer();
        }
    }

    private void OnDestroy()
    {
        //EnemyController.onPlayerHit -= EnemyController_onPlayerHit;
        //PlayerInteractionPowerUps.onPowerUpPickedUp -= PlayerInteractionPowerUps_onPowerUpPickedUp;
        healthSystem.onDie -= HealthSystem_onDie;
    }

    private void OnDisable()
    {
        if (powerUpPickedUp != null)
        {
            StopCoroutine(powerUpPickedUp);
            powerUpPickedUp = null;
        }
    }

    private void HealthSystem_onDie()
    {
        onPlayerDied?.Invoke();
    }

    private void EnemyController_onPlayerHit(Transform enemyTransform)
    {
       // StartCoroutine(WasHit(enemyTransform));
    }

    private void PlayerInteractionPowerUps_onPowerUpPickedUp(int id, float cooldownTime)
    {
        /*switch (id)
        {
            case (int)PlayerActionType.Damage:
                powerUpPickedUp = StartCoroutine(PowerUpPickedUp((int)PlayerActionType.Damage, cooldownTime));
                break;
            case (int)PlayerActionType.TripleJump:
                powerUpPickedUp = StartCoroutine(PowerUpPickedUp((int)PlayerActionType.TripleJump, cooldownTime));
                break;
            case (int)PlayerActionType.Invulnerability:
                powerUpPickedUp = StartCoroutine(PowerUpPickedUp((int)PlayerActionType.Invulnerability, cooldownTime));
                break;
        }*/
    }

    private void MovePlayer()
    {

        Vector2 velocity = rb.velocity;

        // Move left and right
        if (Input.GetKey(playerDataSO.Left))
        {
            velocity.x = -playerDataSO.CurrentSpeed;
        }
        else if (Input.GetKey(playerDataSO.Right))
        {
            velocity.x = playerDataSO.CurrentSpeed;
        }
        else
        {
            velocity.x = 0;
        }

        // Move up and down
        if (Input.GetKey(playerDataSO.Down))
        {
            velocity.y = -playerDataSO.CurrentSpeed;
        }
        else if (Input.GetKey(playerDataSO.Up))
        {
            velocity.y = playerDataSO.CurrentSpeed;
        }
        else
        {
            velocity.y = 0f;
        }

        rb.velocity = velocity;

    }

    private void CombatActions()
    {
        if (Input.GetKeyDown(playerDataSO.NormalAttack))
        {
            weaponController.TryAttack();
        }
        if (Input.GetMouseButton(1))
        {
            weaponController.StartBlock();
        }
        if (Input.GetMouseButtonUp(1) && playerDataSO.currentStamina > 0)
        {
            weaponController.StopBlock();
        }
    }
    private void HandleDashInput()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(playerDataSO.Dash) && dashCooldownTimer <= 0f)
        {
            StartDash();
        }
    }

    private void PerformDash()
    {
        float scaledSpeed = playerDataSO.CurrentSpeed * playerDataSO.dashMultiplier;
        rb.velocity = dashDirection * scaledSpeed;

        dashDurationTimer -= Time.fixedDeltaTime;

        if (dashDurationTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashDurationTimer = playerDataSO.dashDuration;
        dashCooldownTimer = playerDataSO.dashCooldown;

        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        dashDirection = movementInput.normalized;

        if (dashDirection == Vector2.zero)
            dashDirection = -transform.up; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if (collision.gameObject.layer == (int)Layers.Experience)
       {
            if (playerCollider.IsTouching(collision))
            {
                ExperienceGem gem = collision.gameObject.GetComponent<ExperienceGem>();
                playerStatsSO.CurrentExperience += gem.GetValue();
                gem.GemPickedUp();
                onExpObtained?.Invoke();
            }
       }
    }


    /* private IEnumerator WasHit(Transform enemyTransform)
     {
         audioSource.PlayOneShot(playerHitAudio);

         wasHit = true;
         isInvulnerable = true;
         float direction = (transform.position.x - enemyTransform.position.x) >= 0 ? 1f : -1f;
         rb.velocity = Vector2.zero;
         float knockbackForceX = 8f;
         float knockbackForceY = 7f;
         Vector2 knockbackDir = new Vector2(direction * knockbackForceX, knockbackForceY);

         rb.AddForce(knockbackDir, ForceMode2D.Impulse);
         playerState = PlayerState.Hit;
         weaponAnimator.SetInteger(State, (int)playerState);
         //gameObject.layer = (int)Layers.PlayerInvulnerable;
         yield return new WaitForSeconds(0.3f);
         wasHit = false;
         yield return new WaitForSeconds(playerDataSO.InvulnerabilityAfterHit);
         isInvulnerable = false;
         gameObject.layer = (int)Layers.Player;

    }
    */
}
