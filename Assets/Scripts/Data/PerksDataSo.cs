using UnityEngine;

[CreateAssetMenu(fileName = "Perks", menuName = "Perks/Data")]
public class PerksDataSo : ScriptableObject
{
    [Header("Perks Data")]
    public float MushroomBonusAttackSpeed;
    public float MeatBonusDamage;
    public float CrownBonusRadius;
    public float RedPotionMaxHPIncrease;
    public float GasolineMSIncrease;
}
