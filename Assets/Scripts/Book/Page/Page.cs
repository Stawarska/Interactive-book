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

        public Page OriginalPrefab { get; private set; }

        public void OnValidate() => this.ValidateRefs();

        public Page InstantiatePage()
        {
            var copy = Instantiate(this);
            foreach (var choice in copy.Choices)
            {
                if (choice.ConnectedPage == copy)
                    choice.ConnectedPage = this;
            }
            copy.OriginalPrefab = this;
            return copy;
        }
        

#if UNITY_EDITOR
        public void CopyContentFrom(IPageContent[] pageContents)
        {
            //TODO: dokończyć logikę kopiowania noda
            for (var i = 0; i < pageContents.Length; i++)
            {
                if(pageContents[i].GetType() != PageContents[i].GetType())
                    return;
                PageContents[i] = pageContents[i];
                PageContents[i].DrawEditor();
            }
        }
#endif
        public void ParentAndFill(RectTransform prefabParent)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.SetParent(prefabParent);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}