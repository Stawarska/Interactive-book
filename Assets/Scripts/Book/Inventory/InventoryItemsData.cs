using System.Collections.Generic;
using UnityEngine;

namespace Book.Inventory
{
    public abstract class InventoryItemsData<T> : ScriptableObject 
    {
        [field: SerializeField] public List<T> Items { get; set; }
    }
}