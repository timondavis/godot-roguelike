using System.Collections.Generic;
namespace Roguelike.Map.Generator.Path;

class RoomTree<T> where T : Model.Room
{
    public RoomTreeNode<T> Head = null;

    public void LoadChildrenToGraph(RoomGraph<T> graph)
    {
        LoadChildrenToGraph(graph, Head);
    }
    
    private void LoadChildrenToGraph(RoomGraph<T> graph, RoomTreeNode<T> node)
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

    public List<RoomTreeNode<T>> GetChildren()
    {
        List<RoomTreeNode<T>> childList = new List<RoomTreeNode<T>>();
        SeekChildren(childList, Head);
        return childList;
    }

    private void SeekChildren(List<RoomTreeNode<T>> childList, RoomTreeNode<T> node)
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