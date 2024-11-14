using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    [System.Serializable]
    public class AimSettings
    {
        public float normalFOV = 60f;
        public float aimFOV = 30f;
        public float smoothing = 10f;
        public Vector3 position = new Vector3(-0.926f, -0.1f, 0.5f);
        public Vector3 rotation = Vector3.zero;
    }

    [Header("Aiming Settings")]
    [SerializeField] private AimSettings aimSettings = new AimSettings();
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject crosshair;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isAiming;
    private float currentFOV;

    private void Start()
    {
        InitializeReferences();
        SaveStartTransform();
    }
    
    private void Update()
    {
        UpdateAimingState();
        UpdateCrosshairVisibility();
        HandleAiming();
    }

    private void InitializeReferences()
    {
        playerCamera = playerCamera ? playerCamera : Camera.main;
        if (crosshair) crosshair.SetActive(true);
    }

    private void SaveStartTransform()
    {
        startPosition = transform.localPosition;
        startRotation = transform.localRotation;
        currentFOV = playerCamera.fieldOfView;
    }

    private void UpdateAimingState()
    {
        if (Input.GetMouseButtonDown(1)) isAiming = true;
        else if (Input.GetMouseButtonUp(1)) isAiming = false;
    }

    private void UpdateCrosshairVisibility()
    {
        if (crosshair) crosshair.SetActive(!isAiming);
    }
    
    private void HandleAiming()
    {
        float targetFOV = isAiming ? aimSettings.aimFOV : aimSettings.normalFOV;
        Vector3 targetPosition = isAiming ? aimSettings.position : startPosition;
        Quaternion targetRotation = isAiming ? Quaternion.Euler(aimSettings.rotation) : startRotation;

        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * aimSettings.smoothing);
        playerCamera.fieldOfView = currentFOV;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * aimSettings.smoothing);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * aimSettings.smoothing);
    }

    public void SetAimPosition(Vector3 newPosition, Vector3 newRotation)
    {
        aimSettings.position = newPosition;
        aimSettings.rotation = newRotation;
    }
}