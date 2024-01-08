using Godot;
using System;
using System.Windows.Markup;

public partial class RectangleRoom : Room
{
	private Vector2I _size;
	public Vector2I Size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			_SetCenter();
		}
	}

	private Vector2I _topLeft;

	public Vector2I TopLeft
	{
		get
		{
			return _topLeft;
		}
		set
		{
			_topLeft = value;
			_SetCenter();
		}
	}

	public RectangleRoom() : base() {}

	private void _SetCenter()
	{
		if (Size != default && TopLeft != default)
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
