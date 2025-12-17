using System.Collections.Generic;
using GlobalVariable;
using KBCore.Refs;
using SaintsField.Playa;
using UnityEngine;
using UnityEngine.UI;

namespace Book.Inventory
{
    public class InventoryVisuals : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform itemSlotContainer;

        [Header("Settings")]
        [SerializeField] private InventoryBaseItemData inventoryItemsData;
        [SerializeField] private BaseItemSlot itemSlotPrefab;
        [SerializeField] private int slotsQuantity;
        [SerializeField, Child] private List<BaseItemSlot> freeItemSlots;

        private readonly Dictionary<string, BaseItemSlot> items = new(); 

        public void Awake()
        {
            inventoryButton.onClick.AddListener(ChangeActiveState);
            closeButton.onClick.AddListener(CloseInventory);

            VariablesManager.Instance.OnVariableChanged += UpdateSlotContent;
            SetContentOnStart();
        }

        [Button("Spawn Slots")]
        public void SpawnInventorySlots()
        {
            DestroyAllSlots();
            for (var i = 0; i < slotsQuantity; i++)
                Instantiate(itemSlotPrefab, itemSlotContainer);
            OnValidate();
        }
        
        [Button("Remove all slots")]
        public void DestroyAllSlots()
        {
            foreach (var itemSlot in freeItemSlots)
                DestroyImmediate(itemSlot.gameObject);
            
            OnValidate();
        }

        private void SetContentOnStart()
        {
            foreach (var item in inventoryItemsData.Items)
            {
                var variable = GlobalVariables.instance.GetVariableFromName(item.VariableNameInt);
                var value = variable.variableType.GetValue<int>();
                
                if(value == 0)
                    return;
                
                AddItem(variable);
            }
        }

        private void UpdateSlotContent(GlobalVariables.Variable variable)
        {
            if (variable.variableType.GetValue<int>() == 0)
            {
                RemoveItem(variable);
                return;
            }

            if (items.ContainsKey(variable.variableName))
            {
                UpdateItem(variable);
                return;
            }
            
            AddItem(variable);
        }

        private void AddItem(GlobalVariables.Variable variable)
        {
            var slot = freeItemSlots[^1];
            freeItemSlots.RemoveAt(freeItemSlots.Count - 1);
            slot.transform.SetSiblingIndex(items.Count);
            items.Add(variable.variableName, slot);
            SetItem(variable, slot);
        }

        private void RemoveItem(GlobalVariables.Variable variable)
        {
            if(!items.Remove(variable.variableName, out var slot))
                return;
            freeItemSlots.Add(slot);
            slot.transform.SetAsLastSibling();
            CleanSlot(slot);
        }

        private void UpdateItem(GlobalVariables.Variable variable)
        {
            if(!items.TryGetValue(variable.variableName, out var slot))
               return;
            slot.quantity.SetText(variable.variableType.GetValue<int>().ToString());
        }

        private void SetItem(GlobalVariables.Variable variable, BaseItemSlot slot)
        {
            var baseItemData= inventoryItemsData.GetItem(variable.variableName);
            
            slot.itemName.text = variable.variableName;
            slot.quantity.SetText(variable.variableType.GetValue<int>().ToString());
            slot.image.sprite = baseItemData.Icon;
            slot.gameObject.SetActive(true);
        }

        private static void CleanSlot(BaseItemSlot slot)
        {
            slot.gameObject.SetActive(false);
        }

        private void ChangeActiveState() => inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        private void CloseInventory() => inventoryPanel.SetActive(false);
        private void OnValidate() => this.ValidateRefs();
    }
}