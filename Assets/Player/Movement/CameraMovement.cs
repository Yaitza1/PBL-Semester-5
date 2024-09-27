using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform PosisiCamera;

    private void Update()
    {
        transform.position = PosisiCamera.position;
    }
}
