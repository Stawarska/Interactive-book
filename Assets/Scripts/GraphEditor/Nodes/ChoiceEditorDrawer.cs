using Book;
using Graph.Editor.Nodes;
using Nodes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphEditor.Nodes
{
    [CustomPropertyDrawer(typeof(ChoiceEditor))]
    public class ChoiceEditorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var choiceTextProperty = property.FindPropertyRelative("choice");
            if (choiceTextProperty == null)
            {
                EditorGUI.HelpBox(position, "Property 'choice' not found", MessageType.Error);
                return;
            }
            
            var choice = choiceTextProperty.objectReferenceValue as Choice;
            
            if(choice == null)
                return;
            
            const float padding = 4f;
            const float buttonWidth = 20f;
            
            var textRect = new Rect(position.x, position.y, position.width - buttonWidth*2 - padding, position.height);
            var buttonRectRemove = new Rect(position.x + position.width - buttonWidth, position.y, buttonWidth, position.height);
            var buttonRectEdit = new Rect(position.x + position.width - buttonWidth*2, position.y, buttonWidth, position.height);

            var previousText = choice.ChoiceText.text;
            Undo.RecordObject(choice, "Edit Choice Text");

            choice.ChoiceText.text = EditorGUI.TextArea(textRect, choice.ChoiceText.text);
            EditorStyles.textField.wordWrap = true;
            

            if (GUI.Button(buttonRectRemove, new GUIContent("❌", "Remove choice")))
            {
                Object.DestroyImmediate(choice.gameObject, true);
                var node = (ParagraphNode)property.serializedObject.targetObject;
                ChoiceEditor.OnContentChanged?.Invoke(node);
                ChoiceEditor.OnChoiceRemoved?.Invoke(node);
            }
            
            if (GUI.Button(buttonRectEdit, new GUIContent("✍️", "Open choice editor")))
            {

            }
                

            if (choice.ChoiceText.text == previousText)
                return;
            
            ChoiceEditor.OnContentChanged?.Invoke((ParagraphNode)property.serializedObject.targetObject);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 30;
        }
    }
}