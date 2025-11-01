using System.Linq;
using Book;
using Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using XNodeEditor;

namespace Graph.Editor.Nodes
{
    [CustomNodeEditor(typeof(ParagraphNode))]
    public class ParagraphNodeEditor : NodeEditor
    {
        private ParagraphNode node;
        private Texture2D cachedPreview;
        private Page lastPrefab;
        private readonly string[] excludes = { "m_Script", "graph", "position", "ports", "<PageTemplate>k__BackingField"};
        private Object templatePreviousValue;
        private bool isContentChanged;

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
                cachedPreview = GenerateUIPreview(node.PageVariant);
            }
            
            var preview = isUIElement ? cachedPreview : AssetPreview.GetAssetPreview(node.PageVariant);

            if (preview == null && !isUIElement)
            {
                EditorGUILayout.HelpBox("Ładowanie podglądu...", MessageType.Info);
                return;
            }

            if (preview == null && isUIElement)
            {
                EditorGUILayout.HelpBox("Nie można wygenerować podglądu", MessageType.Warning);
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

        private Texture2D GenerateUIPreview(Page uiPrefab)
        {
            var tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(uiPrefab.gameObject);

            if (tempInstance == null)
                return null;

            try
            {
                if(!tempInstance.TryGetComponent<RectTransform>(out var rectTransform))
                    return null;

                var canvasObj = new GameObject("TempCanvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10;
                
                tempInstance.transform.SetParent(canvasObj.transform, false);
                
                var width = EditorPrefs.GetFloat(Consts.BookSizeX);
                var height = EditorPrefs.GetFloat(Consts.BookSizeY);
                
                canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                
                var camObj = new GameObject("TempCamera");
                var camera = camObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                camera.orthographic = true;
                camera.orthographicSize = 600;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 1000f;

                camObj.transform.position = new Vector3(0, 0, -10);
                
                var renderTexture = new RenderTexture((int)width, (int)height, 24);
                camera.targetTexture = renderTexture;
                
                camera.Render();
                
                RenderTexture.active = renderTexture;
                var preview = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                preview.ReadPixels(new Rect(0, 0, (int)width, (int)height), 0, 0);
                preview.Apply();
                
                RenderTexture.active = null;
                camera.targetTexture = null;
                Object.DestroyImmediate(renderTexture);
                Object.DestroyImmediate(camObj);
                Object.DestroyImmediate(canvasObj);

                return preview;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Błąd podczas generowania podglądu UI: {e.Message}");
                return null;
            }
        }
        private void DrawPorts()
        {
            var iterator = serializedObject.GetIterator();
            var enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (excludes.Contains(iterator.name)) continue;
                NodeEditorGUILayout.PropertyField(iterator);
            }

            foreach (var dynamicPort in target.DynamicPorts) {
                if (NodeEditorGUILayout.IsDynamicPortListPort(dynamicPort)) continue;
                NodeEditorGUILayout.PortField(dynamicPort);
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
                    node.OnPageChanged?.Invoke(newValue);
                
                templatePreviousValue = newValue;
                
                serializedObject.Update();
                NodeEditorWindow.RepaintAll();
            }
    
            EditorGUILayout.EndVertical();
        }
        private void DrawPageTemplateContent()
        {
            if (node.PageVariant == null) return;
            if(!node.PageVariant.TryGetComponent(out Page page))
                   return;

            foreach (var content in page.PageContents)
            {
                content.DrawEditor();
               content.OnContentChanged += () => isContentChanged = true;
            }
            
            EditorGUILayout.Space(5);
            CreatePagePreview();
        }
        public override int GetWidth()
        {
            return 400;
        }
    }
}