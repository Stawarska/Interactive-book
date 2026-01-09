using System;
using System.Linq;
using Book;
using UnityEditor;
using UnityEngine;
using XNode;

namespace Nodes
{
    [Serializable, NodeTint("#4CAF50")]
    public class StartNode : Node
    {
        [Output(connectionType = ConnectionType.Override)]
        public ParagraphNode StartPage;

        public Page GetStartPage() => startPage;
        [SerializeField, HideInInspector] private Page startPage;
        
        public override object GetValue(NodePort port) {
            return null;
        }

#if UNITY_EDITOR
        protected override void Init()
        {
            base.Init();
            ValidateSingleStartNode();
        }
        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            UpdateStartNode();
        }
        
        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);
            UpdateStartNode();
        }

        private void UpdateStartNode()
        {
            var portName = $"{nameof(StartPage)}";
            var outputPort = GetOutputPort(portName);

            if (outputPort == null)
            {
                Debug.LogWarning($"Port {portName} not exists!");
                return;
            }

            if (!outputPort.IsConnected)
            {
                Debug.LogWarning($"Port {portName} is not connected!");
                startPage = null;
                return;
            }
            
            var otherPort = outputPort.Connection;
            var nextNode = otherPort.node as ParagraphNode;
            startPage = nextNode?.PageVariant;
            EditorUtility.SetDirty(startPage);
        }
        
        private void ValidateSingleStartNode()
        {
            foreach (var node in graph.nodes.Where(node => node is StartNode && node != this))
                Debug.LogError("Only one StartNode is allowed! Remove duplicate StartNode.");
        }
#endif
    }
}