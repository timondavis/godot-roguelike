using Godot;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

class RoomTreeNode
{
    public RoomTreeNode Parent = null;
    public RoomTreeNode Left = null;
    public RoomTreeNode Right = null;
    public Room Room = null;

    public RoomTreeNode(Room room)
    {
        Room = room;
    }

    public ShapedRoom<TRoomShape> GetShapedRoom<TRoomShape>() where TRoomShape : Shape
    {
        return Room as ShapedRoom<TRoomShape>;
    }
}