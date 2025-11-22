using Book;
using Nodes;
using UnityEngine;
using XNode;

namespace Graph
{
    [CreateAssetMenu(fileName = "Book Graph", menuName = "Book/Graph/Book Graph")]
    public class BookGraph : NodeGraph
    {
        public Page GetFirstPage()
        {
            if (nodes.Count == 0)
                return null;

            return nodes[0] is not ParagraphNode paragraphNode ? null : paragraphNode.PageVariant;
        }
    }
}