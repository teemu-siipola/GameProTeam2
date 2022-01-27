using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVacuum : MonoBehaviour
{
    public ParticleSystem _vfx;
    public float vacuumRadius;
    public float vacuumingAngle;
    public bool debug;

    public int Inventory { get { return _inventory; } }

    bool _isVacuuming;
    int _inventory;
    LayerMask _pigLayer;

    void Awake()
    {
        _pigLayer = 1 << LayerMask.NameToLayer("Pig");
    }

    void Update()
    {
        PlayerInput();
        if (_isVacuuming)
        {
            Vacuum();
        }
    }

    void PlayerInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartVacuum();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            EndVacuum();
        }

        if (debug && Input.GetKeyDown(KeyCode.Mouse2))
            _inventory++;
            
    }

    void StartVacuum()
    {
        _isVacuuming = true;
        if (_vfx != null)
        {
            _vfx.Play();
        }
    }

    void EndVacuum()
    {
        _isVacuuming = false;
        if (_vfx != null)
        {
            _vfx.Stop();
        }
    }

    void Vacuum()
    {
        ScanForPigs();
    }

    void ScanForPigs()
    {
        Collider[] pigs = Physics.OverlapSphere(transform.position, vacuumRadius, _pigLayer);

        if (debug)
        {
            Debug.DrawRay(transform.position, transform.forward * vacuumRadius, Color.red);
        }

        foreach (Collider col in pigs)
        {
            if (PigInSuctionSector(col.transform))
            {
                //Suck pig here
            }
        }
    }

    bool PigInSuctionSector(Transform pig)
    {
        Vector3 playerPos = _vfx.transform.position;
        playerPos.y = 0;
        Vector3 pigPos = pig.position;
        pigPos.y = 0;

        Vector3 playerToPig = pigPos - playerPos;
        float angle = Vector3.Angle(transform.forward, playerToPig);
        return angle < vacuumingAngle; 
    }
}
