using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace Book
{
    public class Page : MonoBehaviour
    {
        [field: SerializeReference, SubclassSelector] public IPageContent[] PageContents { get; private set; } = Array.Empty<IPageContent>();
        [field: SerializeField, Child(Flag.Optional)] public List<Choice> Choices { get; set; }
        [field: SerializeField] public Transform ChoiceParent { get; private set; }
        [field: SerializeField] public Choice ChoiceTemplate { get; private set; }

        public void OnValidate() => this.ValidateRefs();
    }
}