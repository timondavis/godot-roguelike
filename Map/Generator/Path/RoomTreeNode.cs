using Godot;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

class RoomTreeNode<TRoomShape> where TRoomShape : Shape, new()
{
    public RoomTreeNode<TRoomShape> Parent = null;
    public RoomTreeNode<TRoomShape> Left = null;
    public RoomTreeNode<TRoomShape> Right = null;
    public Room<TRoomShape> Room = null;

    public RoomTreeNode(Room<TRoomShape> room)
    {
        Room = room;
    }
}