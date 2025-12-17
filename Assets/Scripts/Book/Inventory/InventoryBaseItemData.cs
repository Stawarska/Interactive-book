using UnityEngine;

namespace Book.Inventory
{
    [CreateAssetMenu(fileName = "InventoryItemsData", menuName = "Inventory/Inventory Base Items Data")]
    public class InventoryBaseItemData : InventoryItemsData<BaseItemData>
    {
        public BaseItemData GetItem(string variableName)
        {
            return Items.Find(x => x.VariableNameInt == variableName);
        }
    }
}