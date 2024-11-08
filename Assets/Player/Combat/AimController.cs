using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    [Header("Aiming Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float aimFOV = 30f;
    [SerializeField] private float aimSmoothing = 10f;
    
    [Header("Aim Position Settings")]
    [SerializeField] private Vector3 aimPosition = new Vector3(0f, -0.1f, 0.5f);    // Posisi saat aim
    [SerializeField] private Vector3 aimRotation = new Vector3(0f, 0f, 0f);         // Rotasi saat aim
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject Crosshair;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isAiming = false;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (Crosshair != null)
        {
            Crosshair.SetActive(true);
        }

        // Simpan posisi awal
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
        }

        if (isAiming)
        {
            // Nonaktifkan Crosshair saat aiming
            if (Crosshair != null)
            {
                Crosshair.SetActive(false);
            }
        }
        else
        {
            // Aktifkan Crosshair kembali saat tidak aiming
            if (Crosshair != null)
            {
                Crosshair.SetActive(true);
            }
        }

        HandleAiming();
    }
    
    private void HandleAiming()
    {
        float targetFOV = isAiming ? aimFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * aimSmoothing);
        
        if (isAiming)
        {
            // Bergerak ke posisi aim
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aimSmoothing);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(aimRotation), Time.deltaTime * aimSmoothing);
        }
        else
        {
            // Kembali ke posisi normal
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * aimSmoothing);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, startRotation, Time.deltaTime * aimSmoothing);
        }
    }

    // Optional: Method untuk mengubah posisi aim melalui script lain
    public void SetAimPosition(Vector3 newPosition, Vector3 newRotation)
    {
        aimPosition = newPosition;
        aimRotation = newRotation;
    }
}