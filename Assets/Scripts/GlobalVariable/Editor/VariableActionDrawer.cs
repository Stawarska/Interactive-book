using GlobalVariable.Actions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils;

namespace GlobalVariable.Editor
{
    [CustomPropertyDrawer(typeof(VariableAction))]
    public class VariableActionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            var variableIdProp = property.FindAutoPropertyRelative("VariableID");

            var stringActionProp = property.FindPropertyRelative("stringActions");
            var boolActionProp = property.FindPropertyRelative("boolActions");
            var intActionProp = property.FindPropertyRelative("intActions");
            var floatActionProp = property.FindPropertyRelative("floatActions");
            
            var variableIdField = new PropertyField(variableIdProp, "Variable");

            var stringActionField = new PropertyField(stringActionProp);
            var boolActionField = new PropertyField(boolActionProp);
            var intActionField = new PropertyField(intActionProp);
            var floatActionField = new PropertyField(floatActionProp);
            
            root.Add(variableIdField);
            root.Add(stringActionField);
            root.Add(boolActionField);
            root.Add(intActionField);
            root.Add(floatActionField);
            
            void RefreshVisibility()
            {
                if (string.IsNullOrEmpty(variableIdProp.stringValue))
                {
                    HideAll();
                    return;
                }

                var type = GlobalVariables.instance
                    .GetTypeFromName(variableIdProp.stringValue);

                SetVisible(stringActionField, type == typeof(StringGlobalVariable));
                SetVisible(boolActionField, type == typeof(BoolGlobalVariable));
                SetVisible(intActionField, type == typeof(IntGlobalVariable));
                SetVisible(floatActionField, type == typeof(FloatGlobalVariable));
            }

            void HideAll()
            {
                stringActionField.style.display = DisplayStyle.None;
                boolActionField.style.display = DisplayStyle.None;
                intActionField.style.display = DisplayStyle.None;
                floatActionField.style.display = DisplayStyle.None;
            }

            static void SetVisible(VisualElement element, bool visible)
            {
                element.style.display = visible
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            }
            
            root.TrackPropertyValue(variableIdProp, _ => RefreshVisibility());
            
            RefreshVisibility();

            return root;
        }
    }
}