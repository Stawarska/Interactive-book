using System;
using UnityEngine;

namespace Book
{
    public class Page : MonoBehaviour
    {
        [field: SerializeReference, SubclassSelector] public IPageContent[] PageContents { get; private set; } = Array.Empty<IPageContent>();
    }
}