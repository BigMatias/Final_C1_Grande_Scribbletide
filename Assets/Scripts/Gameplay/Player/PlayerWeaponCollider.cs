using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponCollider : MonoBehaviour
{
    [SerializeField] private WeaponDataSo weaponDataSo;
    [SerializeField] private WeaponType weaponType;

    public static event Action onAttackBlocked;
    public static event Action onEnemyHit;

    private WeaponsEvents weaponEvents;
    private WeaponController weaponController;
    private bool isBlocking;

    private HashSet<HealthSystem> enemiesHitThisAttack = new HashSet<HealthSystem>();

    private void Awake()
    {
        weaponEvents = GetComponent<WeaponsEvents>();
        weaponController = GetComponentInParent<WeaponController>();
        weaponController.onPlayerIsBlocking += WeaponController_onPlayerIsBlocking;
        weaponController.onPlayerIsNotBlocking += WeaponController_onPlayerIsNotBlocking;
        weaponEvents.onAttackStarted += WeaponEvents_onAttackStarted;
    }

    private void OnDestroy()
    {
        weaponController.onPlayerIsBlocking -= WeaponController_onPlayerIsBlocking;
        weaponController.onPlayerIsNotBlocking -= WeaponController_onPlayerIsNotBlocking;
        weaponEvents.onAttackStarted -= WeaponEvents_onAttackStarted;
    }

    private void WeaponController_onPlayerIsBlocking(int obj)
    {
        isBlocking = true;
    }

    private void WeaponController_onPlayerIsNotBlocking()
    {
        isBlocking = false;
    }

    private void WeaponEvents_onAttackStarted()
    {
        enemiesHitThisAttack.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isBlocking)
        {
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();

            if (collision.gameObject.layer == (int)Layers.Enemy && collision.TryGetComponent(out HealthSystem healthSystem))
            {
                if (enemiesHitThisAttack.Contains(healthSystem))
                    return;

                onEnemyHit?.Invoke();
                enemiesHitThisAttack.Add(healthSystem);
                if (collision.gameObject.layer == (int)Layers.Enemy)
                {
                    healthSystem.DoDamage(weaponDataSo.WeaponCurrentDamage);
                    enemyController.Stagger(weaponDataSo.WeaponStaggerTime, "Attack");
                }
            }
        }
        else //Bloqueo ataque
        {
            if (collision.CompareTag("EnemyWeapon"))
            {
                onAttackBlocked?.Invoke();
                weaponController.AttackBlocked(collision.gameObject);
            }
        }
    }

}
