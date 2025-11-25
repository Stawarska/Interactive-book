using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Utils
{
    public static class GeneratePreview
    {
        public enum PageSection
        {
            Full,
            Left,
            Right
        }
        
        public static Texture2D GeneratePagePreview(GameObject uiPrefab, PageSection section)
        {
            var tempInstance = (GameObject)PrefabUtility.InstantiatePrefab(uiPrefab);

            if (tempInstance == null)
                return null;

            try
            {
                if (!tempInstance.TryGetComponent<RectTransform>(out var rectTransform))
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
                
                float renderWidth = section == PageSection.Full ? width : width / 2f;
                float cameraOffsetX = section switch
                {
                    PageSection.Left => -width / 4f,
                    PageSection.Right => width / 4f,
                    _ => 0f
                };

                var camObj = new GameObject("TempCamera");
                var camera = camObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                camera.orthographic = true;
                camera.orthographicSize = height / 2f;
                camera.nearClipPlane = 0.1f;
                camera.farClipPlane = 1000f;

                camObj.transform.position = new Vector3(cameraOffsetX, 0, -10);

                var renderTexture = new RenderTexture((int)renderWidth, (int)height, 24);
                camera.targetTexture = renderTexture;

                camera.Render();

                RenderTexture.active = renderTexture;
                var preview = new Texture2D((int)renderWidth, (int)height, TextureFormat.RGBA32, false);
                preview.ReadPixels(new Rect(0, 0, (int)renderWidth, (int)height), 0, 0);
                preview.Apply();

                RenderTexture.active = null;
                camera.targetTexture = null;
                Object.DestroyImmediate(renderTexture);
                Object.DestroyImmediate(camObj);
                Object.DestroyImmediate(canvasObj);

                return preview;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating UI preview ({section}): {e.Message}");
                return null;
            }
        }
        
        public static Sprite GeneratePageSprite(GameObject uiPrefab, PageSection section)
        {
            var texture = GeneratePagePreview(uiPrefab, section);
            return texture != null ? Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)) : null;
        }
    }
}