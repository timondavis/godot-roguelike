using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Map.Generator.Path;

public class RoomGraph<T> where T : Model.Room
{
    public HashSet<RoomGraphNode<T>> Nodes { get; private set; }
    private SortedDictionary<int, HashSet<RoomGraphNode<T>>> xIndex;
    private SortedDictionary<int, HashSet<RoomGraphNode<T>>> yIndex;

    public RoomGraph()
    {
        Nodes = new HashSet<RoomGraphNode<T>>();
        xIndex = new SortedDictionary<int, HashSet<RoomGraphNode<T>>>();
        yIndex = new SortedDictionary<int, HashSet<RoomGraphNode<T>>>();
    }

    public void AddRoom(T room)
    {
        RoomGraphNode<T> node = new RoomGraphNode<T>(room);
        Nodes.Add(node);
        xIndex.TryAdd(room.Center.X, new HashSet<RoomGraphNode<T>>());
        yIndex.TryAdd(room.Center.Y, new HashSet<RoomGraphNode<T>>());

        xIndex[room.Center.X].Add(node);
        yIndex[room.Center.Y].Add(node);
    }

    public void AddRoomConnection(RoomGraphNode<T> left, RoomGraphNode<T> right)
    {
        left.ConnectedNodes.Add(right);
        right.ConnectedNodes.Add(left);
    }

    public List<RoomGraphNode<T>> GetClosestNodes(RoomGraphNode<T> node, int nodesMax)
    {
        SortedDictionary<int, HashSet<RoomGraphNode<T>>> candidateNodes = new SortedDictionary<int, HashSet<RoomGraphNode<T>>>();
        AddCandidateDistanceNodesFromIndex(node.Position.X, node, xIndex, candidateNodes, nodesMax + 1);
        AddCandidateDistanceNodesFromIndex(node.Position.Y, node, yIndex, candidateNodes, nodesMax + 1);
        
        List<RoomGraphNode<T>> resultsList = new List<RoomGraphNode<T>>();
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
        RoomGraphNode<T> referenceNode,
        SortedDictionary<int, HashSet<RoomGraphNode<T>>> nodeIndex,
        SortedDictionary<int, HashSet<RoomGraphNode<T>>> candidateNodes,
        int maxCandidates
        )
    {
        var values = nodeIndex.Keys
            .OrderBy(key => Math.Abs(key - nodeReferenceValue))
            .Take(maxCandidates).ToArray();

        foreach (int x in values)
        {
            var list = nodeIndex[x];
            foreach (RoomGraphNode<T> n in list)
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

                candidateNodes.TryAdd(distance, new HashSet<RoomGraphNode<T>>());
                candidateNodes[distance].Add(n);
            }
        }
    }
}