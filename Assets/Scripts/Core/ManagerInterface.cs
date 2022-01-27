using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* - - - - - - - - - - - - - - - - - - - - - - *
*
*           ~ Singleton Interface ~            
*
*   for if it is needed to make more than one
*   singleton, e.g. AudioManager
*
* - - - - - - - - - - - - - - - - - - - - - - */

public class ManagerInterface<T> : MonoBehaviour where T : Component
{
    private static T singleton;
    public static T Singleton
    {
        get
        {
            if(singleton == null)
                singleton = FindObjectOfType<T>();
                if(singleton == null)
                {
                    GameObject gameObject = new GameObject();
                    gameObject.name = typeof(T).Name;
                    singleton = gameObject.AddComponent<T>();
                }

            return singleton;
        }
    }
    public void Awake()
    {
        if(singleton == null)
        {
            singleton = this as T;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);
    }
}
