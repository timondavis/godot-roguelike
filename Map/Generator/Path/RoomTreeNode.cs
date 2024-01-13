using Godot;

namespace Roguelike.Map.Generator.Path;

class RoomTreeNode<T> where T : Model.Room
{
    public RoomTreeNode<T> Parent = null;
    public RoomTreeNode<T> Left = null;
    public RoomTreeNode<T> Right = null;
    public T Room = null;

    public RoomTreeNode(T room)
    {
        Room = room;
    }
}