using System.Collections.Generic;
using Godot;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

public class RoomGraphNode<TRoomShape> where TRoomShape : Shape, new()
{
   public HashSet<RoomGraphNode<TRoomShape>> ConnectedNodes { get; private set; }
   public Room<TRoomShape> Room{ get; private set; }

   public Vector2I Position
   {
      get { return Room.Shape.Center; }
   }

   public RoomGraphNode(Room<TRoomShape> room)
   {
      ConnectedNodes = new HashSet<RoomGraphNode<TRoomShape>>();
      Room = room;
   }
}