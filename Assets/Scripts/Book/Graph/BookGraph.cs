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
            foreach (var node in nodes)
            {
                if(node is not StartNode startNode)
                    continue;
                
                return startNode.GetStartPage();
            }
            Debug.LogWarning("No start node found");
            return null;
        }
    }
}