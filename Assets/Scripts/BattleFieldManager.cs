using System;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

public class BattleFieldManager
{
    public enum TileType
    {
        Grass,
        Water,
        Mountain
    }
    
    [Serializable]
    public struct Tile
    {
        public TileType Type;
        public bool Walkable;
        public bool CanShootThrough;
    }

    public struct Config
    {
        public int Size;
        public string FieldConfig;
        public Tile[] TileConfigs;
    }

    public readonly int FieldSize;
    private readonly Tile[] _tileConfigs;
    private readonly Tile[] _tiles;

    public BattleFieldManager(Config config)
    {
        FieldSize = config.Size;
        _tiles = new Tile[FieldSize * FieldSize];
        _tileConfigs = config.TileConfigs;

        ReadConfig(config.FieldConfig);
    }

    private void ReadConfig(string fieldConfig)
    {
        var tileIndex = 0;
        foreach (var currTileConfig in fieldConfig)
        {
            if (!TryGetTile(currTileConfig, out var tile))
            {
                continue;
            }

            _tiles[tileIndex] = tile;
            tileIndex++;
        }
    }

    private readonly List<char> _spaceChars = new List<char>{' ', '\t', '\n', '\r'};
    private bool TryGetTile(char tileConfig, out Tile resultTile)
    {
        resultTile = default;
        if (_spaceChars.Contains(tileConfig))
        {
            return false;
        }

        var resultIndex = tileConfig - '0';
        if (resultIndex < 0 || resultIndex >= _tileConfigs.Length)
        {
            Debug.LogError($"Wrong number, Fool! {resultIndex}('{tileConfig}')");
            return false;
        }

        resultTile = _tileConfigs[resultIndex];
        return true;
    }

    public Tile GetTile(int x, int y)
    {
        var index = y * FieldSize + x;
        Debug.Assert(index >= 0 && index < _tiles.Length);

        return _tiles[index];
    }
    
    public Tile[] GetTiles()
    {
        return _tiles;
    }

    public static Vector2Int GetPositionFromIndex(int fieldIndex, int fieldSize)
    {
        Debug.Assert(fieldIndex >= 0 && fieldIndex < fieldSize * fieldSize);
        return new Vector2Int(
            fieldIndex % fieldSize,
            fieldIndex / fieldSize
        );
    }

    public static int GetIndexFromPosition(Vector2Int pos, int fieldSize)
    {
        return pos.y * fieldSize + pos.x;
    }
}