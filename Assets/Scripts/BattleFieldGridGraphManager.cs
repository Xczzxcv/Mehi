using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleFieldGridGraphManager
{
    public struct Config
    { }

    private readonly Config _config;
    private readonly Graph _graph;
    private readonly BattleFieldManager _fieldManager;

    private readonly Queue<(BattleFieldManager.Tile tile, Vector2Int adjacentPosition)> _positionsToProcess = new();
    private readonly HashSet<Vector2Int> _processedPositions = new();
    private static readonly Vector2Int[] AdjacentPositionShifts =
    {
        new(-1, 0),
        new(1, 0),
        new(0, -1),
        new(0, 1),
    };

    public BattleFieldGridGraphManager(Config config, BattleFieldManager fieldManager)
    {
        _config = config;
        _fieldManager = fieldManager;
        _graph = new Graph();
    }

    public void BuildGraph()
    {
        _graph.Reset();
        
        var firstTileNodePos = BattleFieldManager.GetPositionFromIndex(0, _fieldManager.FieldSize);
        var firstTileGraphPos = GetTileGraphPos(firstTileNodePos);
        var firstTile = _fieldManager.GetTile(firstTileNodePos);
        var firstTileGraphId = GetTileGraphId(in firstTile);
        var firstGraphNode = new Graph.Node(firstTileGraphId, firstTileGraphPos);
        _graph.SetHead(firstGraphNode);

        _positionsToProcess.Clear();
        _processedPositions.Clear();

        AddAdjacentTilePositionsToProcessQueue(firstTileNodePos, firstTile);
        
        while (_positionsToProcess.Any())
        {
            var (parentTile, targetPos) = _positionsToProcess.Dequeue();

            var adjacentTile = _fieldManager.GetTile(targetPos);
            var adjacentTileGraphId = GetTileGraphId(in adjacentTile);
            if (!_graph.TryGetNode(adjacentTileGraphId, out var adjacentTileGraphNode))
            {
                adjacentTileGraphNode = new Graph.Node(
                    adjacentTileGraphId,
                    GetTileGraphPos(targetPos)
                );
                _graph.TryAddNode(adjacentTileGraphNode, GetTileGraphId(in parentTile), 1);
            }

            if (_processedPositions.Contains(targetPos))
            {
                continue;
            }

            AddAdjacentTilePositionsToProcessQueue(targetPos, adjacentTile);
            _processedPositions.Add(targetPos);
        }

        void AddAdjacentTilePositionsToProcessQueue(Vector2Int tilePos, BattleFieldManager.Tile tile)
        {
            foreach (var adjacentPositionShift in AdjacentPositionShifts)
            {
                var adjacentPosition = tilePos + adjacentPositionShift;
                if (!BattleFieldManager.IsValidFieldPos(adjacentPosition, _fieldManager.FieldSize))
                {
                    continue;
                }

                _positionsToProcess.Enqueue((tile, adjacentPosition));
            }
        }

        /*for (var tileInd = 1; tileInd < _fieldManager.FieldSize * _fieldManager.FieldSize; tileInd++)
        {
            var tileGraphNode = GetOrCreateGraphNode(tileInd, out var tilePos);
            foreach (var adjacentPositionShift in AdjacentPositionShifts)
            {
                var adjacentPosition = tilePos + adjacentPositionShift;
                if (!BattleFieldManager.IsValidFieldPos(adjacentPosition, _fieldManager.FieldSize))
                {
                    continue;
                }

                var adjacentTile = _fieldManager.GetTile(adjacentPosition);
                var adjacentTileGraphId = GetTileGraphId(in adjacentTile);
                if (!_graph.TryGetNode(adjacentTileGraphId, out var adjacentTileGraphNode))
                {
                    adjacentTileGraphNode = new Graph.Node(
                        adjacentTileGraphId,
                        GetTileGraphPos(adjacentPosition)
                    );
                    _graph.AddNode(adjacentTileGraphNode);
                }

                tileGraphNode.AddEdge(new Graph.Edge
                {
                    AdjacentNode = adjacentTileGraphNode,
                    Cost = 1
                });
            }
        }*/
    }

    private static int GetTileGraphId(in BattleFieldManager.Tile tile)
    {
        return tile.Id;
    }

    private static Vector3 GetTileGraphPos(Vector2Int tilePos)
    {
        var tileGraphPos = new Vector3(tilePos.x, tilePos.y, 0);
        return tileGraphPos;
    }

    public bool TryGetPath(int srcId, int destId, out Graph.Path path)
    {
        return _graph.TryGetPath(srcId, destId, out path);
    }
}