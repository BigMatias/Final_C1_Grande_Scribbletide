using System.Collections.Generic;
using UnityEngine;

public class ExperiencePool : MonoBehaviour
{
    public static ExperiencePool Instance;

    [Header("References")]
    [SerializeField] private GameDataSo gameDataSO;
    [SerializeField] private GameObject smallPrefab;
    [SerializeField] private GameObject mediumPrefab;
    [SerializeField] private GameObject largePrefab;

    [SerializeField] private int poolSize = 50;

    private Dictionary<ExperienceType, Queue<GameObject>> poolDictionary;
    private Dictionary<ExperienceType, GameObject> prefabDictionary;

    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<ExperienceType, Queue<GameObject>>();
        prefabDictionary = new Dictionary<ExperienceType, GameObject>();

        CreatePool(ExperienceType.Blue, smallPrefab);
        CreatePool(ExperienceType.Yellow, mediumPrefab);
        CreatePool(ExperienceType.Red, largePrefab);
    }

    private void CreatePool(ExperienceType type, GameObject prefab)
    {
        Queue<GameObject> objectPool = new Queue<GameObject>();

        prefabDictionary.Add(type, prefab); 

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = CreateExpGem(type, prefab);
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(type, objectPool);
    }

    private GameObject CreateExpGem(ExperienceType type, GameObject prefab)
    {
        float value = 0;

        GameObject obj = Instantiate(prefab, transform);
        ExperienceGem expGem = obj.GetComponent<ExperienceGem>();

        switch (type)
        {
            case ExperienceType.Blue:
                value = gameDataSO.BlueExperienceValue;
                break;

            case ExperienceType.Yellow:
                value = gameDataSO.YellowExperienceValue;
                break;

            case ExperienceType.Red:
                value = gameDataSO.RedExperienceValue;
                break;
        }

        expGem.Initialize(type, value);
        obj.SetActive(false);

        return obj;
    }

    public GameObject GetExperience(ExperienceType type, Vector3 position)
    {
        GameObject obj = poolDictionary[type].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        poolDictionary[type].Enqueue(obj);

        return obj;
    }

    public void ReturnExperience(ExperienceGem gem)
    {
        poolDictionary[gem.Type].Enqueue(gem.gameObject);
    }
}