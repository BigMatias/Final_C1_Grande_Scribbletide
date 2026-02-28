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
        playerDataSO.SpeedSave = playerDataSO.CurrentSpeed;
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
       if (collision.gameObject.layer == (int)Layers.Pickable)
       {
            if (playerCollider.IsTouching(collision))
            {
                Pickable pickable = collision.gameObject.GetComponent<Pickable>();
                if (pickable.Type == PickableType.HealItem)
                {
                    int healAmount = pickable.GetValue();
                    healthSystem.Heal(healAmount);
                    pickable.PickableCollected();
                }
                else
                {
                    playerStatsSO.CurrentExperience += pickable.GetValue();
                    pickable.PickableCollected();
                    onExpObtained?.Invoke();
                }
            }
       }
    }
}
