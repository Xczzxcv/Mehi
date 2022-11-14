using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    public struct Edge
    {
        public Node AdjacentNode;
        public int Cost;
    }

    public readonly struct Node : IEquatable<Node>
    {
        public readonly int Id;
        public readonly Vector3 Position;
        public IReadOnlyCollection<Edge> Edges => _edges;

        private readonly List<Edge> _edges;

        public Node(
            int id,
            Vector3 position
        )
        {
            Id = id;
            Position = position;
            _edges = new List<Edge>();
        }

        public void AddEdge(Edge newEdge)
        {
            Debug.Assert(!Equals(newEdge.AdjacentNode));
            
            _edges.Add(newEdge);
            _edges.Sort((edge, edge1) => edge.Cost - edge1.Cost);
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
        public Node StartNode;
        public List<Edge> Edges;
        
        public static Path Empty(Node startNode)
        {
            return new Path
            {
                StartNode = startNode,
                Edges = new List<Edge>(),
            };
        }
    }

    private readonly struct PathSubject : IComparable<PathSubject>
    {
        public Node Node => Edge.AdjacentNode;
        public float GCost => _prevCost + Edge.Cost; // from start
        public readonly float HCost; // from end
        public float FCost => GCost + HCost;
        public readonly Node PrevNode;

        public readonly Edge Edge;
        private readonly float _prevCost;

        public PathSubject(
            Edge edge,
            float prevCost,
            float hCost,
            Node prevNode
        )
        {
            Edge = edge;
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
    
    private Node _head;
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

        path = Path.Empty(srcNode);

        if (srcNode.Equals(targetNode))
        {
            return true;
        }

        var openPathSubjects = new List<PathSubject>();
        var closedPathSubjects = new Dictionary<Node, PathSubject>();

        var srcPathSubject = new PathSubject(
            new Edge
            {
                AdjacentNode = srcNode,
                Cost = 0
            },
            0,
            Vector3.Distance(targetNode.Position, srcNode.Position),
            default
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
                while (!pathSubjectToProcess.PrevNode.Equals(default))
                {
                    pathNodes.Add(pathSubjectToProcess.Node);
                    pathSubjectToProcess = closedPathSubjects[pathSubjectToProcess.PrevNode];
                }

                pathNodes.Reverse();
                foreach (var pathNode in pathNodes)
                {
                    var pathSubject = closedPathSubjects[pathNode];
                    path.Edges.Add(pathSubject.Edge);
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

    private static void AddAdjacentNodes(PathSubject pathSubject, Node targetNode, 
        List<PathSubject> openPathSubjects)
    {
        foreach (var edge in pathSubject.Node.Edges)
        {
            var nextToCheck = new PathSubject(
                edge,
                pathSubject.GCost,
                Vector3.Distance(targetNode.Position, edge.AdjacentNode.Position),
                pathSubject.Node
            );
            openPathSubjects.Add(nextToCheck);
        }

        openPathSubjects.Sort();
    }

    public bool TryGetNode(int id, out Node node)
    {
        _alreadyChecked.Clear();
        _queueToCheck.Clear();
        
        _queueToCheck.Enqueue(_head);

        while (_queueToCheck.Any())
        {
            var nodeToCheck = _queueToCheck.Dequeue();
            if (nodeToCheck.Id == id)
            {
                node = nodeToCheck;
                return true;
            }

            if (_alreadyChecked.Contains(nodeToCheck))
            {
                continue;
            }

            _alreadyChecked.Add(nodeToCheck);
            foreach (var edge in nodeToCheck.Edges)
            {
                _queueToCheck.Enqueue(edge.AdjacentNode);
            }
        }

        node = default;
        return false;
    }

    public bool TryAddNode(Node node, int parentNodeId, int cost)
    {
        Debug.Assert(!TryGetNode(node.Id, out _));

        if (!TryGetNode(parentNodeId, out var parentNode))
        {
            return false;
        }
        
        parentNode.AddEdge(new Edge
        {
            AdjacentNode = node,
            Cost = cost,
        });
        return true;
    }

    public void SetHead(Node newHead)
    {
        _head = newHead;
    }

    public void Reset()
    {
        _head = new Node(0, Vector3.zero);
    }
}