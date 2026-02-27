using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSettings", menuName = "Weapon/Data")]

public class WeaponDataSo : ScriptableObject
{

    [Header("References")]
    public GameObject[] weaponPrefabs;
    public WeaponType weaponType;
    public string weaponId;

    [Header("Sounds")]
    public AudioClip WeaponSwingSound;
    public AudioClip WeaponHitSound;
    public AudioClip WeaponBlockSound;

    [Header("Configs")]
    public float BaseAttackSpeed;
    public float WeaponCurrentDamage;
    public float WeaponCurrentSpeed;
    public float WeaponStaggerTime;
    public float WeapopnKnockbackTimer;
    public float WeapopnKnockbackForce;
    public float StaminaDepletedSpeedReduced;
    public int ComboIndex;

    [Header("Longsword Configs")]
    public float LongswordBaseSpeed;
    public float LongswordBaseDamage;
    public float LongswordStaggerTime;
    public float LongswordKnockback;
    public AudioClip LongswordSwingSound;
    public AudioClip LongswordHitSound;

    [Header("Hammer Configs")]
    public float HammerBaseSpeed;
    public float HammerBaseDamage;
    public float HammerStaggerTime;
    public float HammerKnockback;
    public AudioClip HammerSwingSound;
    public AudioClip HammerHitSound;

    [Header("General Configs")]
    public float maxCombo;
    public float timeForComboReset;
    public float parryWindow;
    public float bufferTime;


}
