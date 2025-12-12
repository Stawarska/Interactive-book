using System.Collections.Generic;
using UnityEngine;

namespace Book.Inventory
{
    [CreateAssetMenu(fileName = "InventoryItemsData", menuName = "Inventory/Inventory Items Data")]
    public class InventoryItemsData : ScriptableObject
    {
        [field: SerializeReference, SubclassSelector] public List<IItemData> Items { get; set; } 
    }
}