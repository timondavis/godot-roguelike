using Godot;
using System;
using System.Windows.Markup;
using Roguelike.Map.Model;

public partial class RectangleRoom : Room
{
	
	private bool _isSizeSet = false;

	public bool IsSizeSet
	{
		get
		{
			return _isSizeSet;
		}
	}

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
			_isSizeSet = true;
			_SetCenter();
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

	public Vector2I TopLeft
	{
		get
		{
			return _topLeft;
		}
		set
		{
			_topLeft = value;
			_isTopLeftSet = true;
			_SetCenter();
		}
	}


	private void _SetCenter()
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
	
	public RectangleRoom() : base() {}
}
