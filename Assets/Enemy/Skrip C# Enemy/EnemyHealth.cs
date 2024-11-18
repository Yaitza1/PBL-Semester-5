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

    // Tambahkan event untuk notifikasi kekalahan Boss
    public static event Action<GameObject> OnEnemyDefeated;

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
    }

    void Update()
    {
        HealthNumber.text = currentHealth.ToString();
    }

    public void TakeDamage(float damageAmount)
    {
        Health -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Current health: {Health}");
    }

    private void Die()
    {
        // Trigger event jika boss dikalahkan
        OnEnemyDefeated?.Invoke(gameObject);
        Debug.Log($"{gameObject.name} has been destroyed!");
        Destroy(gameObject);
    }
}
