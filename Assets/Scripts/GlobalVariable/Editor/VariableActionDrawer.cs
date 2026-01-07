using GlobalVariable.Actions;
using UnityEditor;
using UnityEngine.UIElements;

namespace GlobalVariable.Editor
{
    [CustomPropertyDrawer(typeof(VariableAction))]
    public class VariableActionDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VariableOperationPropertyDrawer();
            return drawer.CreatePropertyGUI(property.FindPropertyRelative(VariableAction.SerializationHelper.VariableActionName));
        }
    }
}