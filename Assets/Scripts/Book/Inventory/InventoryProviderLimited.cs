using System.Collections.Generic;
using UnityEngine;

namespace Book.Inventory
{
    // public class InventoryProviderLimited : MonoBehaviour, IInventoryProvider
    // {
    //     [field: SerializeField] public int InventorySize { get; private set; }
    //
    //     public Item? TryAddItem(Item item)
    //     {
    //         if (Items.Count >= InventorySize && !Items.Contains(item))
    //             return item;
    //         
    //         var quantityToAdd = item.Quantity;
    //
    //         if (Items.Contains(item))
    //         {
    //             if (quantityToAdd <= item.ItemData.MaxQuantity - item.Quantity)
    //             {
    //                 Items[Items.IndexOf(item)] = new Item(item.ItemData, item.Quantity + quantityToAdd);
    //                 return null;
    //             }
    //             
    //             quantityToAdd -= item.ItemData.MaxQuantity;
    //         }
    //         
    //         while(true)
    //         {
    //             if (item.ItemData.MaxQuantity >= quantityToAdd)
    //             {
    //                 Items.Add(new Item(item.ItemData, quantityToAdd));
    //                 return null;
    //             }
    //                 
    //             Items.Add(new Item(item.ItemData, item.ItemData.MaxQuantity));
    //             quantityToAdd -= item.ItemData.MaxQuantity;
    //             
    //             if (Items.Count >= InventorySize)
    //                 return new Item(item.ItemData, quantityToAdd);
    //         }
    //     }
    //
    //     public Item? TryRemoveItem(Item item)
    //     {
    //         if (!Items.Contains(item))
    //             return item;
    //
    //         var quantityToRemove = item.Quantity;
    //         while (true)
    //         {
    //             var currentItem = Items[Items.IndexOf(item)];
    //             if (currentItem.Quantity > quantityToRemove)
    //             {
    //                 Items[Items.IndexOf(item)] = new Item(item.ItemData, item.Quantity - quantityToRemove);
    //                 return null;
    //             }
    //
    //             if (currentItem.Quantity == quantityToRemove)
    //             {
    //                 Items.Remove(item);
    //                 return null;
    //             }
    //             
    //             quantityToRemove -= currentItem.Quantity;
    //             Items.Remove(item);
    //             
    //             if (!Items.Contains(item))
    //                 return new Item(item.ItemData, quantityToRemove);
    //         }
    //     }
    //
    //     public List<Item> Items { get; } = new();
    // }
}