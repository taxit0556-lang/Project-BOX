using UnityEngine;

public class Player_Death : MonoBehaviour
{
    [Header("Bool")]
    private bool Dead;

    [Header("refs")]
    [SerializeField] SpriteRenderer spriteRenderer;
    TransitionManager_Base transitionManager;
    public static event System.Action OnPlayerDeath;

    [Header("Settings")]
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

            Debug.Log("NOTDEAD");
        }
    }


    public void OnDeath()
    {
        OnPlayerDeath?.Invoke();

        transitionManager.PlayTransition();
        Dead = true;

        Debug.Log("OnDeath");
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
