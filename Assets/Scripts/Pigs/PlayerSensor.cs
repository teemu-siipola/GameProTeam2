using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* - - - - - - - - - - - - - - - - - - - - - - *
*
*               ~ Player Sensor ~            
*
*   detects if players are near the pig
*
* - - - - - - - - - - - - - - - - - - - - - - */

public class PlayerSensor : MonoBehaviour
{
    private PigAI _parent;

    void Awake()
    {
        _parent = GetComponentInParent<PigAI>();
    }

    void Start()
    {
        print(_parent);
    }

    void OnTriggerStay(Collider collider)
    {
        if(collider.gameObject.layer == 8)
            _parent.OnPlayerSensorTriggerStay(collider);
    }

    void OnTriggerExit(Collider collider)
    {
        if(collider.gameObject.layer == 8)
            _parent.OnPlayerSensorTriggerExit(collider);
    }
}

// yeah, I detect pigs. Deal with it.