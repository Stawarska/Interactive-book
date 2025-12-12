using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace Book.Inventory
{
    public class InventoryVisuals : MonoBehaviour
    {
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private InventoryItemsData inventoryItemsData;
        [SerializeField] private BaseItemSlot itemSlotPrefab;
        [SerializeField] private Transform itemSlotContainer;

        private Dictionary<BaseItemSlot, BaseItemData?> slots = new(); 

        public void Awake()
        {
            inventoryButton.onClick.AddListener(ChangeActiveState);
            closeButton.onClick.AddListener(CloseInventory);
        }

        public void SpawnInventorySlots(int quantity)
        {
            for (var i = 0; i < quantity; i++)
                slots.Add(Instantiate(itemSlotPrefab, itemSlotContainer), null);
        }

        private void ChangeActiveState() => inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        private void CloseInventory() => inventoryPanel.SetActive(false);
    }
}