using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public int Id;
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
    private readonly BattleFieldGridGraphManager _graphManager;

    private int _tileGlobalIndex;

    public BattleFieldManager(Config config)
    {
        FieldSize = config.Size;
        _tiles = new Tile[FieldSize * FieldSize];
        _tileConfigs = config.TileConfigs;
        _graphManager = new BattleFieldGridGraphManager(new BattleFieldGridGraphManager.Config
            {
            },
            this);

        ReadConfig(config.FieldConfig);
        _graphManager.BuildGraph();
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

    private readonly List<char> _spaceChars = new() {' ', '\t', '\n', '\r'};
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
        resultTile.Id = _tileGlobalIndex++;
        return true;
    }

    public Tile GetTile(Vector2Int tilePos)
    {
        var tileIndex = GetIndexFromPosition(tilePos, FieldSize);
        Debug.Assert(tileIndex >= 0 && tileIndex < _tiles.Length);

        return _tiles[tileIndex];
    }
    
    public Tile[] GetTiles()
    {
        return _tiles;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2Int GetPositionFromIndex(int fieldIndex, int fieldSize)
    {
        Debug.Assert(fieldIndex >= 0 && fieldIndex < fieldSize * fieldSize);
        return new Vector2Int(
            fieldIndex % fieldSize,
            fieldIndex / fieldSize
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetIndexFromPosition(Vector2Int pos, int fieldSize)
    {
        return pos.y * fieldSize + pos.x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValidFieldPos(Vector2Int pos, int fieldSize)
    {
        return 0 <= pos.x && pos.x < fieldSize
                && 0 <= pos.y && pos.y < fieldSize;
    }

    public bool TryGetPath(Vector2Int src, Vector2Int dest, out Graph.Path path)
    {
        var srcTile = GetTile(src);
        var destTile = GetTile(dest);
        return _graphManager.TryGetPath(srcTile.Id, destTile.Id, out path);
    }
    
    public bool IsValidTileToAttack(BattleMechManager.WeaponInfo weaponInfo, Vector2Int weaponPos,
        Vector2Int posToCheck)
    {
        if (!IsValidFieldPos(posToCheck, FieldSize))
        {
            return false;
        }

        var posDiff = posToCheck - weaponPos;
        if (posDiff.magnitude > weaponInfo.UseDistance)
        {
            return false;
        }

        if (weaponInfo.ProjectileType == WeaponProjectileType.Direct
            && !IsReachableTile(weaponPos, posToCheck))
        {
            return false;
        }

        return true;
    }
    
    public bool IsReachableTile(Vector2Int srcPos, Vector2Int posToCheck)
    {
        var intersectedTiles = TilesMathHelper.GetIntersectedTiles(
            srcPos, posToCheck);
        foreach (var intersectedTilePos in intersectedTiles)
        {
            var intersectedTile = GetTile(intersectedTilePos);
            if (!intersectedTile.CanShootThrough)
            {
                return false;
            }
        }

        return true;
    }
}