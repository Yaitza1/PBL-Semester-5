using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth = 100;
	public int currentHealth;
    public Text HealthNumber;
	public HealthBar healthBar;
    public float restartDelay = 2f;

    // Start is called before the first frame update
    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
		UpdateHealthUI();
    }

	 public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

	private void UpdateHealthUI()
    {
        healthBar.SetHealth(currentHealth);
        HealthNumber.text = currentHealth.ToString();
    }

	private void Die()
    {
        Debug.Log("Player has died!");
        // Start the coroutine to restart the level after a delay
        StartCoroutine(RestartLevelAfterDelay());
    }

    private IEnumerator RestartLevelAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(restartDelay);

        // Restart the current level
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}