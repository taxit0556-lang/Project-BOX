using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Enemy_Attack : MonoBehaviour
{
    private struct Attack
    {
        public string Name;
        public float Weight;
        public float Range;
        public float AnimationTime;
        public System.Action Execute;
    }

    [Header("Role")]
    public string Role;

    [Header("Values")]
    Vector2 BoxSize;
    Vector2 AttackBoxSize;
    public float AttackRange = 20f;
    public float ShakeAmount;
    private bool isAttacking;
    public bool Attacktrigger;
    public bool afterAttack;
    public bool CanAttack;
    public bool InRangePlayer;
    public bool hittingPlayer;

    private float distance;
    private float roll;
    private int chosenAttackIndex = 0;

    RaycastHit2D hit;
    RaycastHit2D Attackhit;

    [Header("Refs")]
    private Player_Attack player_Attack;
    EnemyAI enemyAI;
    private Transform player;
    

    private List<Attack> attacks;
    private bool lowHealthMode = false;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        player_Attack = GameObject.Find("Player").GetComponent<Player_Attack>();
        player = player_Attack.transform;

        attacks = new List<Attack>
        {
            new Attack { Name = "Lunge",      Weight = 40f, Range = 3f, AnimationTime = 0.26f, Execute = Lunge },
            new Attack { Name = "Slash",      Weight = 35f, Range = 2f,  AnimationTime = 0.16f, Execute = Slash },
            new Attack { Name = "HeavySlash", Weight = 25f, Range = 1f,  AnimationTime = 0.19f, Execute = HeavySlash },
        };
    }

    void Update()
    {
        BoxSize = new Vector2(attacks[chosenAttackIndex].Range, 1.85454f);

        distance = Vector2.Distance(transform.position, player.position);

        // Only update weights when health state changes
        bool shouldUseLowHealth = player_Attack.Health < 50;
        if (shouldUseLowHealth != lowHealthMode)
        {
            lowHealthMode = shouldUseLowHealth;
            if (lowHealthMode)
            {
                ChangeWeight("Lunge", 60);
                ChangeWeight("HeavySlash", 15);
            }
            else
            {
                ChangeWeight("Lunge", 40);
                ChangeWeight("HeavySlash", 35);
            }
        }
        
        UpdateRaycast();
        
        if (AttackRange >= distance && !isAttacking && enemyAI.State != "Stuned")
            StartCoroutine(AttackLoop());

        if(afterAttack)
            AfterAttack();

    }

    IEnumerator AttackLoop()
    {
        isAttacking = true;
        ChooseAttack(false);
        StartCoroutine(AttackCast());
        yield return new WaitForSeconds(2f);
        isAttacking = false;
        CanAttack = false;
    }
    

    IEnumerator AttackCast()
    {
        if (isAttacking)
        {
            AttackRaycast();
            yield return new WaitForSeconds(attacks[chosenAttackIndex].AnimationTime);
            StopCoroutine(AttackCast());
        }
    }   

    IEnumerator Shake()
    {
        Attacktrigger = true;
        CanAttack = false;
        LeanTween.moveX(gameObject, transform.position.x + 0.1f, 0.05f)
        .setEase(LeanTweenType.easeShake)
        .setLoopPingPong(6);

        yield return new WaitForSeconds(0.5f);
        Attacktrigger = false;
        CanAttack = true;
        
        if(attacks[chosenAttackIndex].Range <= distance)
            ChooseAttack(true);
        else
            ChooseAttack(false);
    }

    void ChooseAttack(bool ShakeAttack)
    {
        float total = attacks.Sum(a => a.Weight);
        roll = Random.Range(0f, total);

        float cumulative = 0f;
        for (int i = 0; i < attacks.Count; i++)
        {
            cumulative += attacks[i].Weight;
            if (roll < cumulative)
            {
                if (distance <= attacks[i].Range && InRangePlayer || ShakeAttack)
                {
                    if(CanAttack == false)
                        StartCoroutine(Shake());
                    else if (ShakeAttack)
                    {
                        enemyAI.SetState(attacks[i].Name + " Attack");
                        enemyAI.StunTime(attacks[i].AnimationTime);
                        chosenAttackIndex = i;
                        attacks[i].Execute();

                        if (Attackhit)
                            player_Attack.OnHit(1, transform);

                        ShakeAttack = false;
                    }
                    else
                    {

                        enemyAI.SetState(attacks[i].Name + " Attack");
                        enemyAI.StunTime(attacks[i].AnimationTime);

                        chosenAttackIndex = i;

                        attacks[i].Execute();

                        if (Attackhit)
                        {
                            player_Attack.OnHit(1, transform);
                        }

                        ShakeAttack = false;

                        afterAttack = true;
                    
                    }
                    return;         
                }
            }
        }
    }

    void AfterAttack()
    {
        enemyAI.SetState("AfterAttack");
    }

    void ChangeWeight(string attackName, float newWeight)
    {
        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i].Name == attackName)
            {
                Attack a = attacks[i];
                a.Weight = newWeight;
                attacks[i] = a;
                return;
            }
        }
    }

    void UpdateRaycast()
    {
        Vector2 castOrigin = new Vector2(transform.position.x + 1.1f * transform.localScale.x, transform.position.y);
        Vector2 castDirection = (Vector2)player.position - castOrigin;
        float thisRange = attacks[chosenAttackIndex].Range;

        hit = Physics2D.BoxCast(castOrigin, BoxSize, 0, transform.position);

        InRangePlayer = hit.collider != null && hit.collider.CompareTag("Player");
    }
    void AttackRaycast()
    {
        Vector2 castOrigin = new Vector2(transform.position.x + 1.1f * transform.localScale.x, transform.position.y);

        Attackhit = Physics2D.BoxCast(castOrigin, AttackBoxSize, 0, transform.position);

        hittingPlayer = Attackhit.collider != null && Attackhit.collider.CompareTag("Player");
    }

    void OnDrawGizmos() 
    {
        if (player == null || attacks == null) return;

        Vector2 castOrigin = new Vector2(transform.position.x + 1.1f * transform.localScale.x, transform.position.y);
        Vector2 castDirection = (Vector2)player.position - castOrigin;
        float thisRange = attacks[chosenAttackIndex].Range;

        

        Gizmos.color = InRangePlayer ? Color.green : Color.red;
        Gizmos.DrawWireCube(castOrigin, BoxSize);

        Gizmos.color = hittingPlayer ? Color.green : Color.red;
        Gizmos.DrawCube(castOrigin, AttackBoxSize);
    }

    void Lunge()      { Debug.Log("Lunge!");      if(hittingPlayer){player_Attack.Health -= 10;} AttackBoxSize = new Vector2(1.28f, 0.47f);}
    void Slash()      { Debug.Log("Slash!");      if(hittingPlayer){player_Attack.Health -= 25;} AttackBoxSize = new Vector2(1.28f, 1.14f);}
    void HeavySlash() { Debug.Log("HeavySlash!"); if(hittingPlayer){player_Attack.Health -= 35;} AttackBoxSize = new Vector2(1.28f, 2.55f);}
}