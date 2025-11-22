using System;
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
        }
        private void OnValidate() => this.ValidateRefs();
    }
}