using Godot;
using Roguelike.Script.Map.Model.Shapes;

namespace Roguelike.Script.Map.Model;

public class ShapedRoom<TRoomShape> : Room where TRoomShape : Shape
{
   public TRoomShape Shape;

   public override Vector2I Location
   {
       get
       {
           return Shape.Center;
       }
   }

   public override Vector2I Size
   {
       get
       {
           return Shape.Size;
       }
   }

   public ShapedRoom(TRoomShape shape)
   {
      Shape = shape;
   }
}