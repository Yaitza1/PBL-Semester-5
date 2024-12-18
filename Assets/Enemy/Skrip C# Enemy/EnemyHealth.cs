using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startingHealth = 100f;
    private float currentHealth;
    public HealthBar healthBar;
    public Text HealthNumber;

    // New serialized field for damage effect
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageFlashDuration = 0.2f;

    // Reference to the renderer to apply damage material
    private Renderer[] enemyRenderers;
    private Material[] originalMaterials;

    // Tambahkan event untuk notifikasi kekalahan Boss
    public static event Action<GameObject> OnEnemyDefeated;
    private Animator animator;

    public float Health
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = Mathf.Max(value, 0f);
            healthBar.SetHealth((int)currentHealth);
            
            if (currentHealth <= 0f)
            {
                Die();
            }
        }
    }

    void Start()
    {
        currentHealth = startingHealth;
        healthBar.SetMaxHealth((int)startingHealth);
        animator = GetComponent<Animator>();

        // Find all renderers in this GameObject and its children
        enemyRenderers = GetComponentsInChildren<Renderer>();
        
        // Store original materials
        if (enemyRenderers != null && enemyRenderers.Length > 0)
        {
            originalMaterials = new Material[enemyRenderers.Length];
            for (int i = 0; i < enemyRenderers.Length; i++)
            {
                originalMaterials[i] = enemyRenderers[i].material;
            }
        }
    }

    void Update()
    {
        HealthNumber.text = currentHealth.ToString();
    }

    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Current health: {Health}");
        // Trigger damage flash effect
        StartCoroutine(DamageFlashEffect());
    }

    private IEnumerator DamageFlashEffect()
    {
        // Only proceed if we have a damage material and renderers
        if (damageMaterial != null && enemyRenderers != null && enemyRenderers.Length > 0)
        {
            // Apply white damage material to all renderers
            for (int i = 0; i < enemyRenderers.Length; i++)
            {
                if (enemyRenderers[i] != null)
                {
                    enemyRenderers[i].material = damageMaterial;
                }
            }

            // Wait for specified duration
            yield return new WaitForSeconds(damageFlashDuration);

            // Restore original materials
            for (int i = 0; i < enemyRenderers.Length; i++)
            {
                if (enemyRenderers[i] != null)
                {
                    enemyRenderers[i].material = originalMaterials[i];
                }
            }
        }
    }

    private void Die()
    {
        // Disable all movement-related components
        DisableMovementComponents();

        // Play the death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Start the coroutine to wait for the animation and then destroy the object
        StartCoroutine(DeathSequence());
    }

    private void DisableMovementComponents()
    {
        // Disable Rigidbody to stop physics-based movement
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Disable NavMeshAgent if present
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Disable all MonoBehaviour scripts except this one and Animator
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        // Disable any Colliders to prevent further interactions
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator DeathSequence()
    {
        // Wait for the death animation to finish
        if (animator != null)
        {
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
        }

        // Trigger event if boss is defeated
        OnEnemyDefeated?.Invoke(gameObject);
        Debug.Log($"{gameObject.name} has been destroyed!");

        // Wait for 2 seconds before destroying the game object
        yield return new WaitForSeconds(3f);

        // Destroy the game object
        Destroy(gameObject);
    }
}