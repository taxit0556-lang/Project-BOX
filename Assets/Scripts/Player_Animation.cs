using UnityEngine;

public class Player_Animation : MonoBehaviour
{
    [Header("Refs")]
    private Animator animator;
    

    Player_Movement player_Movement;
    Player_Attack player_Attack;

    void Start()
    {   
        animator = GetComponent<Animator>();

        player_Movement = GetComponent<Player_Movement>();
        player_Attack = GetComponent<Player_Attack>();
    }

    void Update() {Animation();}

    void Animation()
    {
        if(player_Movement.isDashing)
            animator.SetBool("Dashed", true);
        else
            animator.SetBool("Dashed", false);

        if(player_Attack.PlayerAttacking)
            animator.SetBool("Attacked", true);
        else
            animator.SetBool("Attacked", false);

        if(player_Attack.GotHit)
            animator.SetBool("Hit", true);
        else
            animator.SetBool("Hit", false);
    }
}
