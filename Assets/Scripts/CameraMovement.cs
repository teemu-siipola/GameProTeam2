using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform followObject;
    Vector3 _velocity;

    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, followObject.position + GameManager.Singleton.variables.cameraOffset, ref _velocity, GameManager.Singleton.variables.cameraSmooth);
    }
}
