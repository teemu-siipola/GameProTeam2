using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugVictoryBox : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.transform.root.GetComponent < PlayerMovement>() != null)
        {
            GameManager.Singleton.ValidateAddPigs(1000);
        }
    }
}
