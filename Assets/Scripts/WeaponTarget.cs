using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;

namespace Ecs.Components
{
public struct WeaponTarget
{
    public WeaponTargetType TargetType;
    public List<EcsPackedEntity> TargetMechRooms;
    public List<EcsPackedEntity> TargetMechEntities;
    public List<Vector2Int> TargetTiles;
}
}