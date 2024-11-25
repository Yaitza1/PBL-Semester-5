using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public UnityEvent OnGunShoot;
    public float FireCooldown;

    // By default gun is semi-automatic
    public bool Automatic;

    // reload properties
    public int MaxAmmo = 12;
    public float ReloadTime = 5f;
    public int currentAmmo;
    public Text AmmoDisplay;
    private bool isReloading = false;

    private float CurrentCooldown;

    // Audio components
    public AudioSource audioSourceShoot;
    public AudioSource audioSourceReload;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    // Particle system for muzzle flash
    public ParticleSystem muzzleFlash;

    // Bullet tracer components
    public LineRenderer bulletTracerPrefab;
    public Transform shootPoint;
    public float bulletSpeed = 100f;
    public float tracerDuration = 0.05f;
    public float maxDistance = 100f;

    // New variables for handling movement
    private Vector3 lastPosition;
    private Vector3 velocity;

    void Start()
    {
        CurrentCooldown = FireCooldown;
        currentAmmo = MaxAmmo;
        lastPosition = transform.position;
        UpdateAmmoDisplay();
    }

    void Update()
    {
        // Calculate velocity
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        if (!GameOver.isGameOver)
        if (!PauseMenu.isPaused)
        if (isReloading)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Automatic)
        {
            if (Input.GetMouseButton(0))
            {
                TryShoot();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                TryShoot();
            }
        }
        CurrentCooldown -= Time.deltaTime;
    }

    void TryShoot()
    {
        if (!GameOver.isGameOver)
        if (!PauseMenu.isPaused)
        if (CurrentCooldown <= 0f && currentAmmo > 0)
        {
            OnGunShoot?.Invoke();
            CurrentCooldown = FireCooldown;
            currentAmmo--;
            UpdateAmmoDisplay();
            Debug.Log($"Ammo left: {currentAmmo}");

            // Play muzzle flash particle effect
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }

            // Instantiate bullet tracer
            if (bulletTracerPrefab != null && shootPoint != null)
            {
                StartCoroutine(SpawnBulletTracer());
            }

            // Play shoot sound
            if (shootSound != null)
            {
                audioSourceShoot.PlayOneShot(shootSound);
            }
        }
    }

    IEnumerator SpawnBulletTracer()
    {
        Vector3 startPosition = shootPoint.position;
        Vector3 startForward = shootPoint.forward;

        // Calculate initial velocity of the bullet
        Vector3 bulletVelocity = startForward * bulletSpeed + velocity;

        // Instantiate the LineRenderer (tracer)
        LineRenderer tracer = Instantiate(bulletTracerPrefab, startPosition, Quaternion.identity);
        
        // Set the start point of the tracer
        tracer.SetPosition(0, startPosition);

        float distanceTraveled = 0f;
        float travelTime = 0f;
        Vector3 currentBulletPosition = startPosition;
        Vector3 lastBulletPosition = startPosition;

        while (distanceTraveled < maxDistance)
        {
            lastBulletPosition = currentBulletPosition;
            travelTime += Time.deltaTime;
            currentBulletPosition = startPosition + bulletVelocity * travelTime;
            distanceTraveled = Vector3.Distance(startPosition, currentBulletPosition);

            // Perform raycast from last position to current position
            RaycastHit hit;
            Vector3 direction = currentBulletPosition - lastBulletPosition;
            if (Physics.Raycast(lastBulletPosition, direction.normalized, out hit, direction.magnitude))
            {
                currentBulletPosition = hit.point;
                tracer.SetPosition(1, currentBulletPosition);
                break; // Exit the loop if we hit something
            }

            tracer.SetPosition(1, currentBulletPosition);
            yield return null;
        }

        // Keep the tracer visible for a short duration
        yield return new WaitForSeconds(tracerDuration);

        // Destroy the tracer
        Destroy(tracer.gameObject);
    }

    IEnumerator Reload()
    {
        if (!GameOver.isGameOver)
        if (!PauseMenu.isPaused)
        if (currentAmmo < MaxAmmo && !isReloading)
        {
            isReloading = true;
            Debug.Log("Reloading...");

            // Update ammo display to show reloading
            AmmoDisplay.text = "Reloading...";

            // Play reload sound
            if (reloadSound != null)
            {
                audioSourceReload.PlayOneShot(reloadSound);
            }

            yield return new WaitForSeconds(ReloadTime);

            currentAmmo = MaxAmmo;
            isReloading = false;
            UpdateAmmoDisplay();
            Debug.Log("Reload complete. Ammo refilled.");
        }
        else if (currentAmmo == MaxAmmo)
        {
            Debug.Log("Magazine is already full!");
        }
    }

    void UpdateAmmoDisplay()
    {
        AmmoDisplay.text = currentAmmo.ToString();
    }
}