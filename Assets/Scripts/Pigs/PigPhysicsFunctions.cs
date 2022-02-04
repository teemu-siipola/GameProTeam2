using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PigAI))]
public class PigPhysicsFunctions : MonoBehaviour
{
    Rigidbody _rb;
    SphereCollider _col;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<SphereCollider>();
    }

    public void TurnPhysicsOn()
    {
        _rb.isKinematic = false;
        _col.enabled = true; //maybe lerp the collider
    }

    public void TurnPhysicsOff()
    {
        _rb.isKinematic = true;
        _col.enabled = false;
    }

    public void AddForce(Vector3 force)
    {
        _rb.AddForce(force, ForceMode.Impulse);
        _rb.AddTorque(force, ForceMode.Impulse);
    }
}
