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

        for (var tileInd = 0; tileInd < _fieldManager.FieldSize * _fieldManager.FieldSize; tileInd++)
        {
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileInd, _fieldManager.FieldSize);
            var tile = _fieldManager.GetTile(tilePos);
            var tileGraphId = GetTileGraphId(in tile);
            var tileGraphNode = new Graph.Node(tileGraphId, GetTileGraphPos(tilePos));
            _graph.AddNode(tileGraphNode);
        }

        _positionsToProcess.Clear();
        _processedPositions.Clear();


        for (var tileInd = 0; tileInd < _fieldManager.FieldSize * _fieldManager.FieldSize; tileInd++)
        {
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileInd, _fieldManager.FieldSize);
            var tile = _fieldManager.GetTile(tilePos);
            if (!tile.Walkable)
            {
                continue;
            }

            var tileGraphId = GetTileGraphId(in tile);
            if (!_graph.TryGetNode(tileGraphId, out var tileGraphNode))
            {
                continue;
            }

            MarkAdjacentTiles(tilePos, in tileGraphNode);
        }

        /*
        while (_positionsToProcess.Any())
        {
            var (parentTile, adjacentPos) = _positionsToProcess.Dequeue();

            var adjacentTile = _fieldManager.GetTile(adjacentPos);
            var adjacentTileGraphId = GetTileGraphId(in adjacentTile);
            if (!_graph.TryGetNode(adjacentTileGraphId, out var adjacentTileGraphNode))
            {
                adjacentTileGraphNode = new Graph.Node(
                    adjacentTileGraphId,
                    GetTileGraphPos(adjacentPos)
                );
                _graph.TryAddNode(adjacentTileGraphNode, GetTileGraphId(in parentTile), 1);
            }

            if (_processedPositions.Contains(adjacentPos))
            {
                continue;
            }

            AddAdjacentTilePositionsToProcessQueue(adjacentPos, adjacentTile);
            _processedPositions.Add(adjacentPos);
        }
    */
    }

    private void MarkAdjacentTiles(Vector2Int tilePos, in Graph.Node tileGraphNode)
    {
        foreach (var adjacentPositionShift in AdjacentPositionShifts)
        {
            var adjacentPosition = tilePos + adjacentPositionShift;
            if (!BattleFieldManager.IsValidFieldPos(adjacentPosition, _fieldManager.FieldSize))
            {
                continue;
            }

            var adjacentTile = _fieldManager.GetTile(adjacentPosition);
            if (!adjacentTile.Walkable)
            {
                continue;
            }

            var adjacentTileGraphId = GetTileGraphId(in adjacentTile);
            if (!_graph.TryMarkAdjacentNode(tileGraphNode, adjacentTileGraphId, 1))
            {
                Debug.LogError($"Can't mark adjacent node {tilePos} and {adjacentPosition}");
            }
        }
    }

    private void AddAdjacentTilePositionsToProcessQueue(Vector2Int tilePos, BattleFieldManager.Tile tile)
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