using System;
using System.Threading.Tasks;
using Godot;
using Roguelike.Script.Map.Generator.Service;
using Roguelike.Script.Map.Model;
using Roguelike.Script.Map.Model.Shapes;

namespace Roguelike.Script.Map.Generator.Rooms;

public partial class BasicRoomPlacementGenerator : RoomGenerator

{
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMin { get; set; }
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMax { get; set; }
	
	[Export(PropertyHint.Range, "3,1000,1")] public int RoomSizeMin { get; set; } 
	[Export(PropertyHint.Range, "3,1000,1")] public int RoomSizeMax { get; set; }

	public override void _Ready()
	{
		base._Ready();
		RoomSizeMin = Math.Max(3, RoomSizeMin);
		RoomSizeMax = Math.Max(3, RoomSizeMax);
		RoomCountMin = Math.Max(1, RoomCountMin);
		RoomCountMax = Math.Max(1, RoomCountMax);
	}

	/// <summary>
	/// Generates a grid for room generation.
	/// </summary>
	/// <exception cref="ArgumentException">Thrown when the maximum room size is less than the minimum room size or when the maximum room count is less than the minimum room count.</exception>
	public override void Begin()
	{
		if (RoomSizeMax < RoomSizeMin)
		{
			throw new ArgumentException("RoomSizeMax cannot be less than RoomSizeMin");
		}

		if (RoomCountMax < RoomCountMin)
		{
			throw new ArgumentException("RoomCountMax cannot be less than RoomCountMin");
		}
		
		GD.Randomize();
		Generate();
	}

	protected override async Task PlaceRooms()
	{
		NumberOfRooms = GD.RandRange(RoomCountMin, RoomCountMax);
		int currentRoomCounter = 0;
		int attemptsForCurrentRoom = 0;
		bool isRoomPlaced;
		while (currentRoomCounter < NumberOfRooms && attemptsForCurrentRoom < RoomApplicationAttemptsMax)
		{
			isRoomPlaced = PlaceRoom();
			if (isRoomPlaced)
			{
				currentRoomCounter++;
				attemptsForCurrentRoom = 0;

				if (CycleEmissionDelay > 0)
				{
					await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
					EmitSignal(SignalName.MapUpdated, Grid);
				}
			}
			else
			{
				attemptsForCurrentRoom++;
			}
		} 
	}

	private bool PlaceRoom()
	{
		int roomWidth = GD.RandRange(RoomSizeMin, RoomSizeMax);
		int roomHeight = GD.RandRange(RoomSizeMin, RoomSizeMax);
		int startX = GD.RandRange(0, Width - roomWidth);
		int startY = GD.RandRange(0, Height - roomHeight);
		TileType floorTileType = TileTypes.FindByName(TileType_Floor);
		
		double centerX = (double)(startX) + (double)(roomWidth / 2.0);
		double centerY = (double)(startY) + (double)(roomHeight / 2.0);
		ShapedRoom<Rectangle> room = RoomService.Instance.GenerateShapedRoom<Rectangle>();
		room.Shape.Center = new Vector2I( (int) Math.Floor( centerX ), (int) Math.Floor( centerY ) );
		room.Shape.TopLeft = new Vector2I(startX, startY);
		room.Shape.Size = new Vector2I(roomWidth, roomHeight); 
			
		// Return false if conflict is found.
		if (RoomService.Instance.IsRoomAreaIsolated(room, Grid))
		{
			Grid.MoveTo(room.Shape.TopLeft);
			Grid.FillRect(room.Shape.Size, floorTileType);
			Rooms.Add(room);
				
			return true;
		}
			
		return false;
	}
}
