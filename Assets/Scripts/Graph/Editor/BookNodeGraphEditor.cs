using System;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Graph.Editor
{
    public class BookNodeGraphEditor : NodeGraphEditor
    {
        public static event Action OnCreateNode;
        public static event Action OnRemoveNode;
        
        public override Node CreateNode(Type type, Vector2 position)
        {
            OnCreateNode?.Invoke();
            return base.CreateNode(type, position);
        }

        public override void RemoveNode(Node node)
        {
            base.RemoveNode(node);
            OnRemoveNode?.Invoke();
        }
    }
}