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
        public System.Action Execute;
    }

    [Header("Role")]
    public string Role;

    [Header("Values")]
    public float AttackRange = 20f;
    private bool isAttacking;
    public bool hittingPlayer;

    private float distance;
    private float roll;
    private int chosenAttackIndex = 0;

    RaycastHit2D hit;

    [Header("Refs")]
    private Player_Attack player_Attack;
    private Transform player;

    private List<Attack> attacks;
    private bool lowHealthMode = false;

    void Start()
    {
        player_Attack = GameObject.Find("Player").GetComponent<Player_Attack>();
        player = player_Attack.transform;

        attacks = new List<Attack>
        {
            new Attack { Name = "Lunge",      Weight = 40f, Range = 10f, Execute = Lunge },
            new Attack { Name = "Slash",      Weight = 35f, Range = 7f,  Execute = Slash },
            new Attack { Name = "HeavySlash", Weight = 25f, Range = 7f,  Execute = HeavySlash },
        };
    }

    void Update()
    {
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

        
        if (AttackRange >= distance && !isAttacking)
            StartCoroutine(AttackLoop());
    }

    IEnumerator AttackLoop()
    {
        isAttacking = true;
        ChooseAttack();
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    void ChooseAttack()
    {
        float total = attacks.Sum(a => a.Weight);
        roll = Random.Range(0f, total);

        float cumulative = 0f;
        for (int i = 0; i < attacks.Count; i++)
        {
            cumulative += attacks[i].Weight;
            if (roll < cumulative)
            {
                if (distance <= attacks[i].Range && hittingPlayer)
                {
                    chosenAttackIndex = i;
                    attacks[i].Execute();
                    return;
                }
            }
        }
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
        Vector2 castOrigin = transform.position;
        Vector2 castDirection = (Vector2)player.position - castOrigin;
        float thisRange = attacks[chosenAttackIndex].Range;

        hit = Physics2D.Raycast(castOrigin, castDirection.normalized, thisRange);

        hittingPlayer = hit.collider != null && hit.collider.CompareTag("Player");
    }

    void OnDrawGizmos() 
    {
        if (player == null || attacks == null) return;

        Vector2 castOrigin = transform.position;
        Vector2 castDirection = (Vector2)player.position - castOrigin;
        float thisRange = attacks[chosenAttackIndex].Range;

        Gizmos.color = hittingPlayer ? Color.green : Color.red;
        Gizmos.DrawRay(castOrigin, castDirection.normalized * thisRange);
    }

    void Lunge()      { Debug.Log("Lunge!");      player_Attack.Health -= 10; }
    void Slash()      { Debug.Log("Slash!");      player_Attack.Health -= 25; }
    void HeavySlash() { Debug.Log("HeavySlash!"); player_Attack.Health -= 35; }
}