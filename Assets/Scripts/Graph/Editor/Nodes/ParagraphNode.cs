using System;
using System.Linq;
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
        [Input] public ChoiceEditor choice;
        [field: SerializeField] public Page PageTemplate { get; private set; }
        [field: SerializeField, Output(dynamicPortList = true)] public ChoiceEditor[] EditorChoices { get; private set; }
        public Action<Object> OnPageChanged;
        
        [Tooltip("It is created automatically when you select a page template")]
        [field: SerializeField, ReadOnly] public Page PageVariant { get; private set; }
        
        public override object GetValue(NodePort port) {
            return port.fieldName == "Choices" ? EditorChoices : null;
        }
        
        public void CreatePrefabVariant(Object prefabTemplate)
        {
            if (prefabTemplate is not Page page) { return; }

            var instance = PrefabUtility.InstantiatePrefab(page) as Page;
            PageVariant = PrefabUtility.SaveAsPrefabAsset(instance.gameObject, Utils.Consts.PagePrefabsPath + "/" + Utils.Consts.PageNamePrefix + Utils.PageCounter.GetAnotherIndex() + ".prefab").GetComponent<Page>();
            
            DestroyImmediate(instance.gameObject); ;
        }

        public void DeletePagePrefab()
        {
            if(PageVariant == null) { return; }

            if (!Utils.PrefabNameUtils.TryGetIndexFromName(PageVariant.name, out int index))
            {
                Debug.LogError("Can't delete page prefab, page variant name is invalid");
                return;
            }
                
            Utils.PageCounter.ReturnIndexToPool(index);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(PageVariant));
        }

        public void CreateNewChoice()
        {
            var instantiatePrefab = (Page)PrefabUtility.InstantiatePrefab(PageVariant);
            var newChoice = Instantiate(PageVariant.ChoiceTemplate, instantiatePrefab.ChoiceParent);
            
            instantiatePrefab.Choices.Add(newChoice);
            
            PrefabUtility.ApplyPrefabInstance(instantiatePrefab.gameObject, InteractionMode.UserAction);
            DestroyImmediate(instantiatePrefab.gameObject);
            UpdateChoices();
        }

        public void UpdateChoices()
        {
            if (PageVariant == null)
                return;
            
            PageVariant.Choices.Clear();
            PageVariant.OnValidate();

            EditorChoices = PageVariant.Choices.Select(x =>
            {
                var choiceEditor = new ChoiceEditor
                {
                    choice = x
                };
                
                return choiceEditor;
            }).ToArray();
        }

        protected override void Init()
        {
            base.Init();
            if(PageVariant == null)
                return;

            ChoiceEditorDrawer.OnContentChanged += node =>
            {
                UpdateChoices();
            };
            UpdateChoices();
        }

        private void OnDestroy()
        {
            DeletePagePrefab();
        }
    }
}