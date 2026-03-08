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
    public float PlayerHealth;
    public float AttackRange = 20f;
    private bool IsAttacking;
    float distance;

    [Header("Refs")]
    Player_Attack player_Attack;
    private Transform player;




    private List<Attack> attacks;

    void Start()
    {
        player_Attack = GameObject.Find("Player").GetComponent<Player_Attack>();
        

        attacks = new List<Attack>
        {
            new Attack { Name = "Lunge",      Weight = 40f, Range = 8f, Execute = Lunge },
            new Attack { Name = "Slash",      Weight = 35f, Range = 5f, Execute = Slash },
            new Attack { Name = "Projectile", Weight = 25f, Range = 13f, Execute = Projectile },
        };

    }

    void Update()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform;

        distance = Vector2.Distance(transform.position, player.position);
        
        if(AttackRange >= distance && !IsAttacking)
            StartCoroutine(AttackLoop());

        if(player_Attack.Health < 50)
        {
            ChangeWeight("Lunge", 60);
            ChangeWeight("Projectile", 15);
        }
        else
        {
            ChangeWeight("Lunge", 40);
            ChangeWeight("Projectile", 35);
        }
            
    }

    IEnumerator AttackLoop()
    {
        IsAttacking = true;
        ChooseAttack();
        yield return new WaitForSeconds(2f); // attack every 2 seconds
        IsAttacking = false;
    }

    void ChooseAttack()
    {
        float total = attacks.Sum(a => a.Weight);
        float roll = Random.Range(0f, total);

        float cumulative = 0f;
        foreach (var attack in attacks)
        {
            cumulative += attack.Weight;
            if (roll < cumulative && distance <= attack.Range)
            {
                attack.Execute();
                return;
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

    void Lunge()      { Debug.Log("Lunge!"); player_Attack.Health -= 10;}
    void Slash()      { Debug.Log("Slash!"); player_Attack.Health -= 25;}
    void Projectile() { Debug.Log("Projectile!"); player_Attack.Health -= 35;}

    void SetRole(string role) { Role = role; }
}