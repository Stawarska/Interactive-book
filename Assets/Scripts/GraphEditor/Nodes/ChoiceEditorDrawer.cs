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
        private const float padding = 4f;
        private const float buttonSize = 20f;
        private const float iconSize = 16f;
        private const float iconSpacing = 3f;
        private const float minHeight = 30f;
        private const float verticalPadding = 8f;
        private const float horizontalMargin = 40f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var choiceTextProperty = property.FindPropertyRelative("choice");
            if (choiceTextProperty == null)
            {
                EditorGUI.HelpBox(position, "Property 'choice' not found", MessageType.Error);
                return;
            }

            var choice = choiceTextProperty.objectReferenceValue as Choice;

            if (choice == null)
                return;

            var showConditionsIcon = ShowConditionsIcon(choice);
            var showActionsIcon = ShowActionsIcon(choice);

            var rightSideWidth = CalculateRightSideWidth(showConditionsIcon, showActionsIcon);

            var textRect = new Rect(
                position.x,
                position.y,
                position.width - rightSideWidth,
                position.height
            );

            var previousText = choice.ChoiceText.text;
            Undo.RecordObject(choice, "Edit Choice Text");

            var textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            choice.ChoiceText.text = EditorGUI.TextArea(textRect, choice.ChoiceText.text, textAreaStyle);

            var buttonVerticalOffset = (position.height - buttonSize) * 0.5f;
            var currentX = position.x + position.width - buttonSize;
            var buttonY = position.y + buttonVerticalOffset;

            var buttonRectRemove = new Rect(currentX, buttonY, buttonSize, buttonSize);
            if (GUI.Button(buttonRectRemove, new GUIContent("❌", "Remove choice")))
            {
                Object.DestroyImmediate(choice.gameObject, true);
                var node = (ParagraphNode)property.serializedObject.targetObject;
                ChoiceEditor.OnContentChanged?.Invoke(node);
                ChoiceEditor.OnChoiceRemoved?.Invoke(node);
            }

            currentX -= buttonSize;

            var buttonRectEdit = new Rect(currentX, buttonY, buttonSize, buttonSize);
            if (GUI.Button(buttonRectEdit, new GUIContent("✍️", "Open choice editor")))
            {
                ChoiceEditorWindow.ShowWindow(choice);
            }

            currentX -= buttonSize + padding/2;

            var iconVerticalOffset = (position.height - iconSize) * 0.5f;
            var iconY = position.y + iconVerticalOffset;

            if (showActionsIcon)
            {
                var actionsRect = new Rect(currentX, iconY, iconSize - 2, iconSize);
                GUI.Label(actionsRect, new GUIContent("⚡", "Has actions"), EditorStyles.label);
                currentX -= iconSize + iconSpacing;
            }

            if (showConditionsIcon)
            {
                var conditionsRect = new Rect(currentX, iconY, iconSize, iconSize);
                GUI.Label(conditionsRect, new GUIContent("🔒", "Has conditions"), EditorStyles.label);
            }

            if (choice.ChoiceText.text == previousText)
                return;

            ChoiceEditor.OnContentChanged?.Invoke((ParagraphNode)property.serializedObject.targetObject);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var choiceTextProperty = property.FindPropertyRelative("choice");
            if (choiceTextProperty == null || choiceTextProperty.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            var choice = choiceTextProperty.objectReferenceValue as Choice;
            if (choice == null || string.IsNullOrEmpty(choice.ChoiceText.text))
                return minHeight;

            var rightSideWidth = CalculateRightSideWidth(ShowConditionsIcon(choice), ShowActionsIcon(choice));

            var width = EditorGUIUtility.currentViewWidth - rightSideWidth - horizontalMargin;
            var textHeight = CalculateTextHeight(choice.ChoiceText.text, width);

            return Mathf.Max(minHeight, textHeight + verticalPadding);
        }

        private static float CalculateRightSideWidth(bool showConditionsIcon, bool showActionsIcon)
        {
            var width = buttonSize * 2 + padding;

            if (showConditionsIcon) width += iconSize + iconSpacing;
            if (showActionsIcon) width += iconSize + iconSpacing;

            return width;
        }

        private static float CalculateTextHeight(string text, float width)
        {
            var textAreaStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };

            return textAreaStyle.CalcHeight(new GUIContent(text), width);
        }

        private static bool ShowConditionsIcon(Choice choice)
        {
            return choice.Conditions.Count > 0;
        }

        private static bool ShowActionsIcon(Choice choice)
        {
            return choice.Actions.Count > 0;
        }
    }
}