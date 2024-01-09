using Godot;
using System;
using System.Threading.Tasks;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator.Room;

public partial class BinarySpacePartiationGenerator : RoomGenerator
{
	
	public override void _Ready()
	{
		base._Ready();
		GenerateGrid();
	}

	/// <summary>
	/// Places rooms on the map.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	protected override async Task PlaceRooms()
	{
		// Tile Type to draw (floor)
		TileType floorTileType = TileTypes.FindByName(TileType_Floor);
		
		// Create Rectangle Room encapsulating entire map.  This will be subdivided to create rooms below.
		RectangleRoom entireGrid = new RectangleRoom();
		RectangleRoom targetDivision;
		entireGrid.TopLeft = new Vector2I(0, 0);
		entireGrid.Size = new Vector2I(Grid.Size.X - 1, Grid.Size.Y - 1);
		
		// Local variable storage
		bool isRoomAvailable;
		int attempts;
		
		// Scramble the randomizer.
		GD.Randomize();

		// Invoke subdivision for each room
		for (int roomCounter = 0; roomCounter < NumberOfRooms; roomCounter++)
		{
			attempts = 0;
			do
			{
				targetDivision = GenerateRoomDivision(entireGrid);
				isRoomAvailable = IsRoomIsolated(targetDivision);

				if (isRoomAvailable)
				{
					PlaceRoom(targetDivision, floorTileType);
					await EmitUpdate();
				}
				else
				{
					attempts++;
				}
				
			} while (attempts < RoomApplicationAttemptsMax && !isRoomAvailable);
		}
	}

	private RectangleRoom[] _SubdivideX(RectangleRoom levelArea)
	{
		RectangleRoom[] divisions = { new RectangleRoom(), new RectangleRoom() }; 
		divisions[0].TopLeft = new Vector2I(levelArea.TopLeft.X, levelArea.TopLeft.Y);
		divisions[1].TopLeft = new Vector2I(levelArea.Center.X + 1, levelArea.TopLeft.Y);

		divisions[0].Size = new Vector2I(levelArea.Center.X - levelArea.TopLeft.X, levelArea.Size.Y);
		divisions[1].Size = new Vector2I(levelArea.TopLeft.X + (levelArea.Size.X - levelArea.Center.X) , levelArea.Size.Y);

		return divisions;
	}

	private RectangleRoom[] _SubdivideY(RectangleRoom levelArea)
	{
		RectangleRoom[] divisions = { new RectangleRoom(), new RectangleRoom() }; 
		divisions[0].TopLeft = new Vector2I(levelArea.TopLeft.X, levelArea.TopLeft.Y);
		divisions[1].TopLeft = new Vector2I(levelArea.TopLeft.X, levelArea.Center.Y + 1);

		divisions[0].Size = new Vector2I(levelArea.Size.X, levelArea.Center.Y - levelArea.TopLeft.Y);
		divisions[1].Size = new Vector2I(levelArea.Size.X, levelArea.TopLeft.Y + (levelArea.Size.Y - levelArea.Center.Y));

		return divisions;
	}

	private RectangleRoom GenerateRoomDivision(RectangleRoom baseArea)
	{
		// Calculate Depth of BSP Algorithm so that it can faciltiate the chosen number of rooms.
		double preciseBinaryDepth = Math.Log2(NumberOfRooms);
		int minBinaryDepth = (int) Math.Ceiling(preciseBinaryDepth);	
		
		// Randomize a bit to come up with target depth.
		int targetDepth = GD.RandRange(minBinaryDepth, minBinaryDepth + 3);
		
		// Storage for algorithm data and results
		RectangleRoom[] divisions;
		RectangleRoom targetDivision = baseArea;
		int divisionSelection;
		
		for (int currentDepth = 0; currentDepth < (targetDepth); currentDepth++)
		{
			divisions = (currentDepth % 2 == 0) ? _SubdivideY(targetDivision) : _SubdivideX(targetDivision);
					
			divisionSelection = GD.RandRange(0, 1);
			targetDivision = divisions[divisionSelection];

			// Don't create rooms that are smaller than 3 wide or 3 high.
			if (
				(targetDivision.Size.X * 0.5) <= 3.0 ||
				(targetDivision.Size.Y * 0.5) <= 3.0
			)
			{
				break;
			}
		}

		return targetDivision;
	}

	private void PlaceRoom(RectangleRoom targetDivision, TileType tileType)
	{
		Grid.MoveTo(targetDivision.TopLeft);
		Grid.FillRect(targetDivision.Size, tileType);
		Rooms.Add(targetDivision);
	}

	private async Task EmitUpdate()
	{
		if (CycleEmissionDelay > 0)
		{
			await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
			EmitSignal(SignalName.MapUpdated, Grid);
		}		
	}
}
