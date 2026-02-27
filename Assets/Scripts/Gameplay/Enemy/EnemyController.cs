using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyDataSo enemyDataSO;
    [SerializeField] private PlayerStatsSo playerStatsSo;
    [SerializeField] private WeaponDataSo weaponDataSO;
    [SerializeField] private ParticleSystem enemyDeathParticles;
    [SerializeField] private EnemyWeaponController enemyWeaponController;
    [SerializeField] private ExperienceType baseExperienceType;
    [SerializeField] public EnemyType enemyType;

    [Header("Configs")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float obstacleCheckDistance = 0.8f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float avoidDuration = 0.5f;

    private GameObject player;
    private Transform playerTransform;

    private float cooldownTimer;
    private float staggerTimer;
    private bool isStaggered;
    private bool isAvoiding;
    private Vector2 avoidDirection;
    private float avoidTimer;

    public event Action onEnemyHit;
    public static event Action onPlayerHit;
    public static event Action onEnemyDie;

    private Rigidbody2D rb;
    private HealthSystem healthSystem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.onDamageDealt += HealthSystem_onDamageDealt;
        healthSystem.onDie += HealthSystem_onDie;
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerTransform = player.transform;
    }


    private void Update()
    {
        Attack();
        if (isStaggered)
        {
            staggerTimer -= Time.deltaTime;
            if (staggerTimer <= 0)
            {
                isStaggered = false;
            }
        }
    }

    private void FixedUpdate()
    {
        SeparateFromOtherEnemies();
        TrackPlayer();
    }

    private void OnDestroy()
    {
        healthSystem.onDamageDealt -= HealthSystem_onDamageDealt;
        healthSystem.onDie -= HealthSystem_onDie;
    }

    private void HealthSystem_onDamageDealt()
    {
        onEnemyHit?.Invoke();

    }

    private void HealthSystem_onDie()
    {
        SpawnExperience();
        Instantiate(enemyDeathParticles, transform.position, Quaternion.identity);
        if (HordeSpawner.Instance != null)
        {
            HordeSpawner.Instance.ReturnToPool(gameObject, enemyType);
        }
        onEnemyDie?.Invoke();

    }

    private void Attack()
    {
        if (isStaggered) return;

        float distance = Vector2.Distance(transform.position, playerTransform.position);
    
        switch (enemyType)
        {
            case EnemyType.EnemySword:
                {
                    if (distance <= enemyDataSO.ESAttackRange)
                    {
                        if (cooldownTimer > 0f) return;

                        enemyWeaponController.TryAttack();
                        cooldownTimer = enemyDataSO.attackCooldown;
                    }
                    break;
                }
            case EnemyType.EnemyHammer:
                {
                    if (distance <= enemyDataSO.EHAttackRange)
                    {
                        if (cooldownTimer > 0f) return;

                        enemyWeaponController.TryAttack();
                        cooldownTimer = enemyDataSO.attackCooldown;
                    }
                    break;
                }
            case EnemyType.EnemySwordElite:
                {
                    if (distance <= enemyDataSO.ESEliteAttackRange)
                    {
                        if (cooldownTimer > 0f) return;

                        enemyWeaponController.TryAttack();
                        cooldownTimer = enemyDataSO.attackCooldown;
                    }
                    break;
                }
            case EnemyType.EnemyHammerElite:
                {
                    if (distance <= enemyDataSO.EHEliteAttackRange)
                    {
                        if (cooldownTimer > 0f) return;

                        enemyWeaponController.TryAttack();
                        cooldownTimer = enemyDataSO.attackCooldown;
                    }
                    break;
                }
        }
        cooldownTimer -= Time.deltaTime;
    }

    public void Stagger(float duration, string staggerType)
    {
        if (enemyType == EnemyType.EnemyHammerElite && staggerType != "Block" || enemyType == EnemyType.EnemySwordElite && staggerType != "Block") return;
            ApplyKnockback(playerTransform.position);
            staggerTimer = duration;
            isStaggered = true;
            enemyWeaponController.StaggerAnim(staggerType);
        
    }

    private void ApplyKnockback(Vector2 hitSourcePosition)
    {
        Vector2 direction = ((Vector2)transform.position - hitSourcePosition).normalized;

        rb.velocity = Vector2.zero; 
        rb.AddForce(direction * weaponDataSO.WeapopnKnockbackForce, ForceMode2D.Impulse);
    }

    private void TrackPlayer()
    {
        if (player == null || isStaggered) return;

        Vector2 dir = playerTransform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle - 90f);

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        switch (enemyType)
        {
            case EnemyType.EnemySword:
                {
                    if (distance >= enemyDataSO.ESAttackRange)
                    {
                        StartTracking();
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                    break;
                }
            case EnemyType.EnemyHammer:
                {
                    if (distance >= enemyDataSO.EHAttackRange)
                    {
                        StartTracking();
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                    break;
                }
            case EnemyType.EnemySwordElite:
                {
                    if (distance >= enemyDataSO.ESAttackRange)
                    {
                        StartTracking();
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                    break;
                }
            case EnemyType.EnemyHammerElite:
                {
                    if (distance >= enemyDataSO.EHAttackRange)
                    {
                        StartTracking();
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                    }
                    break;
                }
        }
    }

    private void StartTracking()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (isAvoiding)
        {
            rb.velocity = avoidDirection * speed;
            avoidTimer -= Time.fixedDeltaTime;

            if (avoidTimer <= 0f)
                isAvoiding = false;

            return;
        }

        float radius = 0.25f;

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position,
            radius,
            direction,
            obstacleCheckDistance,
            obstacleLayer
        );

        if (hit.collider == null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            // modo rodear
            isAvoiding = true;
            avoidTimer = avoidDuration;

            avoidDirection = Vector2.Perpendicular(direction);

            // lado más libre
            RaycastHit2D sideHit = Physics2D.CircleCast(
                transform.position,
                radius,
                avoidDirection,
                obstacleCheckDistance,
                obstacleLayer
            );

            if (sideHit.collider != null)
                avoidDirection = -avoidDirection;

            rb.velocity = avoidDirection * speed;
        }
    }

    private void SpawnExperience()
    {
        ExperienceType type = GetExperienceType();

        ExperiencePool.Instance.GetExperience(type, transform.position);
    }

    private ExperienceType GetExperienceType()
    {
        if (enemyType == EnemyType.EnemyHammer || enemyType == EnemyType.EnemySword)
            return ExperienceType.Blue;

        if (enemyType == EnemyType.EnemyHammerElite || enemyType == EnemyType.EnemySwordElite)
            return ExperienceType.Yellow;
        
        return ExperienceType.Red;
    }

    private void SeparateFromOtherEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.6f);

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == (int)Layers.Enemy)
            {
                Vector2 direction = (transform.position - hit.transform.position);
                float distance = direction.magnitude;

                if (distance > 0f)
                {
                    Vector2 push = direction.normalized * 0.02f;
                    transform.position += (Vector3)push;
                }
            }
        }
    }
}
