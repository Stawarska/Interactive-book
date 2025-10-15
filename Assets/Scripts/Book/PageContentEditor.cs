using UnityEditor;
using UnityEngine;

namespace Book
{
    public partial class PageImage
    {
        public void DrawEditor()
        {
            EditorGUILayout.LabelField(FieldName);
            ImageRef.sprite = EditorGUILayout.ObjectField(ImageRef.sprite, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        }
    }

    public partial class PageText
    {
        public void DrawEditor()
        {
            EditorGUILayout.LabelField(FieldName);
            EditorStyles.textField.wordWrap = true;
            TextFieldRef.text = EditorGUILayout.TextArea(TextFieldRef.text);
        }
    }
}