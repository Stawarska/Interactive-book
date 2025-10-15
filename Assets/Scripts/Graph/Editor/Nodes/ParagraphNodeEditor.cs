using Book;
using Nodes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XNodeEditor;

namespace Graph.Editor.Nodes
{
    [CustomNodeEditor(typeof(ParagraphNode))]
    public class ParagraphNodeEditor : NodeEditor
    {
        private ParagraphNode node;
        private Texture2D cachedPreview;
        private Page lastPrefab;

        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            node = target as ParagraphNode;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            
            if (node.PageTemplate != null)
            {
                foreach (var content in node.PageTemplate.PageContents)
                    content.DrawEditor();
                
                EditorGUILayout.Space(5);

                CreatePagePreview();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void CreatePagePreview()
        {
            // Regeneruj podgląd jeśli prefab się zmienił
            if (lastPrefab != node.PageTemplate)
            {
                cachedPreview = null;
                lastPrefab = node.PageTemplate;
            }

            // Sprawdź czy to UI element
            bool isUIElement = node.PageTemplate.GetComponent<RectTransform>() != null;

            if (isUIElement && cachedPreview == null)
            {
                cachedPreview = GenerateUIPreview(node.PageTemplate);
            }

            // Użyj custom preview dla UI lub standardowego dla innych
            Texture2D preview = isUIElement ? cachedPreview : AssetPreview.GetAssetPreview(node.PageTemplate);

            if (preview != null)
            {
                float previewSize = 400f;
                Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);

                float width = Mathf.Min(previewSize, previewRect.width);
                previewRect.x += (previewRect.width - width) / 2;
                previewRect.width = width;
                previewRect.height = width;

                // Dodaj ramkę dla lepszej widoczności
                EditorGUI.DrawRect(
                    new Rect(previewRect.x - 1, previewRect.y - 1, previewRect.width + 2, previewRect.height + 2),
                    Color.gray);
                EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f));

                GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);

                EditorGUILayout.LabelField(node.PageTemplate.name, EditorStyles.centeredGreyMiniLabel);
            }
            else if (!isUIElement)
            {
                EditorGUILayout.HelpBox("Ładowanie podglądu...", MessageType.Info);
                // Repaint();
            }
            else
            {
                EditorGUILayout.HelpBox("Nie można wygenerować podglądu", MessageType.Warning);
            }
        }
        private Texture2D GenerateUIPreview(Page uiPrefab)
        {
            // Stwórz tymczasową instancję UI
            GameObject tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(uiPrefab.gameObject);

            if (tempInstance == null)
                return null;

            try
            {
                RectTransform rectTransform = tempInstance.GetComponent<RectTransform>();
                if (rectTransform == null)
                    return null;

                // Stwórz tymczasowy Canvas
                GameObject canvasObj = new GameObject("TempCanvas");
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;

                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.dynamicPixelsPerUnit = 10;

                // Dodaj UI element do canvas
                tempInstance.transform.SetParent(canvasObj.transform, false);

                // Ustaw rozmiar i pozycję
                Rect rect = rectTransform.rect;
                float scale = 512f / Mathf.Max(rect.width, rect.height);

                var width = EditorPrefs.GetFloat(Utils.Consts.BookSizeX);
                var height = EditorPrefs.GetFloat(Utils.Consts.BookSizeY);
                
                canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                // Stwórz kamerę do renderowania
                GameObject camObj = new GameObject("TempCamera");
                Camera camera = camObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                camera.orthographic = true;
                camera.orthographicSize = 600;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 1000f;

                camObj.transform.position = new Vector3(0, 0, -10);

                // Render Texture
                RenderTexture renderTexture = new RenderTexture((int)width, (int)height, 24);
                camera.targetTexture = renderTexture;

                // Renderuj
                camera.Render();

                // Skopiuj do Texture2D
                RenderTexture.active = renderTexture;
                Texture2D preview = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
                preview.ReadPixels(new Rect(0, 0, (int)width, (int)height), 0, 0);
                preview.Apply();

                // Cleanup
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

        public override int GetWidth()
        {
            return 400;
        }
    }
}