using System.Collections.Generic;
using Book.Animation;
using Book.Inventory;
using Graph;
using SaintsField;
using SaintsField.Playa;
using UnityEditor;
using UnityEngine;

namespace Book
{
    public class BookManager : MonoBehaviour
    {
        public static BookManager Instance;
        
        [Header("Graph")]
        [SerializeField] private BookGraph bookGraph;
        
        [Header("Animation")]
        [ResourcePath(typeof(ISwapPageAnimation)), SerializeField] private string pageAnimationPath;
        [SerializeField, ReadOnly] private string previousPageAnimationPath;
        [SerializeField, ReadOnly] private GameObject pageAnimationPrefab;
        [SerializeField] private Transform animationParent;
        
        [Header("Inventory")]
        [SerializeField] private bool useInventory;
        [ShowIf(nameof(useInventory)), ResourcePath(typeof(InventoryVisuals)), SerializeField] private string inventoryVisualsPath;
        [ShowIf(nameof(useInventory)), SerializeField, ReadOnly] private string previousInventoryPath;
        [ShowIf(nameof(useInventory)), SerializeField, ReadOnly] private GameObject inventoryPrefab;
        [ShowIf(nameof(useInventory)), SerializeField] private Transform inventoryParent;

        private Stack<Page> pagesHistory = new();
        private Page targetPage;
        private ISwapPageAnimation pageAnimation;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            pageAnimation = pageAnimationPrefab.GetComponent<ISwapPageAnimation>();
            SelectNextPage(bookGraph.GetFirstPage());
        }
        
        public void SelectNextPage(Page page)
        {
            if(targetPage != null)
                pagesHistory.Push(targetPage);
            SelectPage(page);
        }
    
        public void SelectPreviousPage()
        {
            if (pagesHistory == null || pagesHistory.Count == 0)
                return;
    
            var nextPage = pagesHistory.Pop();
        
            SelectPage(nextPage);
        }
    
        private void SelectPage(Page page)
        {
            targetPage = page;
            pageAnimation.FlipPage(page);
        }

#if UNITY_EDITOR
        
        [Button("Apply changes")]
        private void ApplyChanges()
        {
            SpawnPageAnimationPrefab();
            SwapInventoryPrefab();
        }
        
        private void SpawnPageAnimationPrefab()
        {
            if (pageAnimationPath == null)
            {
                Debug.LogError("Page animation is null, please pick animation first");
                previousPageAnimationPath = null;
                DestroyImmediate(pageAnimationPrefab.gameObject);
                pageAnimation = null;
                return;
            }

            if (previousPageAnimationPath == pageAnimationPath)
            {
                Debug.Log("Animation was not changed, nothing happened");
                return;
            }
            
            if (pageAnimationPrefab != null)
            {
                DestroyImmediate(pageAnimationPrefab.gameObject);
                pageAnimationPrefab = null;
                pageAnimation = null;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{pageAnimationPath}.prefab");
            pageAnimationPrefab = Instantiate(prefab, animationParent);
            previousPageAnimationPath = pageAnimationPath;
        }

        private void SwapInventoryPrefab()
        {
            if (!useInventory)
            {
                if(inventoryPrefab != null)
                    DestroyImmediate(inventoryPrefab.gameObject);
                inventoryPrefab = null;
                return;
            }
            
            if (inventoryVisualsPath == null)
            {
                Debug.LogError("Inventory is null, please pick inventory first or deselect use inventory");
                previousInventoryPath = null;
                DestroyImmediate(inventoryPrefab.gameObject);
                inventoryPrefab = null;
                return;
            }

            if (previousInventoryPath == inventoryVisualsPath)
            {
                Debug.Log("Inventory was not changed, nothing happened");
                return;
            }

            if (inventoryPrefab != null)
            {
                DestroyImmediate(inventoryPrefab.gameObject);
                inventoryPrefab = null;
            }
            
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{inventoryVisualsPath}.prefab");
            inventoryPrefab = Instantiate(prefab, inventoryParent);
            previousInventoryPath = inventoryVisualsPath;
        }
#endif
    }
}
