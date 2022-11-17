using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    public readonly struct Node : IEquatable<Node>
    {
        public readonly int Id;
        public readonly Vector3 Position;

        public static readonly Node Default = new(-1, Vector3.zero);
        
        public Node(
            int id,
            Vector3 position
        )
        {
            Id = id;
            Position = position;
        }

        public bool Equals(Node otherNode)
        {
            return Id == otherNode.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public struct Path
    {
        public struct Part
        {
            public Node Node;
            public  int Cost;
        }

        public List<Part> Parts;

        public static Path Empty()
        {
            return new Path
            {
                Parts = new List<Part>(),
            };
        }
    }

    private readonly struct PathSubject : IComparable<PathSubject>
    {
        public readonly Node Node;
        public float GCost => _prevCost + CurrCost; // from start
        public readonly float HCost; // from end
        public float FCost => GCost + HCost;
        public readonly Node PrevNode;

        public readonly int CurrCost;
        private readonly float _prevCost;

        public PathSubject(
            Node node,
            int currCost,
            float prevCost,
            float hCost,
            Node prevNode
        )
        {
            Node = node;
            CurrCost = currCost;
            _prevCost = prevCost;
            HCost = hCost;
            PrevNode = prevNode;
        }

        public int CompareTo(PathSubject other)
        {
            var fCostComparison = FCost.CompareTo(other.FCost);
            if (fCostComparison != 0)
            {
                return fCostComparison;
            }

            var hCostComparison = HCost.CompareTo(other.HCost);
            if (hCostComparison != 0)
            {
                return hCostComparison;
            }

            return GCost.CompareTo(other.GCost);
        }
    }
    
    private readonly Dictionary<Node, HashSet<Node>> _adjacenсyMap = new();
    private readonly Dictionary<(Node, Node), int> _costMap = new();
    private readonly Dictionary<int, Node> _nodes = new();
    private readonly HashSet<Node> _alreadyChecked = new();
    private readonly Queue<Node> _queueToCheck = new();

    public Graph()
    {
        Reset();
    }
    
    public bool TryGetPath(int srcId, int targetId, out Path path)
    {
        if (!TryGetNode(srcId, out var srcNode)
            || !TryGetNode(targetId, out var targetNode))
        {
            Debug.LogError($"Can't find src ({srcId}) or target ({targetId}) node");
            path = default;
            return false;
        }

        path = Path.Empty();

        if (srcNode.Equals(targetNode))
        {
            return true;
        }

        var openPathSubjects = new List<PathSubject>();
        var closedPathSubjects = new Dictionary<Node, PathSubject>();

        var srcPathSubject = new PathSubject(
            srcNode,
            0,
            0,
            Vector3.Distance(targetNode.Position, srcNode.Position),
            Node.Default
        );
        closedPathSubjects.Add(srcNode, srcPathSubject);
        AddAdjacentNodes(srcPathSubject, targetNode, openPathSubjects);

        while (openPathSubjects.Any())
        {
            var pathSubjectToCheck = openPathSubjects.First();
            openPathSubjects.RemoveAt(0);

            if (pathSubjectToCheck.Node.Equals(targetNode))
            {
                closedPathSubjects.Add(pathSubjectToCheck.Node, pathSubjectToCheck);

                var pathSubjectToProcess = pathSubjectToCheck;
                var pathNodes = new List<Node>();
                while (true)
                {
                    pathNodes.Add(pathSubjectToProcess.Node);
                    if (pathSubjectToProcess.PrevNode.Equals(Node.Default))
                    {
                        break;
                    }

                    pathSubjectToProcess = closedPathSubjects[pathSubjectToProcess.PrevNode];
                }

                pathNodes.Reverse();
                foreach (var pathNode in pathNodes)
                {
                    var pathSubject = closedPathSubjects[pathNode];
                    var pathPart = new Path.Part
                    {
                        Node = pathSubject.Node,
                        Cost = pathSubject.CurrCost,
                    };
                    path.Parts.Add(pathPart);
                }

                return true;
            }

            if (closedPathSubjects.ContainsKey(pathSubjectToCheck.Node))
            {
                continue;
            }

            closedPathSubjects.Add(pathSubjectToCheck.Node, pathSubjectToCheck);
            AddAdjacentNodes(pathSubjectToCheck, targetNode, openPathSubjects);
        }
        
        return false;
    }

    private void AddAdjacentNodes(PathSubject pathSubject, Node targetNode, 
        List<PathSubject> openPathSubjects)
    {
        var adjacentNodes = _adjacenсyMap[pathSubject.Node];
        foreach (var adjacentNode in adjacentNodes)
        {
            var cost = _costMap[(adjacentNode, pathSubject.Node)];
            var nextToCheck = new PathSubject(
                adjacentNode,
                cost,
                pathSubject.GCost,
                Vector3.Distance(targetNode.Position, adjacentNode.Position),
                pathSubject.Node
            );
            openPathSubjects.Add(nextToCheck);
        }

        openPathSubjects.Sort();
    }

    public bool TryGetNode(int id, out Node node)
    {
        return _nodes.TryGetValue(id, out node);
    }

    public void AddNode(Node node)
    {
        Debug.Assert(!TryGetNode(node.Id, out _));

        _nodes[node.Id] = node;
        _adjacenсyMap[node] = new HashSet<Node>();
    }

    public bool TryMarkAdjacentNode(Node node, int adjacentNodeId, int cost)
    {
        if (!TryGetNode(adjacentNodeId, out var adjacentNode))
        {
            return false;
        }

        _adjacenсyMap[node].Add(adjacentNode);
        _adjacenсyMap[adjacentNode].Add(node);
        _costMap[(node, adjacentNode)] = cost;
        _costMap[(adjacentNode, node)] = cost;

        return true;
    }

    public void Reset()
    {
        _nodes.Clear();
        _adjacenсyMap.Clear();
        _costMap.Clear();
    }
}