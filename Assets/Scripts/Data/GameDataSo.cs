using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "Game/Data")]
public class GameDataSo : ScriptableObject
{
    [Header("Game Data")]
    public float GameTimer;
    public float BlueExperienceValue;
    public float YellowExperienceValue;
    public float RedExperienceValue;
}