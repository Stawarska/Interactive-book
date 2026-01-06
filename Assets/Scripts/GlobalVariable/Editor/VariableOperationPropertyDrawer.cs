using System;
using System.Linq;
using SaintsField.Editor.Utils;
using UnityEditor;
using UnityEngine.UIElements;
using Utils;
using System.Reflection;
using UnityEditor.UIElements;

namespace GlobalVariable.Editor
{
    [CustomPropertyDrawer(typeof(VariableOperation<>))]
    public class VariableOperationPropertyDrawer : PropertyDrawer
    {
        private static VisualTreeAsset dropdownButtonTree;

        private UIToolkitUtils.DropdownButtonField idField;
        private UIToolkitUtils.DropdownButtonField operationField;
        private PropertyField operationPropertyField;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new Foldout
            {
                text = property.displayName
            };

            var idProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.VariableID));
            idField = UIToolkitUtils.MakeDropdownButtonUIToolkit(idProperty.displayName);
            idField.ButtonLabelElement.text = idProperty.stringValue;
            idField.ButtonElement.clicked += () =>
                ShowIdDropdown(property);

            root.Add(idField);


            var operationProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.Operation));
            operationField = UIToolkitUtils.MakeDropdownButtonUIToolkit(operationProperty.displayName);
            operationField.ButtonLabelElement.text = operationProperty.managedReferenceValue == null
                ? "None"
                : operationProperty.managedReferenceValue.GetType().Name;
            operationField.ButtonElement.clicked += () =>
                ShowOperationDropdown(property);

            root.Add(operationField);

            CheckIfActionValid(property);
            
            operationProperty.isExpanded = true;
            operationPropertyField = new PropertyField(operationProperty);
            operationPropertyField.Bind(property.serializedObject);
            operationPropertyField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                operationPropertyField.Q<Toggle>().style.display = DisplayStyle.None;
            });
            root.Add(operationPropertyField);

            return root;
        }

        private void ShowOperationDropdown(SerializedProperty property)
        {
            var operationProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.Operation));
            var idProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.VariableID));
            var variable = GlobalVariables.instance.GetVariableFromName(idProperty.stringValue);
            if (variable == null)
                return;

            var variableBaseType = GetGenericType(variable.variable.GetType());

            var baseFieldType = GetTargetObjectOfProperty(property).GetType()
                .GetField(SerializationExtensions.GetBackingFieldName(nameof(VariableOperation<int>.Operation)),
                    BindingFlags.NonPublic | BindingFlags.Instance)
                ?.FieldType;

            if (baseFieldType == null)
                return;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => baseFieldType.IsAssignableFrom(x))
                .Where(x => GetGenericType(x) == variableBaseType);

            var genericDropdownMenu = new GenericDropdownMenu();
            foreach (var type in types)
            {
                var isChecked = operationProperty.managedReferenceValue != null &&
                                operationProperty.managedReferenceValue.GetType() == type;
                genericDropdownMenu.AddItem(type.Name, isChecked, () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Change Variable");
                    operationProperty.managedReferenceValue = Activator.CreateInstance(type);
                    operationPropertyField.bindingPath = operationProperty.propertyPath;
                    operationPropertyField.Bind(operationProperty.serializedObject);
                    property.serializedObject.ApplyModifiedProperties();
                    operationField.ButtonLabelElement.text = type.Name;
                });
            }

            genericDropdownMenu.DropDown(operationField.ButtonElement.worldBound, operationField, true);
        }

        private void ShowIdDropdown(SerializedProperty property)
        {
            var idProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.VariableID));
            var genericDropdownMenu = new GenericDropdownMenu();

            foreach (var variable in GlobalVariables.instance.GetAllVariablesNames())
            {
                genericDropdownMenu.AddItem(variable, variable == idProperty.stringValue, () =>
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "Change Variable");
                    idProperty.stringValue = variable;
                    property.serializedObject.ApplyModifiedProperties();
                    CheckIfActionValid(property);
                    idField.ButtonLabelElement.text = variable;
                });
            }

            genericDropdownMenu.DropDown(idField.ButtonElement.worldBound, idField, true);
        }

        private void CheckIfActionValid(SerializedProperty property)
        {
            var idProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.VariableID));
            var operationProperty = property.FindAutoPropertyRelative(nameof(VariableOperation<int>.Operation));

            var variable = GlobalVariables.instance.GetVariableFromName(idProperty.stringValue);

            if (variable == null)
            {
                operationProperty.managedReferenceValue = null;
                operationField.ButtonLabelElement.text = "None";
                property.serializedObject.ApplyModifiedProperties();
                return;
            }

            if (operationProperty.managedReferenceValue == null)
                return;

            var variableBaseType = GetGenericType(variable.variable.GetType());
            var operationType = GetGenericType(operationProperty.managedReferenceValue.GetType());

            if (variableBaseType == operationType)
                return;

            //TODO: check if we can convert operation
            operationProperty.managedReferenceValue = null;
            operationField.ButtonLabelElement.text = "None";
            property.serializedObject.ApplyModifiedProperties();
        }

        private static Type GetGenericType(Type baseObject)
        {
            while (!baseObject.IsGenericType)
            {
                baseObject = baseObject.BaseType;
                if (baseObject == null)
                    return null;
            }

            return baseObject.GetGenericArguments()[0];
        }

        private static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }

            return obj;
        }

        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();

            for (var i = 0; i <= index; i++)
                if (!enm.MoveNext())
                    return null;

            return enm.Current;
        }
    }
}