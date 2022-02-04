using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PigAI))]
public class PigPhysicsFunctions : MonoBehaviour
{
    Rigidbody _rb;
    SphereCollider _col;
    float _originalColRadius;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<SphereCollider>();
        _originalColRadius = _col.radius;
    }

    public void TurnPhysicsOn()
    {
        _rb.isKinematic = false;
        _col.radius = _originalColRadius;
    }

    public void TurnPhysicsOff()
    {
        _rb.isKinematic = true;
        _col.radius = 0f; //we don't disable collider so trigger exit still gets called
    }

    public void AddForce(Vector3 force)
    {
        _rb.AddForce(force, ForceMode.Impulse);
        _rb.AddTorque(force, ForceMode.Impulse);
    }
}
