﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsPlayer;

    // Attacking
    public float timeBetweenAttacks;
    public AudioClip AttackSound;
    public AudioSource ZombieAttackAudioSource;
    bool alreadyAttacked;
    public int damageAmount = 20;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private PlayerHealth playerHealth;
    private Animator animator;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        playerHealth = player.GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);
    }

    private void AttackPlayer()
    {
        // Make sure zombie doesn't move while attacking
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            // Trigger attack animation
            animator.SetBool("isAttacking", true);
            animator.SetBool("isRunning", false);

            // Play Attack sound
            if (AttackSound != null)
            {
                ZombieAttackAudioSource.Play();
            }

            // Attack code here: Deal damage to player
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log($"Zombie attacks player! Damage dealt: {damageAmount}");
            }
            else
            {
                Debug.LogWarning("PlayerHealth component not found on the player!");
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}