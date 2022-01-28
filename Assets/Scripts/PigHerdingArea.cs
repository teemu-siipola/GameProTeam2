using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigHerdingArea : MonoBehaviour
{
    int _pigCount;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pig"))
        {
            GameManager.Singleton.ValidateAddPigs(1);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pig"))
        {
            GameManager.Singleton.ValidateRemovePigs(1);
        }
    }
}
