using UnityEngine;

public class EffectObject : MonoBehaviour
{
    [SerializeField] private float time;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, time);
    }
}
