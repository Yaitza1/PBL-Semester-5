using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGun : MonoBehaviour
{
    public float damage = 20f;
    public float bulletRange = 100f;
    private Camera playerCamera;
    public GameObject ImpactEffect;
 
    private void Start()
    {
        playerCamera = Camera.main;
    }

    public void Shoot()
    {
        Ray gunRay = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(gunRay, bulletRange);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damage);
                GameObject impactGO = Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
                break;
            }
        }
    }
}