
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private bool isPlayerOnSpawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == (int)Layers.Player)
        {
            isPlayerOnSpawnPoint = true;
        }
    }

    public bool IsPlayerOnSpawnPoint()
    {
        if (isPlayerOnSpawnPoint) 
            return true;
        else return false;
    }
}
