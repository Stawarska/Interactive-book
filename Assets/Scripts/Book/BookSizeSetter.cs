#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Utils;

namespace Book
{
    [ExecuteInEditMode]
    public class BookSizeSetter : MonoBehaviour
    {
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            var rect = rectTransform.rect;
            if(rectTransform.rect.width != EditorPrefs.GetFloat(Consts.BookSizeX))
                EditorPrefs.SetFloat(Consts.BookSizeX, rect.width);
            if(rectTransform.rect.height != EditorPrefs.GetFloat(Consts.BookSizeX))
                EditorPrefs.SetFloat(Consts.BookSizeY, rect.height);
        }
    }
}

#endif