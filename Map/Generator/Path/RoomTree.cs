using System.Collections.Generic;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

class RoomTree<TRoomShape> where TRoomShape : Shape, new()
{
    public RoomTreeNode<TRoomShape> Head = null;

    public void LoadChildrenToGraph(RoomGraph<TRoomShape> graph)
    {
        LoadChildrenToGraph(graph, Head);
    }
    
    private void LoadChildrenToGraph(RoomGraph<TRoomShape> graph, RoomTreeNode<TRoomShape> node)
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

    public List<RoomTreeNode<TRoomShape>> GetChildren()
    {
        List<RoomTreeNode<TRoomShape>> childList = new List<RoomTreeNode<TRoomShape>>();
        SeekChildren(childList, Head);
        return childList;
    }

    private void SeekChildren(List<RoomTreeNode<TRoomShape>> childList, RoomTreeNode<TRoomShape> node)
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