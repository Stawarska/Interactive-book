using System;
using System.Collections.Generic;
using System.Linq;
using GlobalVariable.Actions;
using GlobalVariable.Conditions;
using KBCore.Refs;
using SaintsField;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Book
{
    [Serializable]
    public class Choice : MonoBehaviour, IChoice
    {
        [field: SerializeField] public TMP_Text ChoiceText { get; private set; }
        [field: SerializeField, ReadOnly] public Page ConnectedPage { get; set; }
        [field: SerializeField] public List<VariableCondition> Conditions { get; private set; }
        [field: SerializeField] public List<VariableAction> Actions { get; private set; }
        [SerializeField, Child] private Button button;
        [SerializeField] private Color hoverColor;
        public event Action OnChoicePicked;
        private Color originalColor;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                if (ConnectedPage == null)
                {
                    Debug.LogError($" Choice {name} don't have a connected page");
                    return;
                }
                OnChoicePicked?.Invoke();
                BookManager.Instance.SelectNextPage(ConnectedPage);
            });

            OnChoicePicked += PerformActions;
            
            if(!MeetConditions())
                DisableChoice();
            
            originalColor = ChoiceText.color;
        }

        public bool MeetConditions()
        {
            return Conditions.All(condition => condition.Check());
        }

        private void PerformActions()
        {
            foreach (var action in Actions)
                action.Perform();
        }

        private void DisableChoice()
        {
            button.interactable = false;
            ChoiceText.color = Color.gray;
        }
        private void OnValidate() => this.ValidateRefs();
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(!button.interactable)
                return;
            ChoiceText.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ChoiceText.color = originalColor;
        }
    }
}