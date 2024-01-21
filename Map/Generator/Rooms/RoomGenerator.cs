using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Roguelike.Map.Generator.Service;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Rooms;

public abstract partial class RoomGenerator : MapGenerator
{
   	public const string TileType_Floor = "floor";

	
	[Export] public float CycleEmissionDelay { get; set; }

	public int NumberOfRooms = 0;
	public int RoomApplicationAttemptsMax = 100;
	public HashSet<Room<Rectangle>> Rooms = new HashSet<Room<Rectangle>>();
	
	public RoomGenerator() : base()
	{
		TileTypes.Add(new TileType { Name=TileType_Floor } );
	}

	public override void Begin()
	{
		GD.Randomize();
		Generate();
	}

	/// <summary>
	/// Generates the map by randomly determining the number of rooms, placing the rooms on the map,
	/// connecting the rooms, and emitting the signal to mark map finalization.
	/// </summary>
	protected async void Generate()
	{
		await PlaceRooms();
		await ConnectRooms();
		EmitSignal(SignalName.MapFinalized, Grid);	
	}

	/// <summary>
	/// Places the rooms in a specified location.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	protected abstract Task PlaceRooms();

	/// <summary>
	/// Connects the rooms by finding a path between them and connecting each pair of adjacent rooms.
	/// </summary>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	protected virtual async Task ConnectRooms()
	{
		PathFinder pf = new PathFinder(Grid, TileTypes.FindByName(TileType_Floor));
		Queue<Room<Rectangle>> path = pf.FindRectangleRoomPath(Rooms);

		Room<Rectangle> room1;
		Room<Rectangle> room2;
		RoomConnector rc = new RoomConnector(Grid, TileTypes.FindByName(TileType_Floor));
		while (path.TryDequeue(out room1) && path.TryPeek(out room2))
		{
			rc.ConnectRooms(room1, room2);
			if (CycleEmissionDelay > 0)
			{
				await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
				EmitSignal(SignalName.MapUpdated, Grid);
			}
		}
	}
}