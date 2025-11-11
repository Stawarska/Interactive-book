using System;
using TMPro;
using UnityEngine;

namespace Book
{
    [Serializable]
    public class Choice : MonoBehaviour
    {
        [field: SerializeField] public TMP_Text ChoiceText { get; private set; }
    }
}