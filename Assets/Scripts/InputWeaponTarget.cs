using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ecs.Components
{
public struct InputWeaponTarget
{
    public WeaponTargetType TargetType;
    public IReadOnlyList<int> TargetMechRoomEntities => _targetMechRoomEntities;
    public IReadOnlyList<int> TargetMechEntities => _targetMechEntities;
    public IReadOnlyList<Vector2Int> TargetTiles => _targetTiles;
    
    private List<Vector2Int> _targetTiles;
    private List<int> _targetMechRoomEntities;
    private List<int> _targetMechEntities;

    public void AddTargetRoom(int room)
    {
        _targetMechRoomEntities.Add(room);
    }

    public void DelTargetRoom(int room)
    {
        _targetMechRoomEntities.Remove(room);
    }

    public void AddTargetUnit(int unit)
    {
        _targetMechEntities.Add(unit);
    }

    public void DelTargetUnit(int unit)
    {
        _targetMechEntities.Remove(unit);
    }

    public void AddTargetTile(Vector2Int tilePos)
    {
        _targetTiles.Add(tilePos);
    }

    public void DelTargetTile(Vector2Int tilePos)
    {
        _targetTiles.Remove(tilePos);
    }

    public static InputWeaponTarget BuildTargetRooms(IEnumerable<int> rooms)
    {
        var result = BuildDefault();
        result.TargetType = WeaponTargetType.Rooms;
        result._targetMechRoomEntities.AddRange(rooms);

        return result;
    }

    public static InputWeaponTarget BuildTargetUnits(IEnumerable<int> units)
    {
        var result = BuildDefault();
        result.TargetType = WeaponTargetType.Unit;
        result._targetMechEntities.AddRange(units);

        return result;
    }

    public static InputWeaponTarget BuildTargetTiles(IEnumerable<Vector2Int> tiles)
    {
        var result = BuildDefault();
        result.TargetType = WeaponTargetType.BattleFieldTiles;
        result._targetTiles.AddRange(tiles);

        return result;
    }

    public static InputWeaponTarget BuildNonTargeted()
    {
        var result = BuildDefault();
        result.TargetType = WeaponTargetType.NonTargeted;

        return result;
    }

    private static InputWeaponTarget BuildDefault()
    {
        return new InputWeaponTarget
        {
            TargetType = WeaponTargetType.None,
            _targetTiles = new List<Vector2Int>(),
            _targetMechEntities = new List<int>(),
            _targetMechRoomEntities = new List<int>(),
        };
    }

    public static InputWeaponTarget BuildFromTargetType(WeaponTargetType weaponTargetType)
    {
        return weaponTargetType switch
        {
            WeaponTargetType.Rooms => BuildTargetRooms(Enumerable.Empty<int>()),
            WeaponTargetType.Unit => BuildTargetUnits(Enumerable.Empty<int>()),
            WeaponTargetType.BattleFieldTiles => BuildTargetTiles(Enumerable.Empty<Vector2Int>()),
            WeaponTargetType.NonTargeted => BuildNonTargeted(),
            _ => throw new System.NotImplementedException()
        };
    }
}
}