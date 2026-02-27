using UnityEngine;

public class EnemySword : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.2f;

    private float cooldownTimer;
    private EnemyWeaponController weapon;

    private void Awake()
    {
        weapon = GetComponent<EnemyWeaponController>();
    }

    private void Update()
    {
       /* if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            LookAtPlayer();

            if (distance > attackRange)
            {
                MoveTowardsPlayer();
            }
            else
            {
                TryAttack();
            }
        }

        cooldownTimer -= Time.deltaTime;*/
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * 2f * Time.deltaTime);
    }


}
