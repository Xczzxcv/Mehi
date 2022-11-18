using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace Ecs.Components
{
public struct InputWeaponTarget
{
    public WeaponTargetType TargetType;
    public List<EcsPackedEntity> TargetMechRooms;
    public List<EcsPackedEntity> TargetMechEntities;
    public List<Vector2Int> TargetTiles;

    public static InputWeaponTarget BuildTargetRooms(IEnumerable<EcsPackedEntity> rooms)
    {
        var result = BuildDefault();
        result.TargetType = WeaponTargetType.Rooms;
        result.TargetMechRooms.AddRange(rooms);

        return result;
    }

    private static InputWeaponTarget BuildDefault()
    {
        return new InputWeaponTarget
        {
            TargetType = WeaponTargetType.None,
            TargetTiles = new List<Vector2Int>(),
            TargetMechEntities = new List<EcsPackedEntity>(),
            TargetMechRooms = new List<EcsPackedEntity>(),
        };
    }
}
}