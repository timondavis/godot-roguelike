using System;
using System.Drawing;
using System.Transactions;
using Godot;

namespace Roguelike.Script.Map.Model.Shapes;

public partial class Rectangle : Shape
{
	[Export]
	public Vector2I TopLeft
	{
		get { return _topLeft; }
		set
		{
			_topLeft = value;
			_isTopLeftSet = true;
			SetCenter();
		}
	}

	[Export]
	public int Width
	{
		get { return Size.X; }
		set
		{
			_size.X = value;
			_isSizeSet = true;
		}
	}

	[Export]
	public int Height
	{
		get { return Size.Y; }
		set
		{
			_size.Y = value;
			_isSizeSet = true;
		}
	}

	private bool _isSizeSet = false;

	public Rectangle() : base()
	{
		_size = new Vector2I();
	}
	
	public override bool Intersects(Shape leftShape, Shape rightShape)
	{
		if (!(leftShape is Rectangle && rightShape is Rectangle))
		{
			return Intersects(leftShape, rightShape);
		}	
		throw new NotImplementedException("Rectangles can only be tested against other rectangles at this time");
	}

	public bool Intersects(Rectangle leftShape, Rectangle rightShape)
	{
		return false;
	}

	public override void ScanArea()
	{
		for (int x = TopLeft.X; x < TopLeft.X + Size.X; x++)
		{
			for (int y = TopLeft.Y; y < TopLeft.Y + Size.Y; y++)
			{
				OnEachCoordinateInArea(new Vector2I(x, y), this);
			}
		}
	}

	public override void ScanPerimeter()
	{
		for (int x = TopLeft.X; x < TopLeft.X + Size.X; x++)
		{
			OnEachCoordinateInPerimeter(new Vector2I(x, TopLeft.Y), this);
		}
		
		// Start at y+1 to account for the fact that we've already scanned that node above.
		for (int y = TopLeft.Y + 1; y < TopLeft.Y + Size.Y; y++)
		{
			OnEachCoordinateInPerimeter( new Vector2I(TopLeft.X + Size.X, y), this);	
		}

		for (int x = TopLeft.X + Size.X - 1; x >= TopLeft.X; x--)
		{
			OnEachCoordinateInArea(new Vector2I(x, TopLeft.Y + Size.Y), this);
		}

		for (int y = TopLeft.Y + Size.Y - 1; y >= TopLeft.Y; y--)
		{
			OnEachCoordinateInArea(new Vector2I(TopLeft.X, y), this);
		}
	}

	public override bool IsPointWithinShape(Vector2I point)
	{
		return (
			point.X > TopLeft.X &&
			point.Y > TopLeft.Y &&
			point.X < TopLeft.X + Size.X &&
			point.Y < TopLeft.Y + Size.Y
		);
	}

	public bool IsSizeSet
	{
		get
		{
			return _isSizeSet;
		}
	}

	private Vector2I _size;
	public override Vector2I Size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			_isSizeSet = true;
			SetCenter();
		}
	}

	private bool _isTopLeftSet = false;

	public bool IsTopLeftSet
	{
		get
		{
			return _isTopLeftSet;
		}
	}
	private Vector2I _topLeft;

	private void SetCenter()
	{
		if (IsSizeSet && IsTopLeftSet)
		{
			double halfWidth = Size.X * 0.5;
			double halfHeight = Size.Y * 0.5;
			
			Center = new Vector2I(
			TopLeft.X + (int) Math.Ceiling(halfWidth),
			  TopLeft.Y + (int) Math.Ceiling(halfHeight)
			);
		}
		else
		{
			Center = default;
		}
	}
}
