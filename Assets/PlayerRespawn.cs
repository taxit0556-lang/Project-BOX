using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Vector2 respawnPoint;

    void Start()
    {
        respawnPoint = transform.position;
    }

    public void Respawn()
    {
        transform.position = respawnPoint;
    }
}