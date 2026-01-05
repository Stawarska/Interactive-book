using GlobalVariable.Conditions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils;

namespace GlobalVariable.Editor
{
    [CustomPropertyDrawer(typeof(VariableCondition))]
    public class VariableConditionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            
            var variableIdProp = property.FindAutoPropertyRelative("VariableID");

            var stringConditionProp = property.FindPropertyRelative("stringConditions");
            var boolConditionProp = property.FindPropertyRelative("boolConditions");
            var intConditionProp = property.FindPropertyRelative("intConditions");
            var floatConditionProp = property.FindPropertyRelative("floatConditions");
            
            var variableIdField = new PropertyField(variableIdProp, "Variable");

            var stringConditionField = new PropertyField(stringConditionProp);
            var boolConditionField = new PropertyField(boolConditionProp);
            var intConditionField = new PropertyField(intConditionProp);
            var floatConditionField = new PropertyField(floatConditionProp);
            
            root.Add(variableIdField);
            root.Add(stringConditionField);
            root.Add(boolConditionField);
            root.Add(intConditionField);
            root.Add(floatConditionField);
            
            void RefreshVisibility()
            {
                if (string.IsNullOrEmpty(variableIdProp.stringValue))
                {
                    HideAll();
                    return;
                }

                var type = GlobalVariables.instance
                    .GetTypeFromName(variableIdProp.stringValue);

                SetVisible(stringConditionField, type == typeof(StringVariable));
                SetVisible(boolConditionField, type == typeof(BoolVariable));
                SetVisible(intConditionField, type == typeof(IntVariable));
                SetVisible(floatConditionField, type == typeof(FloatVariable));
            }

            void HideAll()
            {
                stringConditionField.style.display = DisplayStyle.None;
                boolConditionField.style.display = DisplayStyle.None;
                intConditionField.style.display = DisplayStyle.None;
                floatConditionField.style.display = DisplayStyle.None;
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