using System;
using System.Linq;
using Book;
using Nodes;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Utils;
using XNodeEditor;
using Object = UnityEngine.Object;

namespace Graph.Editor.Nodes
{
    [CustomNodeEditor(typeof(ParagraphNode))]
    public class ParagraphNodeEditor : NodeEditor
    {
        private ParagraphNode node;
        private Texture2D cachedPreview;
        private Page lastPrefab;

        private ReorderableList list;

        private readonly string[] excludes =
        {
            "m_Script", "graph", "position", "ports",
            SerializationExtensions.GetBackingFieldName(nameof(ParagraphNode.PageTemplate)),
            SerializationExtensions.GetBackingFieldName(nameof(ParagraphNode.EditorChoices))
        };

        private Object templatePreviousValue;
        private bool isContentChanged;

        public override void OnCreate()
        {
            base.OnCreate();
            ChoiceEditor.OnChoiceRemoved += node =>
            {
                if (!ReferenceEquals(node, this.node))
                    return;
                list.onRemoveCallback?.Invoke(list);
            };

            ChoiceEditor.OnContentChanged += node =>
            {
                if (!ReferenceEquals(node, this.node))
                    return;
                ContentChanged();
            };
        }

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            node = target as ParagraphNode;
        }

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            DrawPorts();
            DrawPageTemplate();
            DrawPageTemplateContent();

            foreach (var dynamicPort in target.DynamicPorts)
            {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreatePagePreview()
        {
            if (lastPrefab != node.PageVariant || isContentChanged)
            {
                cachedPreview = null;
                lastPrefab = node.PageVariant;
                isContentChanged = false;
            }

            var isUIElement = node.PageVariant.TryGetComponent(out RectTransform _);

            if (isUIElement && cachedPreview == null)
            {
                var bookManager = Object.FindFirstObjectByType<BookManager>();
                cachedPreview = GeneratePreview.GeneratePagePreview(node.PageVariant.gameObject, GeneratePreview.PageSection.Full, new Vector2(bookManager.BookWidth, bookManager.BookHeight));
            }

            var preview = isUIElement ? cachedPreview : AssetPreview.GetAssetPreview(node.PageVariant);

            if (preview == null && !isUIElement)
            {
                EditorGUILayout.HelpBox("Loading preview...", MessageType.Info);
                return;
            }

            if (preview == null && isUIElement)
            {
                EditorGUILayout.HelpBox("Can't generate preview", MessageType.Warning);
                return;
            }

            var previewSize = 400f;
            var previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);

            var width = Mathf.Min(previewSize, previewRect.width);
            previewRect.x += (previewRect.width - width) / 2;
            previewRect.width = width;
            previewRect.height = width;

            EditorGUI.DrawRect(
                new Rect(previewRect.x - 1, previewRect.y - 1, previewRect.width + 2, previewRect.height + 2),
                Color.gray);
            EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f));

            GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);

            EditorGUILayout.LabelField(node.PageTemplate.name, EditorStyles.centeredGreyMiniLabel);
        }
        
        #region DrawElements

        private void DrawPorts()
        {
            var choices = node.EditorChoices;
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator);
            }
        }

        private void DrawPageTemplate()
        {
            var pageTemplateProperty = serializedObject.FindAutoProperty("PageTemplate");

            if (pageTemplateProperty == null)
            {
                EditorGUILayout.HelpBox("Nie znaleziono property 'pageTemplate'", MessageType.Error);
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(pageTemplateProperty, new GUIContent("Page template"), true);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                var newValue = pageTemplateProperty.objectReferenceValue;

                if (templatePreviousValue == null && newValue != null)
                    node.CreatePrefabVariant(newValue);

                else if (templatePreviousValue != null && newValue == null)
                    node.DeletePagePrefab();

                else if (templatePreviousValue != newValue)
                    node.OnPageTemplateChanged(newValue);

                templatePreviousValue = newValue;

                serializedObject.Update();
                NodeEditorWindow.RepaintAll();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawChoices()
        {
            var previousChoicesCount = lastPrefab?.Choices.Count;
            serializedObject.ApplyModifiedProperties();
            var property = serializedObject.FindAutoProperty(nameof(node.EditorChoices));

            Type type = NodeEditorGUILayout.GetType(property);

            XNode.NodePort port = node.GetPort(property.name);

            NodeEditorGUILayout.DynamicPortList(property.name, type, property.serializedObject, port.direction,
                XNode.Node.ConnectionType.Override, onCreation: OnCreation, displayAddRemoveButtons: false);

            if (GUILayout.Button("Add Choice"))
            {
                node.CreateNewChoice();
                list.onAddCallback?.Invoke(list);
            }

            void OnCreation(ReorderableList list)
            {
                this.list = list;
                list.onReorderCallback += OnReorderCallback;
            }

            void OnReorderCallback(ReorderableList reorderableList)
            {
                var choices = node.EditorChoices;

                for (var i = 0; i < choices.Length; i++)
                {
                    for (var j = 0; j < node.PageVariant.Choices.Count; j++)
                    {
                        if(!ReferenceEquals(choices[i].choice, node.PageVariant.Choices[j]))
                            continue;
                        
                        node.PageVariant.Choices[j].transform.SetAsLastSibling();
                    }
                }
                ContentChanged();
            }
            
            serializedObject.Update();
            
            if(previousChoicesCount == lastPrefab?.Choices.Count)
                return;
            
            node.UpdateChoices();
            ContentChanged();
        }

        private void DrawPageTemplateContent()
        {
            if (node.PageVariant == null)
                return;
            if (!node.PageVariant.TryGetComponent(out Page page))
                return;

            foreach (var content in page.PageContents)
            {
                content.DrawEditor();
                content.OnContentChanged += ContentChanged;
            }

            DrawChoices();

            EditorGUILayout.Space(5);
            CreatePagePreview();
        }

        #endregion
        
        private void ContentChanged()
        {
            isContentChanged = true;
        }

        public override int GetWidth()
        {
            return 400;
        }
    }
}