using UnityEngine;
using System.Collections;

public class FallingPlatforms : MonoBehaviour
{
    public float fallDelay = 0.6f;
    public float destroyDelay = 2f;
    public float shakeAmount = 0.15f;

    private Rigidbody2D rb;
    private bool hasFallen = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasFallen)
        {
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        hasFallen = true;

        // SHAKE PLATFORM
        LeanTween.moveX(gameObject, transform.position.x + shakeAmount, 0.05f)
            .setLoopPingPong(6);

        yield return new WaitForSeconds(fallDelay);

        rb.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}