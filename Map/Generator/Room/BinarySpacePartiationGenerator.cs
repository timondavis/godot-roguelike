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
	
	protected override async Task PlaceRooms()
	{
		// Calculate Depth of BSP Algorithm such that it should faciltiate the chosen number of rooms.
		double preciseBinaryDepth = Math.Log2(NumberOfRooms);
		int minBinaryDepth = (int) Math.Ceiling(preciseBinaryDepth);
		
		TileType floorTileType = TileTypes.FindByName(TileType_Floor);
		RectangleRoom[] divisions;
		RectangleRoom entireGrid = new RectangleRoom();
		RectangleRoom targetDivision;
		entireGrid.TopLeft = new Vector2I(0, 0);
		entireGrid.Size = new Vector2I(Grid.Size.X - 1, Grid.Size.Y - 1);

		GD.Randomize();
		bool isRoomAvailable;
		int attempts;

		for (int roomCounter = 0; roomCounter < NumberOfRooms; roomCounter++)
		{
			attempts = 0;
			targetDivision = entireGrid;
			do
			{
				int targetDepth = GD.RandRange(minBinaryDepth, minBinaryDepth + 3);
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

				isRoomAvailable = IsRoomIsolated(targetDivision);

				if (isRoomAvailable)
				{
					Grid.MoveTo(targetDivision.TopLeft);
					Grid.DrawRect(targetDivision.Size, floorTileType);
					Rooms.Add(targetDivision);
					if (CycleEmissionDelay > 0)
					{
						await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
						EmitSignal(SignalName.MapUpdated, Grid);
					}
				}
				else
				{
					attempts++;
					targetDivision = entireGrid;
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


}
