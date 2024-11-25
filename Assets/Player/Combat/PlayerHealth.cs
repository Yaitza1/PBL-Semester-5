using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	public int maxHealth = 100;
	public int currentHealth;
    public Text HealthNumber;
	public HealthBar healthBar;
    public float restartDelay = 2f;

    // Reference to GameOver script
    public GameOver gameOverManager;

    // Post-processing variables
    public PostProcessVolume postProcessVolume;
    public float vignetteDuration = 0.5f;
    public float vignetteMaxIntensity = 0.4f;

    private Vignette vignette;

    void Start()
    {
		currentHealth = maxHealth;
		healthBar.SetMaxHealth(maxHealth);
		UpdateHealthUI();

        // Initialize post-processing
        if (postProcessVolume != null && postProcessVolume.profile.TryGetSettings(out vignette))
        {
            vignette.intensity.value = 0f;
        }
    }

	 public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

        UpdateHealthUI();
        StartCoroutine(ShowDamageVignette());

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

    private IEnumerator ShowDamageVignette()
    {
        if (vignette != null)
        {
            // Fade in
            float elapsedTime = 0f;
            while (elapsedTime < vignetteDuration / 2)
            {
                elapsedTime += Time.deltaTime;
                float intensity = Mathf.Lerp(0f, vignetteMaxIntensity, elapsedTime / (vignetteDuration / 2));
                vignette.intensity.value = intensity;
                yield return null;
            }

            // Fade out
            elapsedTime = 0f;
            while (elapsedTime < vignetteDuration / 2)
            {
                elapsedTime += Time.deltaTime;
                float intensity = Mathf.Lerp(vignetteMaxIntensity, 0f, elapsedTime / (vignetteDuration / 2));
                vignette.intensity.value = intensity;
                yield return null;
            }

            // Ensure vignette is fully transparent at the end
            vignette.intensity.value = 0f;
        }
    }

	private void Die()
    {
        Debug.Log("Player has died!");
        if (gameOverManager != null)
        {
            StartCoroutine(ShowGameOverAfterDelay());
        }
        else
        {
            Debug.Log("GameOver reference hilang! Tidak bisa menampilkan game over screen.");
        }
    }

    private IEnumerator ShowGameOverAfterDelay()
    {
        // tunggu delay
        yield return new WaitForSeconds(restartDelay);

        // Tampilkan game over screen
        gameOverManager.ShowGameOver();
    }
}