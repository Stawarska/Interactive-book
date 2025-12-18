using System;
using System.Collections.Generic;
using System.Linq;
using GlobalVariable.Actions;
using GlobalVariable.Conditions;
using KBCore.Refs;
using SaintsField;
using TMPro;
using UnityEngine;
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
        public event Action OnChoicePicked;

        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                if (ConnectedPage == null)
                {
                    Debug.LogError($" Choice {name} don't have a connected page");
                    return;
                }
                BookManager.Instance.SelectNextPage(ConnectedPage);
                OnChoicePicked?.Invoke();
            });

            OnChoicePicked += PerformActions;
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
        private void OnValidate() => this.ValidateRefs();
    }
}