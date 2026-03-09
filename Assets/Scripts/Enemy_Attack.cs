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


    [Header("Attack Values")]
    public float AttackRange = 20f;
    private bool IsAttacking;
    float distance;


    [Header("Refs")]
    public Transform player;
    Player_Attack player_Attack;


    [Header("Attack Settings")]
    public float lungeSpeed = 10f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private List<Attack> attacks;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        player_Attack = player.GetComponent<Player_Attack>();

        attacks = new List<Attack>
        {
            new Attack { Name = "AttackA", Weight = 40f, Range = 8f, Execute = Lunge },
            new Attack { Name = "AttackB", Weight = 35f, Range = 5f, Execute = Slash },
            new Attack { Name = "AttackC", Weight = 25f, Range = 13f, Execute = Projectile },
        };
    }

    void Update()
    {
        distance = Vector2.Distance(transform.position, player.position);

        if (AttackRange >= distance && !IsAttacking)
            StartCoroutine(AttackLoop());

        if (player_Attack.Health < 50)
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

        yield return new WaitForSeconds(2f);

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

    //Attacks

    void Lunge()
    {
        Debug.Log("Lunge!");
        StartCoroutine(LungeRoutine());
    }

    IEnumerator LungeRoutine()
    {
        Vector2 dir = (player.position - transform.position).normalized;

        float timer = 0.25f;

        while (timer > 0)
        {
            transform.Translate(dir * lungeSpeed * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }

        if (Vector2.Distance(transform.position, player.position) < 2f)
        {
            player_Attack.Health -= 10;
        }
    }

    void Slash()
    {
        Debug.Log("Slash!");

        if (distance <= 5f)
        {
            player_Attack.Health -= 25;
        }
    }

    void Projectile()
    {
        Debug.Log("Projectile!");

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.position - firePoint.position).normalized;

        proj.GetComponent<Rigidbody2D>().linearVelocity = dir * 12f;
    }

    void SetRole(string role)
    {
        Role = role;
    }
}