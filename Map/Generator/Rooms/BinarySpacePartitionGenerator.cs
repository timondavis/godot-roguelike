using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Roguelike.Map.Generator.Path;
using Roguelike.Map.Generator.Service;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Rooms;

public partial class BinarySpacePartitionGenerator : RoomGenerator
{
	[Export(PropertyHint.Range, "1,1000,1")] public int DivisionDepth { get; set; } 
	[Export(PropertyHint.Range, "1,10,1")] public int MinConnectionsPerRoom { get; set; }
	[Export(PropertyHint.Range, "1,10,1")] public int MaxConnectionsPerRoom { get; set; }
	private const int AxisDivisions = 2;
	private RoomTree<Rectangle> Tree;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DivisionDepth = Math.Max(1, DivisionDepth);
		MinConnectionsPerRoom = Math.Max(1, MinConnectionsPerRoom);
		MaxConnectionsPerRoom = Math.Max(1, MaxConnectionsPerRoom);
	}
	
	public override void Begin()
	{
		GD.Randomize();
		Generate();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	protected override async Task PlaceRooms()
	{
		TileType floorTile = TileTypes.FindByName(TileType_Floor);
		
		Tree = InitializeTree();
		var xOrY = GD.RandRange(0, 1);

		int depthRemaining = DivisionDepth;
		if (xOrY == 0)
		{
			SubdivideNodeX(Tree.Head, depthRemaining);
		}
		else
		{
			SubdivideNodeX(Tree.Head, depthRemaining);
		}
		
		await PlaceRoom(Tree.Head, floorTile);
	}

	private void SubdivideNodeX(RoomTreeNode node, int depthRemaining)
	{
		ShapedRoom<Rectangle>[] divisions = new ShapedRoom<Rectangle>[AxisDivisions];
		ShapedRoom<Rectangle> nodeShapedRoom = node.GetShapedRoom<Rectangle>();
		double divisionSizeMultiplier = 0.5;
		Vector2I roomSize = new Vector2I(
			(int) Math.Floor(nodeShapedRoom.Shape.Size.X * divisionSizeMultiplier),
			nodeShapedRoom.Shape.Size.Y
		);
		for (int divIdx = 0; divIdx < AxisDivisions; divIdx++)
		{
			divisions[divIdx] = RoomService.Instance.GenerateShapedRoom<Rectangle>();
			divisions[divIdx].Shape.TopLeft = new Vector2I(
				nodeShapedRoom.Shape.TopLeft.X + (divIdx * (int) Math.Ceiling(nodeShapedRoom.Shape.Size.X / (double) AxisDivisions)),
				nodeShapedRoom.Shape.TopLeft.Y
			);
			divisions[divIdx].Shape.Size = roomSize;
		}

		node.Left = new RoomTreeNode(divisions[0]);
		node.Left.Parent = node;
		node.Right = new RoomTreeNode(divisions[1]);
		node.Right.Parent = node;

		depthRemaining -= 1;
		
		if (depthRemaining > 0)
		{
			SubdivideNodeY(node.Left, depthRemaining);
			SubdivideNodeY(node.Right, depthRemaining);
		} 
	}

	private void SubdivideNodeY(RoomTreeNode node, int depthRemaining)
	{
		ShapedRoom<Rectangle>[] divisions = new ShapedRoom<Rectangle>[AxisDivisions];
		ShapedRoom<Rectangle> nodeShapedRoom = node.GetShapedRoom<Rectangle>();
		Vector2I roomSize = new Vector2I(
			nodeShapedRoom.Shape.Size.X,
			(int) Math.Floor(nodeShapedRoom.Shape.Size.Y / (double) AxisDivisions)
		);
		for (int divIdx = 0; divIdx < AxisDivisions; divIdx++)
		{
			divisions[divIdx] = RoomService.Instance.GenerateShapedRoom<Rectangle>();
			divisions[divIdx].Shape.TopLeft = new Vector2I(
				nodeShapedRoom.Shape.TopLeft.X,
				nodeShapedRoom.Shape.TopLeft.Y + (divIdx * (int) Math.Ceiling(nodeShapedRoom.Shape.Size.Y / (double) AxisDivisions))
			);
			divisions[divIdx].Shape.Size = roomSize;
		}

		node.Left = new RoomTreeNode(divisions[0]);
		node.Left.Parent = node;
		node.Right = new RoomTreeNode(divisions[1]);
		node.Right.Parent = node;

		depthRemaining -= 1;
		
		if (depthRemaining > 0)
		{
			SubdivideNodeX(node.Left, depthRemaining);
			SubdivideNodeX(node.Right, depthRemaining);
		} 
	}

	private async Task PlaceRoom(RoomTreeNode node, TileType tileType)
	{
		ShapedRoom<Rectangle> nodeShapedRoom = node.GetShapedRoom<Rectangle>();
		// 0. Don't do anything if the node has children.
		if (node.Left != null)
		{
			await PlaceRoom(node.Left, tileType);
		}

		if (node.Right != null)
		{
			await PlaceRoom(node.Right, tileType);
		}

		else
		{
			// 1. Mutate Room 
			Vector2I maxDimensions = new Vector2I(nodeShapedRoom.Shape.Size.X, nodeShapedRoom.Shape.Size.Y);

			Vector2I roomDimensions = new Vector2I(
				GD.RandRange((int)Math.Floor(maxDimensions.X * .33), maxDimensions.X),
				GD.RandRange((int)Math.Floor(maxDimensions.Y * .33), maxDimensions.Y)
			);

			Vector2I roomOffset = new Vector2I(
				GD.RandRange(0, nodeShapedRoom.Shape.Size.X - roomDimensions.X),
				GD.RandRange(0, nodeShapedRoom.Shape.Size.Y - roomDimensions.Y)
			);

			ShapedRoom<Rectangle> revisedRoom = RoomService.Instance.GenerateShapedRoom<Rectangle>();
			revisedRoom.Shape.Size = roomDimensions;
			revisedRoom.Shape.TopLeft = new Vector2I(
				nodeShapedRoom.Shape.TopLeft.X + roomOffset.X,
				nodeShapedRoom.Shape.TopLeft.Y + roomOffset.Y
			);

			node.Room = revisedRoom;
		
			// 2. Draw Room
			Grid.MoveTo(revisedRoom.Shape.TopLeft);
			Grid.FillRect(revisedRoom.Shape.Size, tileType, true);
		
			if (CycleEmissionDelay > 0)
			{
				await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
				EmitSignal(MapGenerator.SignalName.MapUpdated, Grid);
			}	
		}
	}

	private RoomTree<Rectangle> InitializeTree()
	{
		ShapedRoom<Rectangle> entireRoom = RoomService.Instance.GenerateShapedRoom<Rectangle>();
		entireRoom.Shape.TopLeft = new Vector2I(0, 0);
		entireRoom.Shape.Size = new Vector2I(
			Grid.Size.X,
			Grid.Size.Y
		);

		RoomTree<Rectangle> tree = new RoomTree<Rectangle>();
		tree.Head = new RoomTreeNode(entireRoom);
		
		return tree;
	}

	protected override async Task ConnectRooms()
	{
		RoomGraph rg = new RoomGraph();
		Tree.LoadChildrenToGraph(rg);
		List<RoomGraphNode> nodes = rg.Nodes.ToList();
		int nodesMax;
		foreach (var node in nodes)
		{
			nodesMax = GD.RandRange(MinConnectionsPerRoom, MaxConnectionsPerRoom);
			List<RoomGraphNode> closestNodes = rg.GetClosestNodes(node, nodesMax);
			foreach (RoomGraphNode closeNode in closestNodes)
			{
				rg.AddRoomConnection(node, closeNode);
			}
		}

		RoomConnector rc = new RoomConnector(Grid, TileTypes.FindByName(TileType_Floor));
		HashSet<Tuple<int,int>> consumedConnections = new HashSet<Tuple<int,int>>();
		foreach (var left in nodes)
		{
			foreach (var right in left.ConnectedNodes)
			{
				Tuple<int, int> connection = new Tuple<int, int>(
					Math.Min(left.Room.Id, right.Room.Id),
					Math.Max(left.Room.Id, right.Room.Id)
				);
				
				if (!consumedConnections.Contains(connection))
				{
					consumedConnections.Add(connection);
					rc.ConnectRooms(left.Room, right.Room);
					
					if (CycleEmissionDelay > 0)
					{
						await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
						EmitSignal(MapGenerator.SignalName.MapUpdated, Grid);
					}	
				}
			}
		}
	}
}