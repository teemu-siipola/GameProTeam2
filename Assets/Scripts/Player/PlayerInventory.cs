using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public Sprite filled, empty;
    Camera _camera;
    int _storedPigs = 0;
    Image[] slots;

    void Awake()
    {
        _camera = Camera.main;
        slots = GetComponentsInChildren<Image>();
    }

    void Update()
    {
        transform.LookAt(_camera.transform.position);
    }

    public void AddPig()
    {
        _storedPigs++;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = _storedPigs > i ? filled : empty;
        }
    }

    public void RemovePig()
    {
        _storedPigs--;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = _storedPigs > i ? filled : empty;
        }
    }
}
