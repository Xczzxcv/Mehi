using System.Collections.Generic;
using System.Linq;
using Ecs.Components;
using Ext.LeoEcs;
using Leopotam.EcsLite;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ecs.Systems
{
public class UseWeaponOrdersExecutionSystem : EcsRunSystemBase2<UseWeaponOrderComponent, ActiveCreatureComponent>
{
    public const int USE_WEAPON_ACTION_COST = 1;
    
    public UseWeaponOrdersExecutionSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref UseWeaponOrderComponent useWeaponOrder,
        ref ActiveCreatureComponent activeCreature, int entity)
    {
        if (!useWeaponOrder.WeaponEntity.TryUnpack(World, out var weaponEntity))
        {
            return;
        }

        ref var weaponActivation = ref World.AddComponent<ActiveWeaponComponent>(weaponEntity);
        weaponActivation.WeaponUser = World.PackEntity(entity);
        weaponActivation.WeaponTarget = GetCompleteWeaponTarget(useWeaponOrder.WeaponTarget, 
            weaponEntity, entity);

        activeCreature.ActionPoints = 0;

        // *    оружие — сущность с компонентом WeaponMainComponent и возможно другими компонентами
        // (например PushComponent, DealDamageComponent).
        // *    для использования оружия, на сущность оружия добавляется компонент ActiveWeaponComponent,
        // в котором обозначается цель оружия
        // *    системы оружия написаны так, что они обрабатывают компоненты, только когда на оружии есть
        // этот компонент ActiveWeaponComponent. В этом случае они выполняют свою смысловую нагрузку над
    }

    private WeaponTarget GetCompleteWeaponTarget(in InputWeaponTarget target, int weaponEntity, 
        int weaponUserEntity)
    {
        var resultWeaponTarget = new WeaponTarget
        {
            TargetType = target.TargetType,
            TargetMechRooms = new List<EcsPackedEntity>(World.PackEntities(target.TargetMechRoomEntities)),
            TargetMechEntities = new List<EcsPackedEntity>(World.PackEntities(target.TargetMechEntities)),
            TargetTiles = new List<Vector2Int>(target.TargetTiles),
        };

        var roomPool = World.GetPool<MechRoomComponent>();
        var rooms = World.Filter<MechRoomComponent>().End();
        var mainWeaponComp = World.GetComponent<WeaponMainComponent>(weaponEntity);
        var weaponUserControl = BattleMechManager.GetUnitControl(weaponUserEntity, World);
        switch (target.TargetType)
        {
            case WeaponTargetType.Rooms:
                var positionPool = World.GetPool<PositionComponent>();
                ConvertTargetRooms(target, positionPool, mainWeaponComp, weaponUserControl, ref resultWeaponTarget);
                break;
            case WeaponTargetType.Unit:
                ConvertTargetMechEntities(target, rooms, roomPool, mainWeaponComp, weaponUserControl, ref resultWeaponTarget);
                break;
            case WeaponTargetType.BattleFieldTiles:
                ConvertTargetTiles(target, rooms, roomPool, mainWeaponComp, weaponUserControl, ref resultWeaponTarget);
                break;
        }
        
        return resultWeaponTarget;
    }

    private void ConvertTargetRooms(InputWeaponTarget target, EcsPool<PositionComponent> positionPool,
        WeaponMainComponent weaponMainComp, BattleMechManager.UnitControl weaponUserControl, 
        ref WeaponTarget resultWeaponTarget)
    {
        var targetMechRoomEntities = GetTargetMechRoomEntities(target, 
            weaponMainComp, weaponUserControl);
        var mechRoomCompPool = World.GetPool<MechRoomComponent>();
        foreach (var targetMechRoomEntity in targetMechRoomEntities)
        {
            ref var mechRoomComp = ref mechRoomCompPool.Get(targetMechRoomEntity);
            if (!mechRoomComp.MechEntity.TryUnpack(World, out var mechEntity))
            {
                continue;
            }

            resultWeaponTarget.TargetMechEntities.Add(mechRoomComp.MechEntity);

            ref var positionComp = ref positionPool.Get(mechEntity);
            resultWeaponTarget.TargetTiles.Add(positionComp.Pos);
        }
    }

    private IEnumerable<int> GetTargetMechRoomEntities(InputWeaponTarget target, WeaponMainComponent weaponMainComp,
        BattleMechManager.UnitControl weaponUserControl)
    {
        var targetMechRoomEntities = target.TargetMechRoomEntities;
        return weaponMainComp.IsFriendlyFireEnabled
            ? targetMechRoomEntities
            : targetMechRoomEntities.Where(FilterAllyRooms);

        bool FilterAllyRooms(int mechRoomEntity)
        {
            var mechRoomComp = World.GetComponent<MechRoomComponent>(mechRoomEntity);
            if (!mechRoomComp.MechEntity.TryUnpack(World, out var mechEntity))
            {
                return false;
            }

            return CanAttackMechEntity(weaponUserControl, mechEntity);
        }
    }

    private bool CanAttackMechEntity(BattleMechManager.UnitControl weaponUserControl, int mechEntity)
    {
        var targetMechControl = BattleMechManager.GetUnitControl(mechEntity, World);
        return BattleMechManager.CanAttack(weaponUserControl, targetMechControl);
    }


    private void ConvertTargetMechEntities(InputWeaponTarget target, EcsFilter rooms,
        EcsPool<MechRoomComponent> roomPool, WeaponMainComponent weaponMainComp, 
        BattleMechManager.UnitControl weaponUserControl, ref WeaponTarget resultWeaponTarget)
    {
        var targetMechEntities = GetTargetMechEntities(target, weaponMainComp, 
            weaponUserControl);
        foreach (var targetMechEntity in targetMechEntities)
        {
            AddRandomMechRoom(rooms, roomPool, targetMechEntity, ref resultWeaponTarget);
            
            var unitPosition = Services.BattleManager.GetUnitPosition(targetMechEntity);
            resultWeaponTarget.TargetTiles.Add(unitPosition);
        }
    }

    private IEnumerable<int> GetTargetMechEntities(InputWeaponTarget target, WeaponMainComponent weaponMainComp,
        BattleMechManager.UnitControl weaponUserControl)
    {
        var targetMechEntities = weaponMainComp.IsFriendlyFireEnabled
            ? target.TargetMechEntities
            : target.TargetMechEntities.Where(mechEntity => CanAttackMechEntity(weaponUserControl, mechEntity));
        return targetMechEntities;
    }

    private void ConvertTargetTiles(InputWeaponTarget target, EcsFilter rooms, 
        EcsPool<MechRoomComponent> roomPool, WeaponMainComponent weaponMainComp,
        BattleMechManager.UnitControl weaponUserControl, ref WeaponTarget resultWeaponTarget)
    {
        var targetTiles = GetTargetTiles(target, weaponMainComp, weaponUserControl);
        foreach (var targetPos in targetTiles)
        {
            if (!Services.BattleManager.TryGetUnitInPos(targetPos, out var targetMechEntity))
            {
                continue;
            }

            resultWeaponTarget.TargetMechEntities.Add(World.PackEntity(targetMechEntity));

            AddRandomMechRoom(rooms, roomPool, targetMechEntity, ref resultWeaponTarget);
        }
    }

    private IEnumerable<Vector2Int> GetTargetTiles(InputWeaponTarget target, WeaponMainComponent weaponMainComp,
        BattleMechManager.UnitControl weaponUserControl)
    {
        var targetTiles = weaponMainComp.IsFriendlyFireEnabled
            ? target.TargetTiles
            : target.TargetTiles.Where(CanAttackTile);
        return targetTiles;

        bool CanAttackTile(Vector2Int tilePos)
        {
            if (!Services.BattleManager.TryGetUnitInPos(tilePos, out var mechEntity))
            {
                return true;
            }

            return CanAttackMechEntity(weaponUserControl, mechEntity);
        }
    }

    private static readonly List<int> TargetMechRoomEntities = new();
    private void AddRandomMechRoom(EcsFilter rooms, EcsPool<MechRoomComponent> roomPool, 
        int targetMechEntity, ref WeaponTarget weaponTarget)
    {
        TargetMechRoomEntities.Clear();
        foreach (var roomEntity in rooms)
        {
            ref var roomComp = ref roomPool.Get(roomEntity);
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

    public static bool TryGetWeaponEntity(string weaponId, int unitEntity, EcsWorld world,
        out int resultWeaponEntity)
    {
        var weapons = world.Filter<WeaponMainComponent>().End();
        var weaponsPool = world.GetPool<WeaponMainComponent>();
        foreach (var weaponEntity in weapons)
        {
            ref var weapon = ref weaponsPool.Get(weaponEntity);
            if (weapon.WeaponId != weaponId)
            {
                continue;
            }

            var checkOwner = weapon.OwnerUnitEntity.Unpack(world, out var ownerEntity)
                    && unitEntity == ownerEntity;
            if (!checkOwner)
            {
                continue;
            }

            resultWeaponEntity = weaponEntity;
            return true;
        }

        resultWeaponEntity = default;
        return false;
    }
}
}