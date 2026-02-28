using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatsSo playerStatsSO;
    [SerializeField] private PlayerDataSo playerDataSO;
    [SerializeField] private WeaponDataSo weaponDataSO;
    [SerializeField] private PerksDataSo perksDataSO;
    [SerializeField] private AudioManager audioManager;

    public static PlayerStats Instance;
    private PlayerMovement playerMovement;
    private HealthSystem healthSystem;

    public static event Action onLevelUp;
    public static event Action onMagnetUpgraded;


    private void Awake()
    {
        Instance = this;
        healthSystem = GetComponent<HealthSystem>();
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.onExpObtained += PlayerMovement_onExpObtained;
    }

    void Start()
    {
        playerStatsSO.CurrentExperience = 0;
        playerStatsSO.CurrentLevel = 0;
        playerStatsSO.ExpNeededForNextLvl = playerStatsSO.ExpLvl0Requirement;
        playerStatsSO.CurrentExpMagnetRadius = playerStatsSO.BaseExpMagnetRadius;
    }

    private void PlayerMovement_onExpObtained()
    {
        if (playerStatsSO.CurrentExperience >= playerStatsSO.ExpNeededForNextLvl)
        {
            audioManager.ReproduceClip(playerStatsSO.PlayerLeveledUpAudio);
            playerStatsSO.CurrentExperience = 0;
            playerStatsSO.CurrentLevel += 1;
            onLevelUp?.Invoke();

            if (playerStatsSO.CurrentLevel <= 10)
            {
                playerStatsSO.ExpNeededForNextLvl += playerStatsSO.ExpRequiredIncrease_Lvl0To10;
            }
            else if (playerStatsSO.CurrentLevel <= 25)
            {
                playerStatsSO.ExpNeededForNextLvl += playerStatsSO.ExpRequiredIncrease_Lvl10To25;
            }
            else if (playerStatsSO.CurrentLevel <= 39)
            {
                playerStatsSO.ExpNeededForNextLvl += playerStatsSO.ExpRequiredIncrease_Lvl25To39;
            }
            else if (playerStatsSO.CurrentLevel >= 40)
            {
                playerStatsSO.ExpNeededForNextLvl += playerStatsSO.ExpRequiredIncrease_Lvl40;
            }
        }
    }

    public void ApplyPerk(PerkType perk)
    {
        switch (perk)
        {
            case PerkType.Mushroom:
                weaponDataSO.WeaponCurrentSpeed += perksDataSO.MushroomBonusAttackSpeed;
                break;

            case PerkType.Meat:
                weaponDataSO.WeaponCurrentDamage += perksDataSO.MeatBonusDamage;
                break;

            case PerkType.Crown:
                playerStatsSO.CurrentExpMagnetRadius += perksDataSO.CrownBonusRadius;
                onMagnetUpgraded?.Invoke();
                break;

            case PerkType.RedPotion:
                healthSystem.IncreaseMaxHP(perksDataSO.RedPotionMaxHPIncrease);
                break;

            case PerkType.Gasoline:
                playerDataSO.CurrentSpeed += perksDataSO.GasolineMSIncrease;
                playerDataSO.SpeedSave += perksDataSO.GasolineMSIncrease;
                break;
        }
    }


}
