using System;
using Book;
using Nodes;

namespace Graph.Editor.Nodes
{
    [Serializable]
    public class ChoiceEditor
    {
        public Choice choice;
        public static Action<ParagraphNode> OnContentChanged;
        public static Action<ParagraphNode> OnChoiceRemoved;
    }
}