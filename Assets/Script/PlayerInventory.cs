using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] public Dictionary<Item, int> playerInventory = new Dictionary<Item, int>();
    public TextMeshProUGUI inventoryText;


    public void Awake()
    {
        inventoryText = GameObject.Find("(placeholder) Items").GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            AddItem(new Flashlight());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AddItem(new Bandage());
        }

    }

    private void AddItem(Item item)
    {
        playerInventory.Add(item, playerInventory.Count + 1);

        UpdateInventoryText();
    }

    private void HasItem(Item item)
    {
        
        UpdateInventoryText();
    }

    private void DropItem(Item item)
    {
        
        UpdateInventoryText();
    }

    private void UpdateInventoryText()
    {
        var auxText = "Inventory:\n";
        int[] a = new int[3];
        foreach (var item in playerInventory)
        {
            auxText += $"{item.Key.GetType()} - x{item.Value}\n";
        }
        inventoryText.text = auxText;
    }
}

public abstract class Item
{
    private int _ID = -1;
    protected Item()
    {
        _ID = Random.Range(0, 10000);
    }

    public int GetItemID()
    {
        return this._ID;
    }
}

public class Flashlight : Item
{
    private int BatteryLevel;
    public Flashlight()
    {
        BatteryLevel = Random.Range(10, 20);
    }
}

public class Bandage : Item
{
    private int HealAmount = 10;
}
