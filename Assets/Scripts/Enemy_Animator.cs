using UnityEngine;

public class Enemy_Animator : MonoBehaviour
{
    [Header("Refs")]
    private Animator animator;

    EnemyHealth enemyHealth;
    EnemyAI enemyAI;


    void Start()
    {
        animator = GetComponent<Animator>();

        enemyAI = GetComponent<EnemyAI>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        Animation();
    }

    void Animation()
    {
        if(enemyHealth.gotHit == true || enemyAI.State == "Stuned") 
            animator.SetBool("GotHit", true);
        else 
            animator.SetBool("GotHit", false);



        if(enemyAI.State == "Chase")
            animator.SetBool("Chasing", true);
        else
            animator.SetBool("Chasing", false);


        //////////////////////////////////////////
        ///        ATTACKS       ATTACKS        //      
        /// ATTACKS       ATTACKS       ATTACKS //                          
        //////////////////////////////////////////
        
        if(enemyAI.State == "Lunge Attack")
            animator.SetBool("LungeAttack", true);
        else
            animator.SetBool("LungeAttack", false);


        if(enemyAI.State == "Slash Attack")
            animator.SetBool("SlashAttack", true);
        else
            animator.SetBool("SlashAttack", false);


        if(enemyAI.State == "HeavySlash Attack")
            animator.SetBool("HeavyAttack", true);
        else
            animator.SetBool("HeavyAttack", false);
    }      
}
