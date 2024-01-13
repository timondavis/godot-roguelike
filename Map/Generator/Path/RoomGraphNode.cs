using System.Collections.Generic;
using System.Net.Sockets;
using Godot;

namespace Roguelike.Map.Generator.Path;

public class RoomGraphNode<T> where T : Model.Room
{
   public HashSet<RoomGraphNode<T>> ConnectedNodes { get; private set; }
   public T Room { get; private set; }

   public Vector2I Position
   {
      get { return Room.Center; }
   }

   public RoomGraphNode(T room)
   {
      ConnectedNodes = new HashSet<RoomGraphNode<T>>();
      Room = room;
   }
}