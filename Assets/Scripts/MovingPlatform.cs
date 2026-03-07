using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Vector3 target;
    private Vector3 lastPosition;

    public Vector2 PlatformVelocity { get; private set; }

    void Start()
    {
        target = pointB.position;
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
        }

        PlatformVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        lastPosition = transform.position;
    }
}