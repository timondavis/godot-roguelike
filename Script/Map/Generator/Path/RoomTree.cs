using System.Collections.Generic;
using Roguelike.Script.Map.Model.Shapes;

namespace Roguelike.Script.Map.Generator.Path;

class RoomTree 
{
    public RoomTreeNode Head = null;

    public void LoadChildrenToGraph(RoomGraph graph)
    {
        LoadChildrenToGraph(graph, Head);
    }
    
    private void LoadChildrenToGraph(RoomGraph graph, RoomTreeNode node)
    {
        if (node.Left == null && node.Right == null)
        {
            graph.AddRoom(node.Room);
            return;
        }
        
        if (node.Left != null)
        {
            LoadChildrenToGraph(graph, node.Left);
        }

        if (node.Right != null)
        {
            LoadChildrenToGraph(graph, node.Right);
        } 
    }

    public List<RoomTreeNode> GetChildren()
    {
        List<RoomTreeNode> childList = new List<RoomTreeNode>();
        SeekChildren(childList, Head);
        return childList;
    }

    private void SeekChildren(List<RoomTreeNode> childList, RoomTreeNode node)
    {
        if (node.Left == null && node.Right == null)
        {
            childList.Add(node);
            return;
        }
        
        if (node.Left != null)
        {
            SeekChildren(childList, node.Left);
        }

        if (node.Right != null)
        {
            SeekChildren(childList, node.Right);
        }
    }

}