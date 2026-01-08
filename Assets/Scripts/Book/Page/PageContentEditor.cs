#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Book
{
    public partial class PageImage
    {
        public void DrawEditor()
        {
            var previousSprite = ImageRef.sprite;
            EditorGUILayout.LabelField(FieldName);
            Undo.RecordObject(ImageRef, "Image Change");
            ImageRef.sprite = EditorGUILayout.ObjectField(ImageRef.sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
            
            if(previousSprite == ImageRef.sprite)
                return;
            
            OnContentChanged?.Invoke();
        }

        public event Action OnContentChanged;
    }

    public partial class PageText
    {
        public void DrawEditor()
        {
            var previousText = TextFieldRef.text;
            EditorGUILayout.LabelField(FieldName);
            EditorStyles.textField.wordWrap = true;
            Undo.RecordObject(TextFieldRef, "Text Change");
            TextFieldRef.text = EditorGUILayout.TextArea(TextFieldRef.text);

            if (TextFieldRef.text == previousText)
                return;
            
            OnContentChanged?.Invoke();
        }

        public event Action OnContentChanged;
    }
}
#endif