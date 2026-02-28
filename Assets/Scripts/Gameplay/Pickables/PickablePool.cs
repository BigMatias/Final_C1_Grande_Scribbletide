using System.Collections.Generic;
using UnityEngine;

public class PickablePool : MonoBehaviour
{
    public static PickablePool Instance;

    [Header("References")]
    [SerializeField] private GameDataSo gameDataSO;
    [SerializeField] private GameObject blueExpPrefab;
    [SerializeField] private GameObject yellowExpPrefab;
    [SerializeField] private GameObject redExpPrefab;
    [SerializeField] private GameObject healItemPrefab;

    [SerializeField] private int poolSize = 50;

    private Dictionary<PickableType, Queue<GameObject>> poolDictionary;
    private Dictionary<PickableType, GameObject> prefabDictionary;

    private void Awake()
    {
        Instance = this;

        poolDictionary = new Dictionary<PickableType, Queue<GameObject>>();
        prefabDictionary = new Dictionary<PickableType, GameObject>();

        CreatePool(PickableType.ExperienceBlue, blueExpPrefab);
        CreatePool(PickableType.ExperienceYellow, yellowExpPrefab);
        CreatePool(PickableType.ExperienceRed, redExpPrefab);
        CreatePool(PickableType.HealItem, healItemPrefab);
    }

    private void CreatePool(PickableType type, GameObject prefab)
    {

        Queue<GameObject> objectPool = new Queue<GameObject>();

        prefabDictionary.Add(type, prefab); 

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = CreatePickable(type, prefab);
            objectPool.Enqueue(obj);
        }

        poolDictionary.Add(type, objectPool);
    }

    private GameObject CreatePickable(PickableType type, GameObject prefab)
    {
        int value = 0;

        GameObject obj = Instantiate(prefab, transform);
        Pickable pickable = obj.GetComponent<Pickable>();

        switch (type)
        {
            case PickableType.ExperienceBlue:
                value = gameDataSO.BlueExperienceValue;
                break;

            case PickableType.ExperienceYellow:
                value = gameDataSO.YellowExperienceValue;
                break;

            case PickableType.ExperienceRed:
                value = gameDataSO.RedExperienceValue;
                break;
            case PickableType.HealItem:
                value = gameDataSO.HealItemAmount;
                break;
        }

        pickable.Initialize(type, value);
        obj.SetActive(false);

        return obj;
        
    }

    public GameObject GetPickable(PickableType type, Vector3 position)
    {
        GameObject obj = poolDictionary[type].Dequeue();
        obj.SetActive(true);
        obj.transform.position = position;
        poolDictionary[type].Enqueue(obj);

        return obj;
    }

    public void ReturnPickable(Pickable pickable)
    {
        poolDictionary[pickable.Type].Enqueue(pickable.gameObject);
    }
}