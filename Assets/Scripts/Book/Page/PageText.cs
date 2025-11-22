using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Book
{
    [Serializable]
    public partial class PageText : IPageContent
    {
        [field: SerializeField] public string FieldName { get; private set; }
        [field: SerializeField] public TMP_Text TextFieldRef { get; private set; }
    }
    
    [Serializable]
    public partial class PageImage : IPageContent
    {
        [field: SerializeField] public string FieldName { get; private set; }
        [field: SerializeField] public Image ImageRef { get; private set; }
    }
}