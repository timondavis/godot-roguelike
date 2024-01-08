using System;
using System.Threading.Tasks;
using Godot;
using Roguelike.Map.Generator.Room;
using Roguelike.Map.Model;

public partial class BasicRoomPlacementGenerator : RoomGenerator
{
	[Export(PropertyHint.Range, "3,1000,1")] 
	public int RoomSizeMin { get; set; }

	[Export(PropertyHint.Range, "3,1000,1")]
	public int RoomSizeMax { get; set; }

	public override void _Ready()
	{
		base._Ready();
		RoomSizeMin = Math.Max(3, RoomSizeMin);
		RoomSizeMax = Math.Max(3, RoomSizeMax);
		GenerateGrid();
	}

	public override void GenerateGrid()
	{
		if (RoomSizeMax < RoomSizeMin)
		{
			throw new ArgumentException("RoomSizeMax cannot be less than RoomSizeMin");
		}

		if (RoomCountMax < RoomCountMin)
		{
			throw new ArgumentException("RoomCountMax cannot be less than RoomCountMin");
		}
		
		InitializeGrid();
		GD.Randomize();
		Generate();
	}

	protected override async Task PlaceRooms()
	{
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
			
		// Return false if conflict is found.
		if (IsRoomAvailable(startX, startY, roomWidth, roomHeight))
		{
			double centerX = (double)(startX) + (double)(roomWidth / 2.0);
			double centerY = (double)(startY) + (double)(roomHeight / 2.0);
			RectangleRoom room  = new RectangleRoom
			{
				Center = new Vector2I( (int) Math.Floor( centerX ), (int) Math.Floor( centerY ) ),
				TopLeft = new Vector2I(startX, startY),
				Size = new Vector2I(roomWidth, roomHeight)
			};
				
			Grid.FillRect(room.TopLeft,room.Size, floorTileType);
			Rooms.Add(room);
				
			return true;
		}
			
		return false;
	}
}
