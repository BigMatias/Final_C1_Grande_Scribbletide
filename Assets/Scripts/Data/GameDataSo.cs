using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "Game/Data")]
public class GameDataSo : ScriptableObject
{
    [Header("Game Data")]
    public float GameTimer;
    public int BlueExperienceValue;
    public int YellowExperienceValue;
    public int RedExperienceValue;
    public int HealItemAmount;
}