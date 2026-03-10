using UnityEngine;

public class Player_ChainGrabbing : MonoBehaviour
{
    private HingeJoint2D joint;
    private Rigidbody2D rb;

    private bool grabbing;

    public KeyCode swingKey = KeyCode.X;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        joint = gameObject.AddComponent<HingeJoint2D>();
        joint.enabled = false;
        joint.autoConfigureConnectedAnchor = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("SwingPoint") && !grabbing)
        {
            if (Input.GetKey(swingKey))
            {
                Rigidbody2D swingRB = collision.gameObject.GetComponent<Rigidbody2D>();

                if (swingRB != null)
                {
                    joint.connectedBody = swingRB;
                    joint.enabled = true;

                    grabbing = true;

                    rb.linearVelocity = Vector2.zero;
                }
            }
        }
    }

    void Update()
    {
        if (grabbing && !Input.GetKey(swingKey))
        {
            ReleaseChain();
        }
    }

    void ReleaseChain()
    {
        joint.enabled = false;
        joint.connectedBody = null;
        grabbing = false;
    }
}