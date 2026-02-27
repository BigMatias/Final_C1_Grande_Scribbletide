using UnityEngine;

[CreateAssetMenu(fileName = "HordeSpawner", menuName = "Spawner/Data")]
public class HordeSpawnerDataSo : ScriptableObject
{
    [Header("Spawner Data")]
    public float BaseWaveInterval;
    public float BaseBudget;
    public float EliteStartTime;
    public float InitialPoolSizePerEnemy;
    public float EliteBaseChance;       
    public float EliteChanceGrowth;   
    public float MaxElitePercentage;

}
