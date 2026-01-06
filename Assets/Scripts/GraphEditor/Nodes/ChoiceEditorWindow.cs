using Book;
using GlobalVariable.Actions;
using GlobalVariable.Conditions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;


namespace GraphEditor.Nodes
{
    public class ChoiceEditorWindow : EditorWindow
    {
        private static Choice choice;

        private SerializedObject serializedChoice;
        private SerializedProperty conditionsProperty;
        private SerializedProperty actionsProperty;

        private ListView conditionsListView;
        private ListView actionsListView;

        public static void ShowWindow(Choice choiceEditor)
        {
            var window = GetWindow<ChoiceEditorWindow>("Choice Editor");
            choice = choiceEditor;
            window.Initialize();
        }

        private void OnEnable()
        {
            if (choice != null)
                Initialize();
        }

        private void Initialize()
        {
            if (choice == null)
                return;

            serializedChoice = new SerializedObject(choice);
            
            conditionsProperty = serializedChoice.FindAutoProperty(nameof(choice.Conditions));
            actionsProperty = serializedChoice.FindAutoProperty(nameof(choice.Actions));

            rootVisualElement.Clear();
            CreateGUI();
        }

        public void CreateGUI()
        {
            if (choice == null)
            {
                rootVisualElement.Add(new HelpBox("No Choice selected.", HelpBoxMessageType.Warning));
                return;
            }

            var scroll = new ScrollView();
            rootVisualElement.Add(scroll);
            
            var header = new Label($"Choice Editor - {choice.ChoiceText.text}");
            header.style.height = 40;
            header.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.fontSize = 16;
            header.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);
            header.style.color = Color.white;
            header.style.marginBottom = 10;
            scroll.Add(header);
            
            var conditionsSection = CreateListSection("Conditions", 
                new Color(0.2f, 0.6f, 1f),
                conditionsProperty,
                AddCondition,
                RemoveCondition);

            conditionsListView = conditionsSection.listView;
            scroll.Add(conditionsSection.root);
            
            var actionsSection = CreateListSection(
                "Actions",
                new Color(0.8f, 0.4f, 0.2f),
                actionsProperty,
                AddAction,
                RemoveAction);

            actionsListView = actionsSection.listView;
            scroll.Add(actionsSection.root);
        }
        
        private (VisualElement root, ListView listView) CreateListSection(
            string sectionTitle,
            Color headerColor,
            SerializedProperty listProperty,
            System.Action onAdd,
            System.Action<int> onRemove)
        {
            var root = new VisualElement();
            root.style.marginBottom = 20;
            
            var header = new Label(sectionTitle);
            header.style.height = 30;
            header.style.paddingLeft = 8;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.fontSize = 14;
            header.style.backgroundColor = headerColor;
            header.style.color = Color.white;
            root.Add(header);
            
            var listView = new ListView
            {
                reorderable = true,
                selectionType = SelectionType.Single,
                showBorder = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
            };

            listView.makeItem = () => new VisualElement();

            listView.bindItem = (element, index) =>
            {
                element.Clear();

                if (index >= listProperty.arraySize)
                    return;

                var elementProp = listProperty.GetArrayElementAtIndex(index);
               
                var inspector = new PropertyField(elementProp);
                inspector.BindProperty(elementProp);

                inspector.style.marginBottom = 6;
                element.Add(inspector);
            };

            root.Add(listView);
            RefreshList(listView, listProperty);
            
            var buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.marginTop = 5;

            var addButton = new Button(() =>
            {
                onAdd();
                RefreshList(listView, listProperty);
            })
            {
                text = "Add"
            };

            var removeButton = new Button(() =>
            {
                if (listView.selectedIndex < 0)
                    return;

                if (!EditorUtility.DisplayDialog($"Remove {sectionTitle}", $"Are you sure?", "Yes", "Cancel")) 
                    return;
                
                onRemove(listView.selectedIndex);
                RefreshList(listView, listProperty);
            })
            {
                text = "Remove"
            };

            buttons.Add(addButton);
            buttons.Add(removeButton);
            root.Add(buttons);

            return (root, listView);
        }
        
        private void RefreshList(ListView listView, SerializedProperty property)
        {
            serializedChoice.Update();

            listView.itemsSource = new int[property.arraySize];
            listView.Rebuild();

            serializedChoice.ApplyModifiedProperties();
            EditorUtility.SetDirty(choice);
        }

        private static void AddCondition()
        {
            Undo.RecordObject(choice, "Add Condition");
            choice.Conditions.Add(new VariableCondition());
        }

        private static void RemoveCondition(int index)
        {
            if (index < 0 || index >= choice.Conditions.Count)
                return;

            Undo.RecordObject(choice, "Remove Condition");
            choice.Conditions.RemoveAt(index);
        }

        private static void AddAction()
        {
            Undo.RecordObject(choice, "Add Action");
            choice.Actions.Add(new VariableAction());
        }

        private static void RemoveAction(int index)
        {
            if (index < 0 || index >= choice.Actions.Count)
                return;

            Undo.RecordObject(choice, "Remove Action");
            choice.Actions.RemoveAt(index);
        }
    }
}