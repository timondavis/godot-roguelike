using Godot;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Service;

public class RoomService
{
    private static RoomService _instance;
    
    public static RoomService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RoomService();
            }

            return _instance;
        } 
    }
    
    private RoomService()
    {
    }

    public bool IsRoomAreaVacant<TRoomShape>(Room<TRoomShape> room, GeneratorGrid grid) where TRoomShape : Shape
    {
        Vector2I placeholder = new Vector2I(grid.Current.Position.X, grid.Current.Position.Y);
        bool isRoomAvailable = true;

        void OnEachCoordinateInArea(Vector2I point, Shape shape)
        {
            if (isRoomAvailable == false)
            {
                return;
            }
            
            grid.MoveTo(point);
            if (grid.Current.IsActive)
            {
                isRoomAvailable = false;
            }
        }

        room.Shape.EachCoordinateInArea += OnEachCoordinateInArea;
        room.Shape.ScanArea();
        grid.MoveTo(placeholder);
        room.Shape.EachCoordinateInArea -= OnEachCoordinateInArea;
        return isRoomAvailable;
    }
    
    public Room<TRoomShape> GenerateRoom<TRoomShape>() where TRoomShape : Shape, new ()
    {
        Room<TRoomShape> room = new Room<TRoomShape>(new TRoomShape());
        return room;
    }

    public bool IsRoomAreaIsolated<TRoomShape>(Room<TRoomShape> room, GeneratorGrid grid) where TRoomShape : Shape
    {
        Vector2I placeholder = new Vector2I(grid.Current.Position.X, grid.Current.Position.Y);
        bool isRoomIsolated = true;

        void OnEachCoordinateInPerimeter(Vector2I point, Shape shape)
        {
            if (isRoomIsolated == false)
            {
                return;
            }
            
            grid.MoveTo(point);
            for (int x = point.X - 1; x <= point.X + 1; x++)
            {
                for (int y = point.Y - 1; y <= point.Y + 1; y++)
                {
                    Vector2I checkPoint = new Vector2I(x, y);
                    if (!grid.IsPositionSafe(checkPoint))
                    {
                        continue;
                    }

                    if (shape.IsPointWithinShape(checkPoint))
                    {
                        continue;
                    }

                    if (grid.Current.IsActive)
                    {
                        isRoomIsolated = false;
                        return;
                    }
                }
            }
        }

        room.Shape.EachCoordinateInPerimeter += OnEachCoordinateInPerimeter;
        room.Shape.ScanPerimeter();
        grid.MoveTo(placeholder);
        room.Shape.EachCoordinateInPerimeter -= OnEachCoordinateInPerimeter;
        return isRoomIsolated;
    }

}