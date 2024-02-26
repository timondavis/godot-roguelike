using Godot;
using System;
using System.Threading.Tasks;
using Roguelike.Map.Generator.Service;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Rooms;

public partial class InformalBinarySpacePartitionGenerator : RoomGenerator
{
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMin { get; set; }
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMax { get; set; }
	
	public override void _Ready()
	{
		base._Ready();
		RoomCountMin = Math.Max(1, RoomCountMin);
		RoomCountMax = Math.Max(1, RoomCountMax);
	}

	public override void Begin()
	{
		if (RoomCountMax < RoomCountMin)
		{
			throw new ArgumentException("RoomCountMax cannot be less than RoomCountMin");
		}
		
		GD.Randomize();
		Generate();
	}

	/// <summary>
	/// Places rooms on the map.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	protected override async Task PlaceRooms()
	{
		// Tile Type to draw (floor)
		TileType floorTileType = TileTypes.FindByName(TileType_Floor);
		
		// Determine Number of Rooms
		NumberOfRooms = GD.RandRange(RoomCountMin, RoomCountMax);
		
		// Create Rectangle Room encapsulating entire map.  This will be subdivided to create rooms below.
		ShapedRoom<Rectangle> entireGrid = RoomService.Instance.GenerateShapedRoom<Rectangle>();
		ShapedRoom<Rectangle> targetDivision;
		entireGrid.Shape.TopLeft = new Vector2I(0, 0);
		entireGrid.Shape.Size = new Vector2I(Grid.Size.X - 1, Grid.Size.Y - 1);
		
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
				isRoomAvailable = RoomService.Instance.IsRoomAreaIsolated(targetDivision, Grid);

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

	private ShapedRoom<Rectangle>[] _SubdivideX(ShapedRoom<Rectangle> levelArea)
	{
		ShapedRoom<Rectangle>[] divisions = { RoomService.Instance.GenerateShapedRoom<Rectangle>(), RoomService.Instance.GenerateShapedRoom<Rectangle>()}; 
		divisions[0].Shape.TopLeft = new Vector2I(levelArea.Shape.TopLeft.X, levelArea.Shape.TopLeft.Y);
		divisions[1].Shape.TopLeft = new Vector2I(levelArea.Shape.Center.X + 1, levelArea.Shape.TopLeft.Y);

		divisions[0].Shape.Size = new Vector2I(levelArea.Shape.Center.X - levelArea.Shape.TopLeft.X, levelArea.Shape.Size.Y);
		divisions[1].Shape.Size = new Vector2I(levelArea.Shape.TopLeft.X + (levelArea.Shape.Size.X - levelArea.Shape.Center.X) , levelArea.Shape.Size.Y);

		return divisions;
	}

	private ShapedRoom<Rectangle>[] _SubdivideY(ShapedRoom<Rectangle> levelArea)
	{
		ShapedRoom<Rectangle>[] divisions = { RoomService.Instance.GenerateShapedRoom<Rectangle>(), RoomService.Instance.GenerateShapedRoom<Rectangle>() }; 
		divisions[0].Shape.TopLeft = new Vector2I(levelArea.Shape.TopLeft.X, levelArea.Shape.TopLeft.Y);
		divisions[1].Shape.TopLeft = new Vector2I(levelArea.Shape.TopLeft.X, levelArea.Shape.Center.Y + 1);

		divisions[0].Shape.Size = new Vector2I(levelArea.Shape.Size.X, levelArea.Shape.Center.Y - levelArea.Shape.TopLeft.Y);
		divisions[1].Shape.Size = new Vector2I(levelArea.Shape.Size.X, levelArea.Shape.TopLeft.Y + (levelArea.Shape.Size.Y - levelArea.Shape.Center.Y));

		return divisions;
	}

	private ShapedRoom<Rectangle> GenerateRoomDivision(ShapedRoom<Rectangle> baseArea)
	{
		// Calculate Depth of BSP Algorithm so that it can faciltiate the chosen number of rooms.
		double preciseBinaryDepth = Math.Log2(NumberOfRooms);
		int minBinaryDepth = (int) Math.Ceiling(preciseBinaryDepth);	
		
		// Randomize a bit to come up with target depth.
		int targetDepth = GD.RandRange(minBinaryDepth, minBinaryDepth + 3);
		
		// Storage for algorithm data and results
		ShapedRoom<Rectangle>[] divisions;
		ShapedRoom<Rectangle> targetDivision = baseArea;
		int divisionSelection;
		
		for (int currentDepth = 0; currentDepth < (targetDepth); currentDepth++)
		{
			divisions = (currentDepth % 2 == 0) ? _SubdivideY(targetDivision) : _SubdivideX(targetDivision);
					
			divisionSelection = GD.RandRange(0, 1);
			targetDivision = divisions[divisionSelection];

			// Don't create rooms that are smaller than 3 wide or 3 high.
			if (
				(targetDivision.Shape.Size.X * 0.5) <= 3.0 ||
				(targetDivision.Shape.Size.Y * 0.5) <= 3.0
			)
			{
				break;
			}
		}

		return targetDivision;
	}

	private void PlaceRoom(ShapedRoom<Rectangle> targetDivision, TileType tileType)
	{
		Grid.MoveTo(targetDivision.Shape.TopLeft);
		Grid.FillRect(targetDivision.Shape.Size, tileType);
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
