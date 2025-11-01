using System;
using UnityEditor;
using UnityEngine;

namespace Book
{
    public partial class PageImage
    {
        private Sprite previousSprite;
        public void DrawEditor()
        {
            EditorGUILayout.LabelField(FieldName);
            Undo.RecordObject(ImageRef, "Image Change");
            ImageRef.sprite = EditorGUILayout.ObjectField(ImageRef.sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
            
            if(previousSprite == ImageRef.sprite)
                return;
            
            OnContentChanged?.Invoke();
            previousSprite = ImageRef.sprite;
        }

        public event Action OnContentChanged;
    }

    public partial class PageText
    {
        private string previousText;
        public void DrawEditor()
        {
            EditorGUILayout.LabelField(FieldName);
            EditorStyles.textField.wordWrap = true;
            Undo.RecordObject(TextFieldRef, "Text Change");
            TextFieldRef.text = EditorGUILayout.TextArea(TextFieldRef.text);

            if (TextFieldRef.text == previousText)
                return;
            
            OnContentChanged?.Invoke();
            previousText = TextFieldRef.text;
        }

        public event Action OnContentChanged;
    }
}