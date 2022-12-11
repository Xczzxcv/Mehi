using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

public struct WeaponTarget
{
    public WeaponTargetType TargetType;
    public List<EcsPackedEntity> TargetMechRooms;
    public List<EcsPackedEntity> TargetMechEntities;
    public List<Vector2Int> TargetTiles;

    public static WeaponTarget BuildEmpty()
    {
        return new WeaponTarget()
        {
            TargetType = WeaponTargetType.None,
            TargetMechRooms = new List<EcsPackedEntity>(),
            TargetMechEntities = new List<EcsPackedEntity>(),
            TargetTiles = new List<Vector2Int>(),
        };
    }
}
