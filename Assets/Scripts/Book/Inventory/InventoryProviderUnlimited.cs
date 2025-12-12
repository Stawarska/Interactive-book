using System.Collections.Generic;
using UnityEngine;

namespace Book.Inventory
{
    // public class InventoryProviderUnlimited : MonoBehaviour, IInventoryProvider
    // {
    //     public bool TryAddItem(ItemData item, int quantity, out int quantityOutOfRange)
    //     {
    //         var index = Items.FindIndex(x => x == item);
    //         if (index == -1)
    //         {
    //             Items.Add(item);
    //             return true;
    //         }
    //         var existingItem = Items[index];
    //         existingItem. += item.Quantity;
    //         Items[index] = existingItem;
    //         return true;
    //     }
    //
    //     public bool TryRemoveItem(ItemData item,  int quantity, out int quantityOutOfRange)
    //     {
    //         var index = Items.FindIndex(x => x.ItemData == item.ItemData);
    //         
    //         if (index == -1)
    //             return item;
    //         
    //         var existingItem = Items[index];
    //         if (existingItem.Quantity < item.Quantity)
    //         {
    //             item.Quantity -= existingItem.Quantity;
    //             Items.RemoveAt(index);
    //             return item;
    //         }
    //         existingItem.Quantity -= item.Quantity;
    //         return null;
    //     }
    //
    //     public List<ItemData> Items { get; } = new();
    // }
}