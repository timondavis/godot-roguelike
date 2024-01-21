using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator.Path;

public class RoomGraph<TRoomShape> where TRoomShape : Shape, new()
{
    public HashSet<RoomGraphNode<TRoomShape>> Nodes { get; private set; }
    private SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>> xIndex;
    private SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>> yIndex;

    public RoomGraph()
    {
        Nodes = new HashSet<RoomGraphNode<TRoomShape>>();
        xIndex = new SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>>();
        yIndex = new SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>>();
    }

    public void AddRoom(Room<TRoomShape> room)
    {
        RoomGraphNode<TRoomShape> node = new RoomGraphNode<TRoomShape>(room);
        Nodes.Add(node);
        xIndex.TryAdd(room.Shape.Center.X, new HashSet<RoomGraphNode<TRoomShape>>());
        yIndex.TryAdd(room.Shape.Center.Y, new HashSet<RoomGraphNode<TRoomShape>>());

        xIndex[room.Shape.Center.X].Add(node);
        yIndex[room.Shape.Center.Y].Add(node);
    }

    public void AddRoomConnection(RoomGraphNode<TRoomShape> left, RoomGraphNode<TRoomShape> right)
    {
        left.ConnectedNodes.Add(right);
        right.ConnectedNodes.Add(left);
    }

    public List<RoomGraphNode<TRoomShape>> GetClosestNodes(RoomGraphNode<TRoomShape> node, int nodesMax)
    {
        SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>> candidateNodes = new SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>>();
        AddCandidateDistanceNodesFromIndex(node.Position.X, node, xIndex, candidateNodes, nodesMax + 1);
        AddCandidateDistanceNodesFromIndex(node.Position.Y, node, yIndex, candidateNodes, nodesMax + 1);
        
        List<RoomGraphNode<TRoomShape>> resultsList = new List<RoomGraphNode<TRoomShape>>();
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
        RoomGraphNode<TRoomShape> referenceNode,
        SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>> nodeIndex,
        SortedDictionary<int, HashSet<RoomGraphNode<TRoomShape>>> candidateNodes,
        int maxCandidates
        )
    {
        var values = nodeIndex.Keys
            .OrderBy(key => Math.Abs(key - nodeReferenceValue))
            .Take(maxCandidates).ToArray();

        foreach (int x in values)
        {
            var list = nodeIndex[x];
            foreach (RoomGraphNode<TRoomShape> n in list)
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

                candidateNodes.TryAdd(distance, new HashSet<RoomGraphNode<TRoomShape>>());
                candidateNodes[distance].Add(n);
            }
        }
    }
}