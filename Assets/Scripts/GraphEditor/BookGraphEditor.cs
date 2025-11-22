using Nodes;
using XNode;
using XNodeEditor;

namespace Graph.Editor
{
    [CustomNodeGraphEditor(typeof(BookGraph))]
    public class BookGraphEditor : NodeGraphEditor
    {
        private BookGraph bookGraph;
        
        public override void OnCreate()
        {
            base.OnCreate();
            bookGraph = target as BookGraph;
        }

        
        public override Node CopyNode(Node original)
        {
            //TODO: dokończyć logikę kopiowania noda
            var copiedNode = base.CopyNode(original);

            if (copiedNode is ParagraphNode paragraphNode)
            {
                paragraphNode.CreatePrefabVariant(paragraphNode.PageTemplate);
                paragraphNode.PageVariant.CopyContentFrom(((ParagraphNode)original).PageVariant.PageContents); 
            }
            
            return copiedNode;
        }
        
        public void OnValidate()
        {
            UpdateAllNodes();
        }

        private void UpdateAllNodes()
        {
            foreach (var node in bookGraph.nodes)
            {
                if (node is ParagraphNode paragraphNode)
                {
                    paragraphNode.UpdateChoicesConnections();
                }
            }
        }
    }
}