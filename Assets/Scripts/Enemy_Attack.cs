using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Attack : MonoBehaviour
{
    [Header("Player State")]
    public float PlayerHealth = 100f;
    public float PlayerMaxHealth = 100f;
    public float PlayerDefense = 0f;

    [Header("Economy")]
    public float Tokens = 10f;

    [Header("Shop")]
    public string[] Shop       = { "Dart", "Light Attack", "Heavy Attack", "Special Attack" };
    public float[]  Cost       = { 1f,     2f,             4f,             7f               };
    public float[]  Damage     = { 5f,     10f,            30f,            45f              };
    public float[]  Cooldown   = { 0.5f,   1f,             2f,             3f               };
    public bool[]   IsRanged   = { true,   false,          false,          true             };
    public bool[]   IsAerial   = { false,  false,          true,           false            };
    public bool[]   HasKnockback = { false, true,          true,           false            };

    [Header("Debug / Read-Only")]
    public float TargetDPT;
    public List<string> QueuedAttacks = new List<string>();

    private List<int> _attackQueue = new List<int>();
    private bool _isAttacking = false;

    private Transform _player;
    private Rigidbody2D _playerRb;
    private MonoBehaviour _playerController; // swap this for your actual player controller type

    void Start()
    {
        // swap these for however you reference your player
        _player           = GameObject.FindWithTag("Player").transform;
        _playerRb         = _player.GetComponent<Rigidbody2D>();
        _playerController = _player.GetComponent<MonoBehaviour>(); // swap for your player script
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Call this from a range trigger, wave manager, or AI state machine
    // ────────────────────────────────────────────────────────────────────────
    public void BeginAttackCycle(float tokens, float playerHealth, float playerDefense = 0f)
    {
        if (_isAttacking) return;

        Tokens        = tokens;
        PlayerHealth  = playerHealth;
        PlayerDefense = playerDefense;

        QueuedAttacks.Clear();
        _attackQueue.Clear();

        SetTargetDPT();
        GoShop();

        StartCoroutine(ExecuteAttackQueue());
    }

    // ────────────────────────────────────────────────────────────────────────
    //  STEP 1 — Set a DPT goal based on player health
    // ────────────────────────────────────────────────────────────────────────
    void SetTargetDPT()
    {
        float healthPercent = PlayerHealth / PlayerMaxHealth;

        if      (healthPercent < 0.30f) TargetDPT = 4f;
        else if (healthPercent < 0.60f) TargetDPT = 6f;
        else                            TargetDPT = 8f;

        if (PlayerDefense > 5f) TargetDPT += 2f;
    }

    // ────────────────────────────────────────────────────────────────────────
    //  STEP 2 — Spend tokens, fill the attack queue
    // ────────────────────────────────────────────────────────────────────────
    void GoShop()
    {
        float remaining = Tokens;
        int   safety    = 0;

        while (remaining > 0f && safety++ < 100)
        {
            int best = PickBestAttack(remaining);
            if (best == -1) break;

            _attackQueue.Add(best);
            QueuedAttacks.Add(Shop[best]);
            remaining -= Cost[best];
        }
    }

    int PickBestAttack(float budget)
    {
        int   bestIndex = -1;
        float bestScore = float.MinValue;

        for (int i = 0; i < Shop.Length; i++)
        {
            if (Cost[i] > budget) continue;

            float score = ScoreAttack(i);
            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    float ScoreAttack(int i)
    {
        float score = 0f;
        float healthPercent = PlayerHealth / PlayerMaxHealth;

        // ── Sidescroller context ─────────────────────────────────────────────
        float distance        = Vector2.Distance(transform.position, _player.position);
        bool  playerAirborne  = !IsPlayerGrounded();
        bool  playerApproaching = IsPlayerApproaching();

        // prefer ranged when far, melee when close
        if (distance > 5f &&  IsRanged[i])    score += 6f;
        if (distance < 2f && !IsRanged[i])    score += 6f;

        // aerial attacks are better against jumping players
        if (playerAirborne   &&  IsAerial[i])    score += 5f;
        if (!playerAirborne  && !IsAerial[i])    score += 2f;

        // if player is rushing at you, knock them back
        if (playerApproaching && HasKnockback[i]) score += 4f;

        // ── DPT vs goal ──────────────────────────────────────────────────────
        float dpt      = Damage[i] / Cost[i];
        float dptDelta = dpt - TargetDPT;
        score += (dptDelta >= 0f) ? 10f : 10f + dptDelta;

        // ── Health-based bonuses ─────────────────────────────────────────────
        if (Damage[i] - PlayerDefense >= PlayerHealth) score += 20f;  // one-shot
        if (healthPercent < 0.30f && Cost[i] <= 2f)   score += 5f;   // swarm to finish
        if (healthPercent > 0.60f && Damage[i] >= 30f) score += 4f;  // invest when healthy

        // ── Defense penetration ──────────────────────────────────────────────
        if (PlayerDefense > 5f && Damage[i] >= 30f) score += 3f;

        // ── Redundancy penalty ───────────────────────────────────────────────
        score -= _attackQueue.FindAll(x => x == i).Count * 2f;

        // ── Noise ────────────────────────────────────────────────────────────
        score += Random.Range(-0.5f, 0.5f);

        return score;
    }

    // ────────────────────────────────────────────────────────────────────────
    //  STEP 3 — Fire attacks in sequence with cooldowns
    // ────────────────────────────────────────────────────────────────────────
    IEnumerator ExecuteAttackQueue()
    {
        _isAttacking = true;

        foreach (int i in _attackQueue)
        {
            FireAttack(i);
            yield return new WaitForSeconds(Cooldown[i]);
        }

        _isAttacking = false;
    }

    void FireAttack(int index)
    {
        float netDamage = Mathf.Max(0f, Damage[index] - PlayerDefense);

        // plug in your player damage call here, e.g:
        // _player.GetComponent<PlayerHealth>().TakeDamage(netDamage);

        Debug.Log($"[Enemy] '{Shop[index]}' hits for {netDamage}");
    }

    // ────────────────────────────────────────────────────────────────────────
    //  Helpers — swap these out for your actual player script calls
    // ────────────────────────────────────────────────────────────────────────
    bool IsPlayerGrounded()
    {
        // replace with your player controller's grounded check, e.g:
        // return _player.GetComponent<PlayerController>().IsGrounded;
        return true;
    }

    bool IsPlayerApproaching()
    {
        float directionToPlayer = Mathf.Sign(_player.position.x - transform.position.x);
        float playerVelX        = _playerRb.linearVelocity.x;

        // player is moving toward this enemy
        return Mathf.Sign(playerVelX) != directionToPlayer && playerVelX != 0f;
    }
}