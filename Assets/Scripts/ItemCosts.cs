using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemCosts", menuName = "ItemCosts")]
public class ItemCosts : ScriptableObject
{
    public List<Item> items;
    public bool isPlayerHasSpeaker = false;
}

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite itemImage;
    public float price;
    public bool isPurchased;
}
