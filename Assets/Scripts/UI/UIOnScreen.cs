using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOnScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image[] staminasEmpty;
    [SerializeField] private Image[] staminasFull;
    [SerializeField] private WeaponController playerWeaponController;
    [SerializeField] private PlayerDataSo playerDataSo;
    [SerializeField] private PlayerStatsSo playerStatsSo;
    [SerializeField] private GameDataSo gameDataSO;
    [SerializeField] private TextMeshProUGUI uiGameTimer;
    [SerializeField] private TextMeshProUGUI uiPlayerLevel;
    [SerializeField] private float hideStaminaTime;
    [SerializeField] private HealthSystem playerHealthSystem;

    private float timerForHidingStamina;

    private void Awake()
    {
        playerWeaponController.onPlayerIsBlocking += PlayerWeaponController_onPlayerIsBlocking; 
        playerWeaponController.onPlayerIsNotBlocking += PlayerWeaponController_onPlayerIsNotBlocking;
        StaminaSystem.onStaminaChanged += StaminaSystem_onStaminaChanged;
        PlayerStats.onLevelUp += PlayerStats_onLevelUp;
    }


    void Start()
    {
        uiPlayerLevel.text = "Lvl " + playerStatsSo.CurrentLevel.ToString();
        for (int i = 0; i <= staminasEmpty.Length - 1; i++)
        {
            staminasEmpty[i].gameObject.SetActive(false);
        }
        for (int i = 0; i <= staminasFull.Length - 1; i++)
        {
            staminasFull[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (timerForHidingStamina > 0)
        {
            timerForHidingStamina -= Time.deltaTime;
            if (timerForHidingStamina <= 0)
            {
                HideStaminaUI();
            }
        }
        int minutes = Mathf.FloorToInt(gameDataSO.GameTimer / 60F);
        int seconds = Mathf.FloorToInt(gameDataSO.GameTimer - minutes * 60);

        string time = string.Format("{0:0}:{1:00}", minutes, seconds);
        uiGameTimer.text = time;
    }

    private void OnDestroy()
    {
        playerWeaponController.onPlayerIsBlocking -= PlayerWeaponController_onPlayerIsBlocking;
        playerWeaponController.onPlayerIsNotBlocking -= PlayerWeaponController_onPlayerIsNotBlocking;
        StaminaSystem.onStaminaChanged -= StaminaSystem_onStaminaChanged;
        PlayerStats.onLevelUp -= PlayerStats_onLevelUp;
    }

    private void PlayerStats_onLevelUp()
    {
        uiPlayerLevel.text = "Lvl " + playerStatsSo.CurrentLevel.ToString();
    }

    private void StaminaSystem_onStaminaChanged()
    {
        UpdateStaminaUI(playerDataSo.currentStamina, playerDataSo.maxStamina);
        if (playerDataSo.currentStamina == playerDataSo.maxStamina)
        {
            timerForHidingStamina = hideStaminaTime;
        }
    }

    private void PlayerWeaponController_onPlayerIsBlocking(int currentStamina)
    {
        UpdateStaminaUI(currentStamina, playerDataSo.maxStamina);
    }

    private void PlayerWeaponController_onPlayerIsNotBlocking()
    {
        timerForHidingStamina = 0.1f;
    }

    private void UpdateStaminaUI(int currentStamina, int maxStamina)
    {
        for (int i = 1; i <= maxStamina; i++)
        {
            if (i <= currentStamina)
            {
                staminasFull[i-1].gameObject.SetActive(true);
                staminasEmpty[i-1].gameObject.SetActive(false);
            }
            else
            {
                staminasFull[i-1].gameObject.SetActive(false);
                staminasEmpty[i-1].gameObject.SetActive(true);
            }
        }
    }

    private void HideStaminaUI()
    {
        if (playerDataSo.currentStamina == playerDataSo.maxStamina)
        {
            for (int i = 0; i < staminasEmpty.Length; i++)
            {
                staminasEmpty[i].gameObject.SetActive(false);
                staminasFull[i].gameObject.SetActive(false);
            }
        }
    }
}
