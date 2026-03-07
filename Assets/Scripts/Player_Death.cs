using UnityEngine;

public class Player_Death : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    TransitionManager_Base transitionManager;
    private bool Dead;

    public Vector2 CheckPoint;


    void Awake()
    {
        transitionManager = GameObject.Find("TransitionManager").GetComponent<TransitionManager_Base>();
    }


    void Update()
    {
        if(transitionManager.State == "Middle" && Dead)
        {
            transform.position = CheckPoint;
            Dead = false;
        }
    }


    public void OnDeath()
    {
        transitionManager.PlayTransition();
        Dead = true;
    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
            OnDeath();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            Debug.Log("HitCheckPoint");
            CheckPoint = other.gameObject.transform.position;
        }
    }
}
