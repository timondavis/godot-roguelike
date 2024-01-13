using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Godot;
using Godot.NativeInterop;

namespace Roguelike.Map.Model;

public partial class GeneratorGrid : GodotObject
{
	// The width and height of the grid
	public Vector2I Size { get; private set; }
	
	// The mesh of cells on the map
	public GridCell[,] GridCells { get; private set; }
	
	// We have a pointer to index the position for traversal.
	public GridCell Current { get; private set; }

	public enum Direction
	{
		North = 0,
		NorthEast,
		East,
		SouthEast,
		South,
		SouthWest,
		West,
		NorthWest,
		Here
	};

	public GeneratorGrid(Vector2I size)
	{
		Size = size;
		GridCells = new GridCell[Size.X, Size.Y];
		InitializeGrid();
		Current = GridCells[0, 0];
	}
	
	/// <summary>
	/// Adjusts the position based on the given direction.
	/// </summary>
	/// <param name="position">The initial position</param>
	/// <param name="direction">The direction to move</param>
	/// <returns>The adjusted position</returns>
	/// <exception cref="ArgumentException">Thrown when an invalid direction is provided</exception>
	public static Vector2I AdjustPositionByDirection(Vector2I position, Direction direction)
	{
		Vector2I newPosition = new Vector2I(position.X, position.Y);

		switch (direction)
		{
			case Direction.North:
				newPosition.Y -= 1;
				break;
			case Direction.NorthEast:
				newPosition.Y -= 1;
				newPosition.X += 1;
				break;
			case Direction.East:
				newPosition.X += 1;
				break;
			case Direction.SouthEast:
				newPosition.Y += 1;
				newPosition.X += 1;
				break;
			case Direction.South:
				newPosition.Y += 1;
				break;
			case Direction.SouthWest:
				newPosition.Y += 1;
				newPosition.X -= 1;
				break;
			case Direction.West:
				newPosition.X -= 1;
				break;
			case Direction.NorthWest:
				newPosition.Y -= 1;
				newPosition.X -= 1;
				break;
			case Direction.Here:
				break;
			default:
				throw new ArgumentException(nameof(direction));
		}
		
		return newPosition;
	}

	/// <summary>
	/// Moves the `Current` GridCell pointer to the specified target position.
	/// </summary>
	/// <param name="target">The target position.</param>
	public void MoveTo(Vector2I target)
	{
		var safeTarget = SafePosition(target);
		Current = GridCells[safeTarget.X, safeTarget.Y];
	}

	/// <summary>
	/// Draws a line from the current position to the specified target position, activating tiles along the way.
	/// </summary>
	/// <param name="target">The destination position to draw the line to.</param>
	/// <param name="tileType">The type of tile to activate along the line.</param>
	/// <param name="safeMode">When not in safe mode, lines to invalid posistion will be cancelled.
	/// Safe mode will get as close as it can.</param>
	public void LineTo(Vector2I target, TileType tileType, bool safeMode = false)
	{
		var start = new Vector2I(Current.Position.X, Current.Position.Y);
		Vector2I confirmedTarget;
		if (safeMode)
		{
			confirmedTarget = SafePosition(target);
		}
		else
		{
			confirmedTarget = target;
		}
		if (IsPositionSafe(confirmedTarget))
		{
			Vector2I nextPosition = new Vector2I(start.X, start.Y);
			while (Current.Position != confirmedTarget)
			{
				if (!Current.IsActive)
				{
					Current.Activate(tileType);
				}

				int xMultiplier = (start.X).CompareTo(confirmedTarget.X) * -1;
				int yMultiplier = (start.Y).CompareTo(confirmedTarget.Y) * -1;

				nextPosition.X += (1 * xMultiplier);
				nextPosition.Y += (1 * yMultiplier);
			
				MoveTo(nextPosition);
			}
		}
	}

	/// <summary>
	/// Draws a rectangular shape using the provided dimensions and tile type.
	/// </summary>
	/// <param name="dimensions">The dimensions (width and height) of the rectangle.</param>
	/// <param name="tileType">The type of tile to use when drawing the rectangle.</param>
	/// <param name="tileType">If safe mode is turned on, system will get close as
	/// possible to invalid targets.  Otherwise, invalid targets are ignored.</param>
	public void DrawRect(Vector2I dimensions, TileType tileType, bool safeMode = false)
	{
		Vector2I topLeft = new Vector2I(Current.Position.X, Current.Position.Y);
		LineTo(new Vector2I(topLeft.X, topLeft.Y + (dimensions.Y -1)), tileType, safeMode);
		LineTo(new Vector2I(topLeft.X + (dimensions.X - 1), topLeft.Y + (dimensions.Y - 1)), tileType, safeMode);
		LineTo(new Vector2I( topLeft.X + (dimensions.X - 1), topLeft.Y), tileType, safeMode);
		LineTo(new Vector2I(topLeft.X, topLeft.Y), tileType, safeMode);
	}

	/// <summary>
	/// Fills a rectangular area with a specified tile type.
	/// </summary>
	/// <param name="dimensions">The dimensions of the rectangle to be filled.</param>
	/// <param name="tileType">The type of tile to fill the rectangle with.</param>
	/// <param name="safeMode">If safe mode is turned on, system will get close as possible to invalid targets.  Otherwise, invalid targets are ignored.</param>
	public void FillRect(Vector2I dimensions, TileType tileType, bool safeMode = false)
	{
		Vector2I topLeft = new Vector2I(Current.Position.X, Current.Position.Y);
		for (int x = topLeft.X; x < topLeft.X + dimensions.X ; x++)
		{
			for (int y = topLeft.Y; y < topLeft.Y + dimensions.Y; y++)
			{
				if (safeMode)
				{
					MoveTo(SafePosition(new Vector2I(x, y)));
				}
				else
				{
					MoveTo(new Vector2I(x,y));
				}
				Current.Activate(tileType);	
			}
		}	
	}

	/// <summary>
	/// Queries a grid cell in a relative direction.
	/// </summary>
	/// <param name="direction">The direction in which to query the grid cell.</param>
	/// <param name="safeValuesOnly">If true, unsafe values will be clamped into range. If false (default),
	/// null is returned for cells outside of range</param>
	/// <returns>The queried grid cell if safe position, or null otherwise.</returns>
	public GridCell RelativeQuery(Direction direction, bool safeValuesOnly = false)
	{
		Vector2I newPosition;
		if (safeValuesOnly)
		{
			newPosition = SafeAdjustPositionByDirection(Current.Position, direction);
		}
		else
		{ 
			newPosition = AdjustPositionByDirection(Current.Position, direction);
		}

		return (IsPositionSafe(newPosition)) ? 
			GridCells[newPosition.X, newPosition.Y] : 
			null;
	}

	/// <summary>
	/// Retrieves the adjacent GridCells in the specified directions.
	/// </summary>
	/// <param name="directionSet">The set of directions representing the adjacent cells to query.</param>
	/// <returns>A Dictionary containing the direction as the key and the corresponding GridCell as the value.  Cells which are out of grid range are returned as nulls.</returns>
	public Godot.Collections.Dictionary<Direction, GridCell> RelativeQuery(HashSet<Direction> directionSet)
	{
		var results = new Godot.Collections.Dictionary<Direction, GridCell>();
		foreach (Direction direction in directionSet)
		{
			results.Add(direction, RelativeQuery(direction));
		}

		return results;
	}
	
	/// <summary>
	/// Checks if a given position is safe within the defined Size.
	/// </summary>
	/// <param name="position">The position to check.</param>
	/// <returns>True if the position is safe, False otherwise.</returns>
	public bool IsPositionSafe(Vector2I position)
	{
		return (position.X >= 0 && position.X < Size.X &&
				position.Y >= 0 && position.Y < Size.Y);
	}
	
	/// <summary>
	/// Queries the grid for all active or inactive cells based on the specified isActive parameter.
	/// </summary>
	/// <param name="isActive">A boolean indicating whether to search for active or inactive cells.</param>
	/// <returns>A dictionary of Vector2I keys and GridCell values representing the found cells.</returns>
	public Dictionary<Vector2I, GridCell> QueryActiveCells(bool isActive = true)
	{
		var found = new Dictionary<Vector2I, GridCell>();
		for (int x = 0 ; x < Size.X ; x++)
		{
			for (int y = 0; y < Size.Y; y++)
			{
				if (isActive == GridCells[x, y].IsActive)
				{
					found.Add(new Vector2I(x,y), GridCells[x,y]);
				}	
			}
		}

		return found;
	}
	
	private void InitializeGrid()
	{
		for (int x = 0; x < Size.X; x++)
		{
			for( int y = 0 ; y < Size.Y; y++ )
			{
				GridCells[x, y] = new GridCell(x,y);
			}
		}
	}

	/// <summary>
	/// Ensures that the given position is within the bounds defined by the Size property.
	/// </summary>
	/// <param name="position">The input position to be checked.</param>
	/// <returns>A new Vector2I instance with X and Y values clamped to fit within the Size bounds.</returns>
	protected Vector2I SafePosition(Vector2I position)
	{
		return new Vector2I(
			Math.Clamp(position.X, 0, Size.X -1 ),
			Math.Clamp(position.Y, 0, Size.Y -1 )
		);
	}

	/// <summary>
	/// Adjusts position by a given direction and returns the safe target
	/// </summary>
	/// <param name="position">The original position</param>
	/// <param name="direction">The direction of adjustment</param>
	/// <returns>The adjusted position as a Vector2I</returns>
	protected Vector2I SafeAdjustPositionByDirection(Vector2I position, Direction direction)
	{
		Vector2I adjusted = AdjustPositionByDirection(position, direction);
		return SafePosition(adjusted);
	}
}
