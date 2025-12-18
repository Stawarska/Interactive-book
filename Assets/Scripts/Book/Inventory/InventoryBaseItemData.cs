using UnityEngine;

namespace Book.Inventory
{
    [CreateAssetMenu(fileName = "InventoryItemsData", menuName = "Inventory/Inventory Base Items Data")]
    public class InventoryBaseItemData : InventoryItemsData<BaseItemData>
    {
        public bool GetItem(string variableName, out BaseItemData itemData)
        {
            itemData = Items.Find(x => x.VariableNameInt == variableName);
            return itemData.VariableNameInt != null;
        }
    }
}