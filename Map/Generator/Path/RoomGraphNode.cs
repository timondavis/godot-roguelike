using System.Collections.Generic;
using Godot;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

public class RoomGraphNode
{
   public HashSet<RoomGraphNode> ConnectedNodes { get; private set; }
   public Room Room{ get; private set; }

   public Vector2I Position
   {
      get
      {
         return Room.Location;
      }
   }

   public RoomGraphNode(Room room)
   {
      ConnectedNodes = new HashSet<RoomGraphNode>();
      Room = room;
   }
}