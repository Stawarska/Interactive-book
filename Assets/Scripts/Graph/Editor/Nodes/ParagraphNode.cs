using System;
using Book;
using Graph.Editor.Nodes;
using UnityEngine;
using XNode;

namespace Nodes
{
    [Serializable]
    public class ParagraphNode : Node
    {
        [Input] public Choice inputChoices;
        [field: SerializeField] public Page PageTemplate { get; private set; }

        [field: SerializeField, Output(dynamicPortList = true)] public Choice[] Choices { get; private set; }
        
        public override object GetValue(NodePort port) {
            return port.fieldName == "Choices" ? Choices : null;
        }

        protected override void Init()
        {
            base.Init();
            
            
        }
        
        
    }
}