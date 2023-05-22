using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
        if(playerInventory.ContainsKey(item))
            playerInventory[item]++;
        else
        {
            playerInventory.Add(item, playerInventory.Count + 1);
        }

        UpdateInventoryText();
    }

    private bool HasItem(Item item)
    {
        if (playerInventory.ContainsKey(item))
            return false;
        return true;
        
        UpdateInventoryText();
    }

    private bool DropItem(Item item)
    {
        if (playerInventory[item] == 0)
            return false;
        return true;
        
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
    
}

public class Flashlight : Item
{
    private int batteryLevel = 5;
    public Flashlight()
    {
        batteryLevel = 12;
    }
    
}

public class Bandage : Item
{
    private int healthAmount = 10;
    public Bandage()
    {
        healthAmount = 5;
    }
    
}
