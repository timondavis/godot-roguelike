using System.Collections.Generic;
using Godot;
using Roguelike.Script.Map.Model;
using Roguelike.Script.Map.Model.Shapes;

namespace Roguelike.Script.Map.Generator.Path;

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