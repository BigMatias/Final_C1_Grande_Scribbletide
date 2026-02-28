using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HordeSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyClass
    {
        public EnemyType enemyType;
        public GameObject prefab;
        public int cost;

        [HideInInspector] public Queue<GameObject> pool;
    }
    public static HordeSpawner Instance;
    private Transform enemiesParent;

    [SerializeField] private GameDataSo gameDataSO;
    [SerializeField] private HordeSpawnerDataSo hordeSpawnerDataSo;

    [Header("Enemies")]
    [SerializeField] private List<EnemyClass> normalEnemies;
    [SerializeField] private List<EnemyClass> eliteEnemies;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private Dictionary<EnemyType, EnemyClass> enemyLookup;
    private Transform spawnPoint;

    private void Awake()
    {
        PlayerMovement.onPlayerDied += PlayerMovement_onPlayerDied;
    }

    private void PlayerMovement_onPlayerDied()
    {
        ResetSpawner();
    }

    private void Start()
    {
        Instance = this;
        enemiesParent = transform.Find("Enemies");

        InitializePools();
        ResetSpawner();
        SpawnWave();
        StartCoroutine(WaveRoutine());
    }

    private void OnDisable()
    {
        PlayerMovement.onPlayerDied -= PlayerMovement_onPlayerDied;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        StopAllCoroutines();
    }

    private void InitializePools()
    {
        enemyLookup = new Dictionary<EnemyType, EnemyClass>();

        InitializeEnemyListPool(normalEnemies);
        InitializeEnemyListPool(eliteEnemies);
    }

    private void InitializeEnemyListPool(List<EnemyClass> enemyList)
    {
        foreach (var enemy in enemyList)
        {
            enemy.pool = new Queue<GameObject>();

            enemyLookup.Add(enemy.enemyType, enemy);

            for (int i = 0; i < hordeSpawnerDataSo.InitialPoolSizePerEnemy; i++)
            {
                GameObject obj = Instantiate(enemy.prefab, enemiesParent);
                obj.SetActive(false);
                enemy.pool.Enqueue(obj);
            }
        }
    }

    private IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(hordeSpawnerDataSo.BaseWaveInterval);
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        float difficulty = 1 + (gameDataSO.GameTimer * 0.03f);
        float waveBudget = hordeSpawnerDataSo.BaseBudget * Mathf.Pow(difficulty, 1.15f);

        float remainingBudget = waveBudget;

        int totalSpawned = 0;
        int eliteSpawned = 0;

        GetWaveSpawnPoint();

        while (remainingBudget > 0)
        {
            EnemyClass chosenEnemy = ChooseEnemyControlled(totalSpawned, eliteSpawned);

            if (chosenEnemy.cost <= remainingBudget)
            {
                SpawnFromPool(chosenEnemy);

                remainingBudget -= chosenEnemy.cost;
                totalSpawned++;

                if (eliteEnemies.Contains(chosenEnemy))
                    eliteSpawned++;
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnFromPool(EnemyClass enemyType)
    {
        if (enemyType.pool.Count == 0)
        {
            ExpandPool(enemyType);
        }

        GameObject obj = enemyType.pool.Dequeue();

        obj.transform.SetParent(enemiesParent);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;

        obj.SetActive(true);
    }
    private void ExpandPool(EnemyClass enemy)
    {
        int expandAmount = 5;

        for (int i = 0; i < expandAmount; i++)
        {
            GameObject obj = Instantiate(enemy.prefab, enemiesParent);
            obj.SetActive(false);
            enemy.pool.Enqueue(obj);
        }
    }

    public void ReturnToPool(GameObject obj, EnemyType enemyTypeIn)
    {
        obj.SetActive(false);

        if (enemyLookup.TryGetValue(enemyTypeIn, out EnemyClass enemyClass))
        {
            enemyClass.pool.Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    private EnemyClass ChooseEnemyControlled(int totalSpawned, int eliteSpawned)
    {
        float currentEliteRatio = totalSpawned == 0 ? 0 : (float)eliteSpawned / totalSpawned;

        if (currentEliteRatio >= hordeSpawnerDataSo.MaxElitePercentage)
        {
            return normalEnemies[Random.Range(0, normalEnemies.Count)];
        }

        float eliteChance = 0f;

        if (gameDataSO.GameTimer > hordeSpawnerDataSo.EliteStartTime)
        {
            eliteChance = hordeSpawnerDataSo.EliteBaseChance +
                          (gameDataSO.GameTimer - hordeSpawnerDataSo.EliteStartTime) *
                          hordeSpawnerDataSo.EliteChanceGrowth;
        }

        eliteChance = Mathf.Clamp(eliteChance, 0f, 0.1f);

        if (Random.value < eliteChance && eliteEnemies.Count > 0)
        {
            return eliteEnemies[Random.Range(0, eliteEnemies.Count)];
        }

        return normalEnemies[Random.Range(0, normalEnemies.Count)];
    }

    private void GetWaveSpawnPoint()
    {
        int spawnPointSelected = Random.Range(0, spawnPoints.Length);
        SpawnPoint spawnPointComponent = spawnPoints[spawnPointSelected].GetComponent<SpawnPoint>();

        if (!spawnPointComponent.IsPlayerOnSpawnPoint())
        {
            spawnPoint = spawnPoints[spawnPointSelected];
        }
        else
        {
            GetWaveSpawnPoint();
        }
    }

    private void ResetSpawner()
    {
        StopAllCoroutines();

        foreach (Transform child in enemiesParent)
        {
            GameObject obj = child.gameObject;

            if (obj.activeSelf)
            {
                EnemyController enemy = obj.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    ReturnToPool(obj, enemy.enemyType);
                }
                else
                {
                    obj.SetActive(false);
                }
            }
        }

        StartCoroutine(WaveRoutine());
    }
}