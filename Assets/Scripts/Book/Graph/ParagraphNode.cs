using System;
using System.Linq;
using Book;
using Graph.Editor.Nodes;
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
        
        [Tooltip("It is created automatically when you select a page template")]
        [field: SerializeField, ReadOnly] public Page PageVariant { get; private set; }
        
        public override object GetValue(NodePort port) {
            
            return port.fieldName == "Choices" ? EditorChoices : null;
        }

#if UNITY_EDITOR
        //TODO: Przenieść funkcjonalności edytorowe do ParagraphNodeEditor

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            UpdateChoicesConnections();
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            UpdateChoicesConnections();
        }

        public void CreatePrefabVariant(Object prefabTemplate)
        {
            if (prefabTemplate is not Page page) { return; }
            
            var instance = UnityEditor.PrefabUtility.InstantiatePrefab(page) as Page;
            PageVariant = UnityEditor.PrefabUtility.SaveAsPrefabAsset(instance.gameObject, Utils.Consts.PagePrefabsPath + "/" + Utils.Consts.PageNamePrefix + Utils.PageCounter.GetAnotherIndex() + ".prefab").GetComponent<Page>();
            
            DestroyImmediate(instance.gameObject);
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
            
            UnityEditor.AssetDatabase.DeleteAsset(UnityEditor.AssetDatabase.GetAssetPath(PageVariant));
        }
        
        public void OnPageTemplateChanged(Object newValue)
        {
            DeletePagePrefab();
            CreatePrefabVariant(newValue);
            var inputChoice = GetInputValue<ChoiceEditor>(nameof(choice));
            if(inputChoice != null)
                inputChoice.choice.ConnectedPage = PageVariant;
        }

        public void CreateNewChoice()
        {
            var instantiatePrefab = (Page)UnityEditor.PrefabUtility.InstantiatePrefab(PageVariant);
            var newChoice = Instantiate(PageVariant.ChoiceTemplate, instantiatePrefab.ChoiceParent);
            
            instantiatePrefab.Choices.Add(newChoice);
            
            UnityEditor.PrefabUtility.ApplyPrefabInstance(instantiatePrefab.gameObject, UnityEditor.InteractionMode.UserAction);
            DestroyImmediate(instantiatePrefab.gameObject);
            UpdateChoices();
        }
        
        [ContextMenu("Update Choices Connections")]
        public void UpdateChoicesConnections()
        {
            if(EditorChoices == null || EditorChoices.Length == 0)
                return;
            
            for(var i = 0; i < EditorChoices.Length; i++)
            {
                var portName = $"{Utils.SerializationExtensions.GetBackingFieldName(nameof(EditorChoices))} {i}";
                var outputPort = GetOutputPort(portName);
                var currentChoice = EditorChoices[i].choice;

                if (outputPort == null)
                {
                    Debug.LogWarning($"Port {portName} not exists!");
                    return;
                }

                if (!outputPort.IsConnected)
                {
                    Debug.LogWarning($"Port {portName} is not connected!");
                    SetConnectedPageValue(currentChoice, null);
                    return;
                }

                var otherPort = outputPort.Connection;
                var nextNode = otherPort.node as ParagraphNode;
                
                SetConnectedPageValue(currentChoice, nextNode?.PageVariant);
            }
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
            UpdateChoicesConnections();
        }

        protected override void Init()
        {
            base.Init();
            
            UpdateChoicesConnections();
            
            if(PageVariant == null)
                return;

            ChoiceEditor.OnContentChanged += node =>
            {
                UpdateChoices();
            };
            UpdateChoices();
        }
        
        private void SetConnectedPageValue(Choice currentChoice, Page newValue)
        {
            UnityEditor.Undo.RecordObject(currentChoice, "Connected Page Change");
            currentChoice.ConnectedPage= newValue;
            UnityEditor.EditorUtility.SetDirty(currentChoice);
        }

        private void OnDestroy()
        {
            DeletePagePrefab();
        }
    }
}
#endif