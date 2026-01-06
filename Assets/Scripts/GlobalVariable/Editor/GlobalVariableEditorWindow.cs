using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace GlobalVariable.Editor
{

    [CustomEditor(typeof(GlobalVariables))]
    public class GlobalVariablesEditor : UnityEditor.Editor
    {
        private SerializedProperty variablesProperty;
        private Vector2 scrollPosition;
        private string searchQuery = string.Empty;
        private readonly Dictionary<int, bool> foldoutStates = new();
        private readonly HashSet<int> duplicateIndices = new();
        private readonly HashSet<int> emptyNameIndices = new();

        private GUIStyle headerStyle;
        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle addButtonStyle;
        private bool stylesInitialized;

        private static readonly Dictionary<Type, string> TypeLabels = new()
        {
            { typeof(IntVariable), "🔢" },
            { typeof(FloatVariable), "📊" },
            { typeof(BoolVariable), "☑️" },
            { typeof(StringVariable), "📝" }
        };

        private static readonly Dictionary<Type, Color> TypeColors = new()
        {
            { typeof(IntVariable), new Color(0.7f, 0.85f, 1f, 0.3f) },
            { typeof(FloatVariable), new Color(1f, 0.85f, 0.7f, 0.3f) },
            { typeof(BoolVariable), new Color(0.7f, 1f, 0.85f, 0.3f) },
            { typeof(StringVariable), new Color(1f, 1f, 0.7f, 0.3f) }
        };

        private static readonly Color DuplicateColor = new(1f, 0.3f, 0.3f, 0.3f);
        private static readonly Color DeleteButtonColor = new(1f, 0.6f, 0.6f);

        private const int IconWidth = 30;
        private const int IconSpacing = 10;
        private const int TypeLabelWidth = 60;
        private const int ButtonWidth = 30;
        private const int ButtonHeight = 20;
        private const int LabelWidth = 100;
        private const int ClearButtonWidth = 60;
        private const int ClearAllButtonWidth = 80;
        private const int AddButtonHeight = 30;
        private const int MinScrollHeight = 200;
        private const int MaxScrollHeight = 400;

        private void OnEnable()
        {
            variablesProperty = serializedObject.FindAutoProperty(nameof(GlobalVariables.Variables));
            if (variablesProperty == null)
                Debug.LogError("Cannot find 'Variables' property.");
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.8f, 0.9f, 1f) }
            };

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(5, 5, 5, 5)
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold
            };

            addButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(15, 15, 8, 8)
            };

            stylesInitialized = true;
        }

        public override void OnInspectorGUI()
        {
            InitializeStyles();
            serializedObject.Update();

            ValidateVariables();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("📚 Global Variables Editor", headerStyle);
            EditorGUILayout.Space(10);

            DrawAddNewSection();
            DrawSearchSection();
            DrawVariablesList();
            DrawBottomStats();

            serializedObject.ApplyModifiedProperties();
        }

        private void ValidateVariables()
        {
            duplicateIndices.Clear();
            emptyNameIndices.Clear();

            var nameToIndices = new Dictionary<string, List<int>>(variablesProperty.arraySize);

            for (var i = 0; i < variablesProperty.arraySize; i++)
            {
                var variable = variablesProperty.GetArrayElementAtIndex(i);
                var nameProperty = variable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variableName));

                if (nameProperty == null) continue;

                var name = nameProperty.stringValue;
                if (string.IsNullOrWhiteSpace(name))
                {
                    emptyNameIndices.Add(i);
                    continue;
                }

                if (!nameToIndices.TryGetValue(name, out var indices))
                {
                    indices = new List<int>(1);
                    nameToIndices[name] = indices;
                }

                indices.Add(i);
            }

            foreach (var index in nameToIndices.Where(kvp => kvp.Value.Count > 1).SelectMany(kvp => kvp.Value))
                duplicateIndices.Add(index);
        }

        private void DrawAddNewSection()
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField("➕ Add New Variable", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            var originalColor = GUI.backgroundColor;

            ShowTypeButton<IntVariable>();
            ShowTypeButton<FloatVariable>();
            ShowTypeButton<BoolVariable>();
            ShowTypeButton<StringVariable>();

            GUI.backgroundColor = originalColor;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(5);

            void ShowTypeButton<T>() where T : IVariable, new()
            {
                GUI.backgroundColor = TypeColors[typeof(T)];
                if (GUILayout.Button($"{TypeLabels[typeof(T)]} {GetTypeName(typeof(T))}", addButtonStyle, GUILayout.Height(AddButtonHeight)))
                    AddVariable<T>("NewVariable");
            }
        }

        private void DrawSearchSection()
        {
            EditorGUILayout.BeginHorizontal(boxStyle);
            EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
            searchQuery = EditorGUILayout.TextField(searchQuery, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Clear", GUILayout.Width(ClearButtonWidth)))
            {
                searchQuery = string.Empty;
                GUI.FocusControl(null);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        private void DrawVariablesList()
        {
            EditorGUILayout.BeginVertical(boxStyle);

            var filteredCount = GetFilteredVariablesCount();
            EditorGUILayout.LabelField($"📋 Variables ({filteredCount})", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.MinHeight(MinScrollHeight),
                GUILayout.MaxHeight(MaxScrollHeight)
            );

            if (variablesProperty.arraySize == 0)
                EditorGUILayout.HelpBox("No variables yet. Add your first variable above!", MessageType.Info);
            
            else if (filteredCount == 0)
                EditorGUILayout.HelpBox("No variables match your search.", MessageType.Info);
            
            else
                DrawFilteredVariables();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawFilteredVariables()
        {
            var hasSearch = !string.IsNullOrEmpty(searchQuery);
            var searchLower = hasSearch ? searchQuery.ToLower() : null;

            for (var i = 0; i < variablesProperty.arraySize; i++)
            {
                var variable = variablesProperty.GetArrayElementAtIndex(i);
                if (variable == null) continue;

                var nameProperty = variable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variableName));
                var typeProperty = variable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variable));

                if (nameProperty == null || typeProperty == null) continue;
                if (typeProperty.managedReferenceValue == null) continue;

                if (hasSearch && !nameProperty.stringValue.ToLower().Contains(searchLower))
                    continue;

                DrawVariable(nameProperty, typeProperty, i);
            }
        }

        private void DrawVariable(SerializedProperty nameProperty, SerializedProperty typeProperty, int index)
        {
            var varType = typeProperty.managedReferenceValue.GetType();

            foldoutStates.TryAdd(index, false);

            var bgColor = TypeColors.TryGetValue(varType, out var color)
                ? color
                : new Color(1f, 1f, 1f, 0.1f);

            if (duplicateIndices.Contains(index) || emptyNameIndices.Contains(index))
                bgColor = DuplicateColor;

            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUI.backgroundColor = originalColor;

            DrawVariableHeader(nameProperty, varType, index);

            if (foldoutStates[index])
                DrawVariableContent(nameProperty, typeProperty, index);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawVariableHeader(SerializedProperty nameProperty, Type varType, int index)
        {
            EditorGUILayout.BeginHorizontal();

            var emoji = TypeLabels.GetValueOrDefault(varType, "📦");
            EditorGUILayout.LabelField(emoji, GUILayout.Width(IconWidth));

            GUILayout.Space(IconSpacing);

            var displayName = string.IsNullOrWhiteSpace(nameProperty.stringValue)
                ? "<empty>"
                : nameProperty.stringValue;

            foldoutStates[index] = EditorGUILayout.Foldout(
                foldoutStates[index],
                displayName,
                true,
                EditorStyles.boldLabel
            );

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField($"({GetTypeName(varType)})", EditorStyles.miniLabel,
                GUILayout.Width(TypeLabelWidth));

            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = DeleteButtonColor;
            if (GUILayout.Button("❌", GUILayout.Width(ButtonWidth), GUILayout.Height(ButtonHeight)))
            {
                if (EditorUtility.DisplayDialog("Remove Variable",
                        $"Remove '{nameProperty.stringValue}'?", "Yes", "Cancel"))
                {
                    variablesProperty.DeleteArrayElementAtIndex(index);
                    foldoutStates.Remove(index);
                    serializedObject.ApplyModifiedProperties();
                    GUIUtility.ExitGUI();
                }
            }

            GUI.backgroundColor = originalColor;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawVariableContent(SerializedProperty nameProperty, SerializedProperty typeProperty, int index)
        {
            EditorGUILayout.Space(5);
            EditorGUI.indentLevel++;

            if (duplicateIndices.Contains(index))
                EditorGUILayout.HelpBox($"Duplicate name: '{nameProperty.stringValue}'", MessageType.Warning);
            
            else if (emptyNameIndices.Contains(index))
                EditorGUILayout.HelpBox("Name cannot be empty!", MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name:", GUILayout.Width(LabelWidth));

            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.DelayedTextField(nameProperty.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Change Variable Name");
                nameProperty.stringValue = newName;
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(3);

            DrawVariableValueField(typeProperty);

            EditorGUI.indentLevel--;
        }

        private void DrawVariableValueField(SerializedProperty typeProperty)
        {
            var defaultProperty = typeProperty.FindAutoPropertyRelative(nameof(Variable<int>.Value));

            if (defaultProperty == null)
            {
                EditorGUILayout.HelpBox("Cannot find Value property", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Default value:", GUILayout.Width(LabelWidth));

            EditorGUI.BeginChangeCheck();

            switch (defaultProperty.propertyType)
            {
                case SerializedPropertyType.Integer:
                    defaultProperty.intValue = EditorGUILayout.IntField(defaultProperty.intValue);
                    break;
                case SerializedPropertyType.Float:
                    defaultProperty.floatValue = EditorGUILayout.FloatField(defaultProperty.floatValue);
                    break;
                case SerializedPropertyType.Boolean:
                    defaultProperty.boolValue = EditorGUILayout.Toggle(defaultProperty.boolValue);
                    break;
                case SerializedPropertyType.String:
                    defaultProperty.stringValue =
                        EditorGUILayout.TextField(defaultProperty.stringValue ?? string.Empty);
                    break;
                default:
                    EditorGUILayout.LabelField($"Unsupported type: {defaultProperty.propertyType}");
                    break;
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBottomStats()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal(boxStyle);

            var issueCount = duplicateIndices.Count + emptyNameIndices.Count;
            var statsText = issueCount > 0
                ? $"⚠️ Total Variables: {variablesProperty.arraySize} ({issueCount} with issues)"
                : $"Total Variables: {variablesProperty.arraySize}";

            EditorGUILayout.LabelField(statsText, EditorStyles.miniLabel);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear All", buttonStyle, GUILayout.Width(ClearAllButtonWidth)))
            {
                if (EditorUtility.DisplayDialog("Clear All Variables",
                        "Are you sure you want to remove all variables?", "Yes", "Cancel"))
                {
                    variablesProperty.ClearArray();
                    foldoutStates.Clear();
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void AddVariable<T>(string baseName) where T : IVariable, new()
        {
            if (variablesProperty == null) return;

            Undo.RecordObject(target, "Add Variable");

            var uniqueName = GetUniqueName(baseName);

            var newIndex = variablesProperty.arraySize;
            variablesProperty.InsertArrayElementAtIndex(newIndex);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            var newVariable = variablesProperty.GetArrayElementAtIndex(newIndex);
            var nameProperty = newVariable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variableName));
            var typeProperty = newVariable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variable));

            if (nameProperty != null)
                nameProperty.stringValue = uniqueName;

            if (typeProperty != null)
            {
                typeProperty.managedReferenceValue = new T();

                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                var defaultProperty =
                    typeProperty.FindAutoPropertyRelative(nameof(Variable<int>.Value));

                if (defaultProperty != null)
                    SetDefaultValue(defaultProperty);
            }

            serializedObject.ApplyModifiedProperties();

            foldoutStates[newIndex] = true;

            EditorUtility.SetDirty(target);
        }

        private static void SetDefaultValue(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue = 0;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = 0f;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = false;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = string.Empty;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string GetUniqueName(string baseName)
        {
            if (variablesProperty == null) return baseName;

            var existingNames = new HashSet<string>(variablesProperty.arraySize);

            for (var i = 0; i < variablesProperty.arraySize; i++)
            {
                var variable = variablesProperty.GetArrayElementAtIndex(i);
                var nameProperty = variable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variableName));
                if (nameProperty != null)
                    existingNames.Add(nameProperty.stringValue);
            }

            if (!existingNames.Contains(baseName))
                return baseName;

            var counter = 1;
            string name;
            do
            {
                name = $"{baseName}{counter}";
                counter++;
            } while (existingNames.Contains(name));

            return name;
        }

        private int GetFilteredVariablesCount()
        {
            if (string.IsNullOrEmpty(searchQuery))
                return variablesProperty.arraySize;

            var count = 0;
            var searchLower = searchQuery.ToLower();

            for (var i = 0; i < variablesProperty.arraySize; i++)
            {
                var variable = variablesProperty.GetArrayElementAtIndex(i);
                var nameProperty = variable.FindPropertyRelative(nameof(GlobalVariables.VariableWrapper.variableName));
                if (nameProperty != null && nameProperty.stringValue.ToLower().Contains(searchLower))
                    count++;
            }

            return count;
        }

        private static string GetTypeName(Type type)
        {
            if (type == typeof(IntVariable)) return "Int";
            if (type == typeof(FloatVariable)) return "Float";
            if (type == typeof(BoolVariable)) return "Bool";
            if (type == typeof(StringVariable)) return "String";
            return type.Name;
        }
    }
}