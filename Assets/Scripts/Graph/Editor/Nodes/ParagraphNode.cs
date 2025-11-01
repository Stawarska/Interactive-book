using System;
using Book;
using Graph.Editor.Nodes;
using UnityEditor;
using UnityEngine;
using XNode;
using Object = UnityEngine.Object;
using SaintsField;

namespace Nodes
{
    [Serializable]
    public class ParagraphNode : Node
    {
        [Input] public Choice inputChoices;
        [field: SerializeField] public Page PageTemplate { get; private set; }
        [field: SerializeField, Output(dynamicPortList = true)] public Choice[] Choices { get; private set; }
        public Action<Object> OnPageChanged;
        
        [Tooltip("It is created automatically when you select a page template")]
        [field: SerializeField, ReadOnly] public Page PageVariant { get; private set; }
        private int index;
        
        public override object GetValue(NodePort port) {
            return port.fieldName == "Choices" ? Choices : null;
        }
        
        public void CreatePrefabVariant(Object prefabTemplate)
        {
            if (prefabTemplate is not Page page) { return; }

            var instance = PrefabUtility.InstantiatePrefab(page) as Page;
            index = Utils.PageCounter.GetAnotherIndex();
            PageVariant = PrefabUtility.SaveAsPrefabAsset(instance.gameObject, Utils.Consts.PagePrefabsPath + "/" + "Page" + index + ".prefab").GetComponent<Page>();
            
            DestroyImmediate(instance.gameObject); ;
        }

        public void DeletePagePrefab()
        {
            if(PageVariant == null) { return; }
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(PageVariant));
            Utils.PageCounter.ReturnIndexToPool(index);
        }
        
        private void OnDestroy()
        {
            DeletePagePrefab();
        }
    }
}