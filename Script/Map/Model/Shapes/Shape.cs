using Godot;

namespace Roguelike.Script.Map.Model.Shapes;

public abstract partial class Shape : Node
{
	public Vector2I Center;

	public abstract Vector2I Size { get; set; }

	public abstract bool Intersects(Shape leftShape, Shape rightShape);
	
	public delegate void ForEachCoordinateInArea(Vector2I point, Shape shape);
	public event ForEachCoordinateInArea EachCoordinateInArea;
	protected virtual void OnEachCoordinateInArea(Vector2I point, Shape shape)
	{
		EachCoordinateInArea?.Invoke(point, shape);
	}

	public delegate void ForEachCoordinateInPerimeter(Vector2I point, Shape shape);
	public event ForEachCoordinateInPerimeter EachCoordinateInPerimeter;

	protected virtual void OnEachCoordinateInPerimeter(Vector2I point, Shape shape)
	{
		EachCoordinateInPerimeter?.Invoke(point, shape);
	}
	
	public abstract void ScanArea(); // Invoke ForEachCoordinateInArea.
	public abstract void ScanPerimeter(); // Invoke ForEachCoordinateInPerimeter.

	public abstract bool IsPointWithinShape(Vector2I point);
} 
