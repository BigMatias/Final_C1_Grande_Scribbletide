using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 cameraPosition;

    void Start()
    {
        cameraPosition = transform.position;
    }

    void Update()
    {
        cameraPosition.x = target.position.x;
        cameraPosition.y = target.position.y;
        transform.position = cameraPosition;
    }
}
