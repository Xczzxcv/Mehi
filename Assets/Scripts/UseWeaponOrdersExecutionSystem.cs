using System.Collections.Generic;
using Ecs.Components;
using Ext.LeoEcs;
using Leopotam.EcsLite;
using UnityEngine;

namespace Ecs.Systems
{
public class UseWeaponOrdersExecutionSystem : EcsRunSystemBase2<UseWeaponOrderComponent, ActiveCreatureComponent>
{
    public UseWeaponOrdersExecutionSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref UseWeaponOrderComponent useWeaponOrder,
        ref ActiveCreatureComponent activeCreature, int entity)
    {
        // var mechsPool = World.GetPool<MechComponent>();
        // var mechs = World.Filter<MechComponent>().End();
        // foreach (var mechWeaponEntity in mechs)
        // {
        //     var mechComp = mechsPool.Get(mechWeaponEntity);
        //     mechComp.WeaponIds;
        // }

        if (!useWeaponOrder.WeaponEntity.TryUnpack(World, out var weaponEntity))
        {
            return;
        }

        ref var weaponActivation = ref World.AddComponent<ActiveWeaponComponent>(weaponEntity);
        weaponActivation.WeaponUser = World.PackEntity(entity);
        weaponActivation.WeaponTarget = GetCompleteWeaponTarget(useWeaponOrder.WeaponTarget);

        activeCreature.ActionPoints = 0;

        // *    оружие — сущность с компонентом WeaponComponent и возможно другими компонентами
        // (например PushComponent, DealDamageComponent).
        // *    для использования оружия, на сущность оружия добавляется компонент ActiveWeaponComponent,
        // в котором обозначается цель оружия
        // *    системы оружия написаны так, что они обрабатывают компоненты, только когда на оружии есть
        // этот компонент ActiveWeaponComponent. В этом случае они выполняют свою смысловую нагрузку над
    }

    private WeaponTarget GetCompleteWeaponTarget(in InputWeaponTarget target)
    {
        var resultWeaponTarget = new WeaponTarget
        {
            TargetType = target.TargetType,
            TargetMechRooms = new List<EcsPackedEntity>(target.TargetMechRooms),
            TargetMechEntities = new List<EcsPackedEntity>(target.TargetMechEntities),
            TargetTiles = new List<Vector2Int>(target.TargetTiles),
        };

        var positionPool = World.GetPool<PositionComponent>();
        ConvertTargetRooms(target, positionPool, ref resultWeaponTarget);

        var roomPool = World.GetPool<MechRoomComponent>();
        var rooms = World.Filter<MechRoomComponent>().End();
        ConvertTargetMechEntities(target, rooms, roomPool, ref resultWeaponTarget);

        ConvertTargetTiles(target, positionPool, rooms, roomPool, ref resultWeaponTarget);

        return resultWeaponTarget;
    }

    private void ConvertTargetRooms(InputWeaponTarget target, EcsPool<PositionComponent> positionPool,
        ref WeaponTarget resultWeaponTarget)
    {
        var mechRoomCompPool = World.GetPool<MechRoomComponent>();
        foreach (var targetMechRoomPacked in target.TargetMechRooms)
        {
            if (!targetMechRoomPacked.TryUnpack(World, out var targetMechRoomEntity))
            {
                continue;
            }

            var mechRoomComp = mechRoomCompPool.Get(targetMechRoomEntity);
            if (!mechRoomComp.MechEntity.TryUnpack(World, out var mechEntity))
            {
                continue;
            }

            resultWeaponTarget.TargetMechEntities.Add(mechRoomComp.MechEntity);

            var positionComp = positionPool.Get(mechEntity);
            resultWeaponTarget.TargetTiles.Add(positionComp.Pos);
        }
    }

    private void ConvertTargetMechEntities(InputWeaponTarget target, EcsFilter rooms,
        EcsPool<MechRoomComponent> roomPool, ref WeaponTarget resultWeaponTarget)
    {
        foreach (var targetMechEntityPacked in target.TargetMechEntities)
        {
            if (!targetMechEntityPacked.TryUnpack(World, out var targetMechEntity))
            {
                continue;
            }

            AddRandomMechRoom(rooms, roomPool, targetMechEntity, ref resultWeaponTarget);
        }
    }

    private void ConvertTargetTiles(InputWeaponTarget target, EcsPool<PositionComponent> positionPool, 
        EcsFilter rooms, EcsPool<MechRoomComponent> roomPool, ref WeaponTarget resultWeaponTarget)
    {
        var mechPosFilter = World.Filter<MechComponent>().Inc<PositionComponent>().End();
        foreach (var targetPos in target.TargetTiles)
        {
            int targetMechEntity = default;
            foreach (var mechEntity in mechPosFilter)
            {
                var mechPosComp = positionPool.Get(mechEntity);
                if (mechPosComp.Pos == targetPos)
                {
                    targetMechEntity = mechEntity;
                    break;
                }
            }

            var noMechOnTargetPos = targetMechEntity == default;
            if (noMechOnTargetPos)
            {
                continue;
            }

            AddRandomMechRoom(rooms, roomPool, targetMechEntity, ref resultWeaponTarget);
        }
    }

    private static readonly List<int> TargetMechRoomEntities = new();
    private void AddRandomMechRoom(EcsFilter rooms, EcsPool<MechRoomComponent> roomPool, 
        int targetMechEntity, ref WeaponTarget weaponTarget)
    {
        TargetMechRoomEntities.Clear();
        foreach (var roomEntity in rooms)
        {
            var roomComp = roomPool.Get(roomEntity);
            if (!roomComp.MechEntity.TryUnpack(World, out var roomMechEntity))
            {
                continue;
            }

            if (targetMechEntity == roomMechEntity)
            {
                TargetMechRoomEntities.Add(roomEntity);
            }
        }

        var randomRoomIndex = Random.Range(0, TargetMechRoomEntities.Count);
        var randomRoomEntity = TargetMechRoomEntities[randomRoomIndex];
        var randomRoomEntityPacked = World.PackEntity(randomRoomEntity);
        weaponTarget.TargetMechRooms.Add(randomRoomEntityPacked);
    }

    public static bool TryGetWeaponEntity(string weaponId, EcsWorld world, 
        out EcsPackedEntity weaponEntPacked)
    {
        var weapons = world.Filter<WeaponComponent>().End();
        var weaponsPool = world.GetPool<WeaponComponent>();
        foreach (var weaponEntity in weapons)
        {
            var weapon = weaponsPool.Get(weaponEntity);
            if (weapon.WeaponId == weaponId)
            {
                weaponEntPacked = world.PackEntity(weaponEntity);
                return true;
            }
        }

        weaponEntPacked = default;
        return false;
    }
}
}

namespace Ecs.Components
{
public struct WeaponComponent
{
    public string WeaponId;
}

public struct ActiveWeaponComponent
{
    public EcsPackedEntity WeaponUser;
    public WeaponTarget WeaponTarget;
}

public struct InputWeaponTarget
{
    public WeaponTargetType TargetType;
    public List<EcsPackedEntity> TargetMechRooms;
    public List<EcsPackedEntity> TargetMechEntities;
    public List<Vector2Int> TargetTiles;
}

public struct WeaponTarget
{
    public WeaponTargetType TargetType;
    public List<EcsPackedEntity> TargetMechRooms;
    public List<EcsPackedEntity> TargetMechEntities;
    public List<Vector2Int> TargetTiles;
}

public struct UseWeaponOrderComponent
{
    public EcsPackedEntity WeaponEntity;
    public InputWeaponTarget WeaponTarget;
}
}