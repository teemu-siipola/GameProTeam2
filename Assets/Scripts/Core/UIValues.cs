using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIValues : MonoBehaviour
{
    public PlayerVacuum playerReference;
    public TMPro.TextMeshProUGUI inventory;
    public TMPro.TextMeshProUGUI timer;
    public TMPro.TextMeshProUGUI objective;

    int inventoryPigs;
    int totalPigs;
    uint fencedPigs;
    int seconds;

    void Start()
    {
        if (playerReference == null)
        {
            GameObject scenePlayer = GameObject.Find("Player");
            if (scenePlayer) playerReference = scenePlayer.GetComponent<PlayerVacuum>();
        }
        SetUI();
    }
    void Update()
    {
        ReadInventory();
        ReadSeconds();
        ReadObjective();
    }

    void SetUI()
    {
        WriteObjective();
        WriteInventory();
    }

    void ReadInventory()
    {
        if (playerReference == null) return;

        if (inventoryPigs != playerReference.Inventory)
        {
            inventoryPigs = playerReference.Inventory; ;
            WriteInventory();
        }
    }

    void WriteInventory()
    {
        inventory.text = inventoryPigs.ToString();
    }

    void ReadSeconds()
    {
        int readSeconds = (int)GameManager.Singleton.TimeRemaining;
        if (readSeconds != seconds)
        {
            seconds = readSeconds;
            WriteSeconds();
        }
    }

    void WriteSeconds()
    {
        int minutes = seconds / 60;
        timer.text = minutes.ToString() + ":" + (seconds - 60 * minutes).ToString();
    }

    void ReadObjective()
    {
        uint fenced = GameManager.Singleton.PigsFenced;
        if (fenced != fencedPigs)
        {
            fencedPigs = fenced;
            WriteObjective();
        }
    }

    void WriteObjective()
    {
        objective.text = GameManager.Singleton.PigsFenced.ToString() + " / " + GameManager.Singleton.PigsRequired.ToString();
    }
}
