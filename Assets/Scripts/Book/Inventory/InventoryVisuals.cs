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

        private readonly Dictionary<IVariable, BaseItemSlot> items = new(); 

        public void Awake()
        {
            inventoryButton.onClick.AddListener(ChangeActiveState);
            closeButton.onClick.AddListener(CloseInventory);
            
            foreach (var slot in freeItemSlots)
                slot.gameObject.SetActive(false);
            
            SetContentOnStart();
            CloseInventory();
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
                var variable = (Variable<int>)VariablesManager.Instance.GetVariableFromName(item.VariableNameInt);
                variable.OnValueChanged += (_, _) => OnItemUpdated(variable, item);
                
                if(variable.Value == 0)
                    return;
                
                AddItem(variable, item);
            }
        }

        private void OnItemUpdated(Variable<int> variable, BaseItemData item)
        {
            if (variable.Value == 0)
            {
                RemoveItem(variable);
                return;
            }
        
            if (items.ContainsKey(variable))
            {
                UpdateItem(variable);
                return;
            }
            
            AddItem(variable, item);
        }
        
        private void AddItem(Variable<int> variable, BaseItemData itemData)
        {
            var slot = freeItemSlots[^1];
            freeItemSlots.RemoveAt(freeItemSlots.Count - 1);
            slot.transform.SetSiblingIndex(items.Count);
            items.Add(variable, slot);
            SetItem(variable, slot, itemData);
        }
        
        private void RemoveItem(Variable<int> variable)
        {
            if(!items.Remove(variable, out var slot))
                return;
            freeItemSlots.Add(slot);
            slot.transform.SetAsLastSibling();
            CleanSlot(slot);
        }
        
        private void UpdateItem(Variable<int> variable)
        {
            if(!items.TryGetValue(variable, out var slot))
               return;
            slot.quantity.SetText(variable.Value.ToString());
        }

        private void SetItem(Variable<int> variable, BaseItemSlot slot, BaseItemData itemData)
        {
            slot.itemName.text = itemData.DisplayName;
            slot.quantity.SetText(variable.Value.ToString());
            slot.image.sprite = itemData.Icon;
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