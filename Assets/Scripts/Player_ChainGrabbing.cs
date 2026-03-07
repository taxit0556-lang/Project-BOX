using UnityEngine;

public class Player_ChainGrabbing: MonoBehaviour
{
    private HingeJoint2D joint;
    private Rigidbody2D rb;

    private bool grabbing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = gameObject.AddComponent<HingeJoint2D>();
        joint.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Chain") && !grabbing)
        {
            Rigidbody2D chainRB = collision.gameObject.GetComponent<Rigidbody2D>();

            if (chainRB != null)
            {
                joint.connectedBody = chainRB;
                joint.enabled = true;

                grabbing = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void Update()
    {
        if (grabbing && Input.GetKeyDown(KeyCode.Space))
        {
            ReleaseChain();
        }
    }

    void ReleaseChain()
    {
        rb.AddForce(new Vector2(0, 9f),ForceMode2D.Impulse);
        joint.enabled = false;
        joint.connectedBody = null;
        grabbing = false;
    }
}