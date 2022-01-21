using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform followObject;
    public Vector3 offset;
    public float smoothness = 0.3f;

    Vector3 _velocity;

    private void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, followObject.position + offset, ref _velocity, smoothness);
    }
}
