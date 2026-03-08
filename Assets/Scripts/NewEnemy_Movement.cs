using UnityEngine;

public class NewEnemy_Movement : MonoBehaviour
{
    [SerializeField] Transform Player;
    private float speed = 10.0f;

    void Start()
    {
        
    }
    void Update()
    {
        float step = speed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, Player.position, step);
    }

}
