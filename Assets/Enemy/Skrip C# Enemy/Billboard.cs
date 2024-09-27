using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public LayerMask PlayerCameraLayer;
    public Transform cameraTransform;

    void Start()
    {
        FindPlayerCamera();
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }
        else
        {
            FindPlayerCamera();
        }
    }

    void FindPlayerCamera()
    {
        Camera playerCamera = Camera.main;
        if (playerCamera != null && ((1 << playerCamera.gameObject.layer) & PlayerCameraLayer) != 0)
        {
            cameraTransform = playerCamera.transform;
        }
        else
        {
            Debug.LogWarning("Player camera not found on the specified layer.");
        }
    }
}