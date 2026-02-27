using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/Stats")]
public class PlayerStatsSo : ScriptableObject
{
    [Header("Configs")]
    public AudioClip PlayerLeveledUpAudio;

    [Header("Stats")]
    public float BaseExpMagnetRadius;
    public float CurrentExpMagnetRadius;
    public float CurrentExperience;
    public int CurrentLevel;
    public int ExpLvl0Requirement;
    public int ExpNeededForNextLvl;
    public int ExpRequiredIncrease_Lvl0To10;
    public int ExpRequiredIncrease_Lvl10To25;
    public int ExpRequiredIncrease_Lvl25To39;
    public int ExpRequiredIncrease_Lvl40;

}