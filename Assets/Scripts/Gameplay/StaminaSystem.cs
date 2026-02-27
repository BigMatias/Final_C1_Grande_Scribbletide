using System;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    [SerializeField] private PlayerDataSo playerDataSO;
    [SerializeField] private WeaponDataSo weaponDataSO;
    [SerializeField] private WeaponController weaponController;

    private bool isPlayerBlocking;
    private float timerForNextStamina = 0f;
    private float timeBetweenStaminaRecharge = 0.5f;
    private float timerBeginStaminaRegen = 1f;
    private float weaponSpeedSave;
    private float timerStaminaDepleted = 2f;
    private bool isStaminaDepleted;

    public static event Action onStaminaChanged;
    public static event Action onStaminaDepleted;

    private void Awake()
    {
        weaponController = GetComponent<WeaponController>();
        weaponController.onPlayerIsNotBlocking += WeaponController_onPlayerIsNotBlocking;
        weaponController.onPlayerIsBlocking += WeaponController_onPlayerIsBlocking;
    }

    private void Start()
    {
        playerDataSO.currentStamina = playerDataSO.maxStamina;
    }

    private void Update()
    {
        TimerForStaminaRecover();
    }
    private void WeaponController_onPlayerIsBlocking(int obj)
    {
        isPlayerBlocking = true;
    }

    private void WeaponController_onPlayerIsNotBlocking()
    {
        isPlayerBlocking = false;
        timerBeginStaminaRegen = 1f;
    }

    public void ReduceStamina(int amount)
    {
        if (playerDataSO.currentStamina > 0)
        {
            playerDataSO.currentStamina -= amount;
            onStaminaChanged?.Invoke();
        }
        if (playerDataSO.currentStamina == 0)
        {
            timerBeginStaminaRegen = timerStaminaDepleted;
            weaponSpeedSave = weaponDataSO.WeaponCurrentSpeed;
            weaponDataSO.WeaponCurrentSpeed = (weaponDataSO.WeaponCurrentSpeed * weaponDataSO.StaminaDepletedSpeedReduced) / 100;
            isStaminaDepleted = true;
            onStaminaDepleted?.Invoke();
        }
    }

    public void RecoverStamina(int amount)
    {
        if (playerDataSO.currentStamina < playerDataSO.maxStamina)
        {
            playerDataSO.currentStamina += amount;
            onStaminaChanged?.Invoke();
        }
    }

    private void TimerForStaminaRecover()
    {
        timerBeginStaminaRegen -= Time.deltaTime;

        if (!isPlayerBlocking && playerDataSO.currentStamina < playerDataSO.maxStamina && timerBeginStaminaRegen <= 0f)
        {
            timerForNextStamina -= Time.deltaTime;
            if (timerForNextStamina <= 0f)
            {
                if (isStaminaDepleted)
                {
                    weaponDataSO.WeaponCurrentSpeed = weaponSpeedSave;
                    isStaminaDepleted = false;
                }
                playerDataSO.currentStamina ++;
                onStaminaChanged?.Invoke();
                timerForNextStamina = timeBetweenStaminaRecharge;
            }
        }


    }
}
