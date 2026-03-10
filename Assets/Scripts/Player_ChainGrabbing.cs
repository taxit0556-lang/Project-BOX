using UnityEngine;

public class Player_ChainGrabbing : MonoBehaviour
{
    private DistanceJoint2D joint;
    private Rigidbody2D rb;

    public float grabRadius = 2.5f;
    public LayerMask swingLayer;

    private bool grabbing;
    private Transform currentPoint;

    private LineRenderer chain;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        joint = gameObject.AddComponent<DistanceJoint2D>();
        joint.enabled = false;
        joint.autoConfigureDistance = false;

        chain = GetComponent<LineRenderer>();
        chain.positionCount = 2;
        chain.enabled = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X) && !grabbing)
        {
            FindSwingPoint();
        }

        if (grabbing && !Input.GetKey(KeyCode.X))
        {
            ReleaseChain();
        }

        if (grabbing)
        {
            UpdateChain();
        }
    }

    void FindSwingPoint()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, grabRadius, swingLayer);

        float closest = Mathf.Infinity;
        Transform closestPoint = null;

        foreach (Collider2D hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (dist < closest)
            {
                closest = dist;
                closestPoint = hit.transform;
            }
        }

        if (closestPoint != null)
        {
            AttachToPoint(closestPoint);
        }
    }

    void AttachToPoint(Transform point)
    {
        SwingPoint sp = point.GetComponent<SwingPoint>();

        joint.connectedAnchor = point.position;
        joint.enabled = true;

        if (sp != null)
        {
            joint.distance = sp.ropeLength;
        }

        grabbing = true;
        currentPoint = point;

        chain.enabled = true;
    }

    void UpdateChain()
    {
        chain.SetPosition(0, transform.position);
        chain.SetPosition(1, currentPoint.position);
    }

    void ReleaseChain()
    {
        joint.enabled = false;
        grabbing = false;

        chain.enabled = false;
        currentPoint = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}