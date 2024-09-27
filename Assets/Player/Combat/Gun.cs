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

    void Start()
    {
        CurrentCooldown = FireCooldown;
        currentAmmo = MaxAmmo;
    }

    void Update()
    {
        AmmoDisplay.text = currentAmmo.ToString();
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
        if (CurrentCooldown <= 0f && currentAmmo > 0)
        {
            OnGunShoot?.Invoke();
            CurrentCooldown = FireCooldown;
            currentAmmo--;
            Debug.Log($"Ammo left: {currentAmmo}");

            // Play shoot sound
            if (shootSound != null)
            {
                audioSourceShoot.PlayOneShot(shootSound);
            }
        }
    }

    IEnumerator Reload()
    {
        if (currentAmmo < MaxAmmo && !isReloading)
        {
            isReloading = true;
            Debug.Log("Reloading...");

            // Play reload sound
            if (reloadSound != null)
            {
                audioSourceReload.PlayOneShot(reloadSound);
            }

            yield return new WaitForSeconds(ReloadTime);

            currentAmmo = MaxAmmo;
            isReloading = false;
            Debug.Log("Reload complete. Ammo refilled.");
        }
        else if (currentAmmo == MaxAmmo)
        {
            Debug.Log("Magazine is already full!");
        }
    }
}
