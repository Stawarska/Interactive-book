using GlobalVariable.Conditions;
using UnityEditor;
using UnityEngine.UIElements;

namespace GlobalVariable.Editor
{
    [CustomPropertyDrawer(typeof(VariableCondition))]
    public class VariableConditionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VariableOperationPropertyDrawer();
            return drawer.CreatePropertyGUI(property.FindPropertyRelative("variableCondition"));
        }
    }
}