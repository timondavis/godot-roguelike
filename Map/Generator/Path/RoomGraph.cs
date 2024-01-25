using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

public class RoomGraph
{
	public HashSet<RoomGraphNode> Nodes { get; private set; }
	private SortedDictionary<int, HashSet<RoomGraphNode>> xIndex;
	private SortedDictionary<int, HashSet<RoomGraphNode>> yIndex;

	public RoomGraph()
	{
		Nodes = new HashSet<RoomGraphNode>();
		xIndex = new SortedDictionary<int, HashSet<RoomGraphNode>>();
		yIndex = new SortedDictionary<int, HashSet<RoomGraphNode>>();
	}

	public void AddRoom(Room room)
	{
		RoomGraphNode node = new RoomGraphNode(room);
		Nodes.Add(node);
		xIndex.TryAdd(node.Room.Location.X, new HashSet<RoomGraphNode>());
		yIndex.TryAdd(node.Room.Location.Y, new HashSet<RoomGraphNode>());
		
		xIndex[node.Room.Location.X].Add(node);
		yIndex[node.Room.Location.Y].Add(node);
	}

	public void AddRoomConnection(RoomGraphNode left, RoomGraphNode right)
	{
		left.ConnectedNodes.Add(right);
		right.ConnectedNodes.Add(left);
	}

	public List<RoomGraphNode> GetClosestNodes(RoomGraphNode node, int nodesMax)
	{
		SortedDictionary<int, HashSet<RoomGraphNode>> candidateNodes = new SortedDictionary<int, HashSet<RoomGraphNode>>();
		AddCandidateDistanceNodesFromIndex(node.Position.X, node, xIndex, candidateNodes, nodesMax + 1);
		AddCandidateDistanceNodesFromIndex(node.Position.Y, node, yIndex, candidateNodes, nodesMax + 1);
		
		List<RoomGraphNode> resultsList = new List<RoomGraphNode>();
		var candidatesEnum = candidateNodes.GetEnumerator();
		while (resultsList.Count < nodesMax && candidatesEnum.MoveNext()) 
		{
			var list = candidatesEnum.Current.Value;
			var candidateValuesEnum = list.GetEnumerator();
			while (resultsList.Count < nodesMax && candidateValuesEnum.MoveNext())
			{
				resultsList.Add(candidateValuesEnum.Current);
			}

			candidateValuesEnum.Dispose();
		}

		candidatesEnum.Dispose();
		return resultsList;
	}

	private void AddCandidateDistanceNodesFromIndex(
		int nodeReferenceValue,
		RoomGraphNode referenceNode,
		SortedDictionary<int, HashSet<RoomGraphNode>> nodeIndex,
		SortedDictionary<int, HashSet<RoomGraphNode>> candidateNodes,
		int maxCandidates
		)
	{
		var values = nodeIndex.Keys
			.OrderBy(key => Math.Abs(key - nodeReferenceValue))
			.Take(maxCandidates).ToArray();

		foreach (int x in values)
		{
			var list = nodeIndex[x];
			foreach (RoomGraphNode n in list)
			{
				if (n.Room.Id == referenceNode.Room.Id)
				{
					continue;
				}
				
				var posX = n.Position.X;
				var posY = n.Position.Y;

				var aSqr = Math.Abs(referenceNode.Position.X - posX) ^ 2;
				var bSqr = Math.Abs(referenceNode.Position.Y - posY) ^ 2;
				var distance = (int)Math.Round(Math.Sqrt(aSqr + bSqr));

				candidateNodes.TryAdd(distance, new HashSet<RoomGraphNode>());
				candidateNodes[distance].Add(n);
			}
		}
	}
}
