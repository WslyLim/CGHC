using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public enum ItemCategory { Items, Sp }
public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;

    public event Action OnUpdated;

    public List<ItemSlot> Slots => slots;

    List<ItemSlot> allSlots;

    public void Awake()
    {
        //allSlots = new List<List<ItemSlot>>() { slots };
    }

    public ItemBase UseItem(int indexItem, Monster selectedMonster)
    {
        var item = slots[indexItem].Item;
        bool itemUsed = item.Use(selectedMonster);
        if (itemUsed) 
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    //ItemCategory GetCategoryFromItem(ItemBase item) 
    //{
        
    //}

    //public List<ItemSlot> GetSlotByCategory(int categoryIndex)
    //{
    //    return allSlots[categoryIndex];
    //}

    public void AddItem(ItemBase item, int count = 1)
    {
        var itemSlot = slots.FirstOrDefault(slot => slot.Item == item);

        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            slots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdated?.Invoke();
    }

    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0 )
            slots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}

[Serializable]

public class ItemSlot
{
    [SerializeField] ItemBase item;
    [SerializeField] int count;

    public ItemBase Item 
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
    
}
