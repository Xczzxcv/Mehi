using System;
using Ecs.Components;
using Ext.LeoEcs;
using Leopotam.EcsLite;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ecs.Systems
{
public class DamageApplySystem : EcsRunSystemBase2<MechHealthComponent, MechDamageApplyComponent>
{
    public DamageApplySystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MechHealthComponent mechHealthComp, 
        ref MechDamageApplyComponent mechDmgApplyComp, int entity)
    {
        var dmgEventInd = 0;
        for (; dmgEventInd < mechDmgApplyComp.Events.Count; dmgEventInd++)
        {
            var mechDamageEvent = mechDmgApplyComp.Events[dmgEventInd];
            ProcessDamageEvent(mechDamageEvent, ref mechHealthComp, entity);
        }
        mechDmgApplyComp.Events.Clear();

        var someEventsWereProcessed = dmgEventInd > 0;
        if (someEventsWereProcessed)
        {
            GlobalEventManager.BattleField.UnitUpdated.HappenedWith(entity);
        }
    }

    private void ProcessDamageEvent(MechDamageEvent mechDamageEvent, 
        ref MechHealthComponent mechHealthComp, int entity)
    {
        var isHit = Random.value < mechDamageEvent.HitChance;
        if (!isHit)
        {
            return;
        }
        
        ModifyDamage(ref mechDamageEvent);
        
        RoomDamageApply(mechDamageEvent);
        UpdateRoomSystem(mechDamageEvent);
        MechDamageApply(mechDamageEvent, ref mechHealthComp, entity);
    }

    private void ModifyDamage(ref MechDamageEvent mechDamageEvent)
    {
        if (mechDamageEvent.DamageTargetRoom.TryUnpack(World, out var mechRoomEntity))
        {
            ref var roomHpComp = ref World.GetComponent<HealthComponent>(mechRoomEntity);
            if (roomHpComp.Health <= 0)
            {
                mechDamageEvent.DamageAmount *= 2;
            }
        }
        else
        {
            mechDamageEvent.DamageAmount = 0;
        }
    }

    private void RoomDamageApply(MechDamageEvent mechDamageEvent)
    {
        if (!mechDamageEvent.DamageTargetRoom.TryUnpack(World, out var mechRoomEntity))
        {
            Debug.LogError("Has no room already");
            return;
        }

        ref var roomHealthComp = ref World.GetComponent<HealthComponent>(mechRoomEntity);
        roomHealthComp.Health = Math.Max(roomHealthComp.Health - mechDamageEvent.DamageAmount, 0);
    }

    private void UpdateRoomSystem(MechDamageEvent mechDamageEvent)
    {
        if (!mechDamageEvent.DamageTargetRoom.TryUnpack(World, out var mechRoomEntity))
        {
            Debug.LogError("Has no room already");
            return;
        }

        ref var roomMechComp = ref World.GetComponent<MechRoomComponent>(mechRoomEntity);
        if (!roomMechComp.MechEntity.TryUnpack(World, out var mechEntity))
        {
            Debug.LogError("Has no mech entity already");
            return;
        }

        ref var mechSystem = ref BattleMechManager.TryGetFirstMechSystemOfType(mechEntity,
            roomMechComp.SystemType, World, out var result);
        if (!result)
        {
            Debug.LogError($"Mech {mechEntity} has no such system as {roomMechComp.SystemType}");
            return;
        }
        
        ref var roomHealthComp = ref World.GetComponent<HealthComponent>(mechRoomEntity);
        mechSystem.IsActive = roomHealthComp.Health > 0;
    }

    private void MechDamageApply(MechDamageEvent mechDmgEvent, ref MechHealthComponent mechHpComp,
        int mechEntity)
    {
        ProcessTempShield(ref mechDmgEvent, mechEntity);

        var dmgShieldAbsorb = Math.Min(mechDmgEvent.DamageAmount, mechHpComp.Shield);
        mechHpComp.Shield -= dmgShieldAbsorb;
        mechDmgEvent.DamageAmount -= dmgShieldAbsorb;

        mechHpComp.Health -= mechDmgEvent.DamageAmount;
        mechDmgEvent.DamageAmount = 0;
    }

    private void ProcessTempShield(ref MechDamageEvent mechDmgEvent, int mechEntity)
    {
        var tempShieldPool = World.GetPool<TempShieldComponent>();
        if (!tempShieldPool.Has(mechEntity))
        {
            return;
        }

        ref var tempShield = ref tempShieldPool.Get(mechEntity);
        var dmgTempShieldAbsorb = Math.Min(mechDmgEvent.DamageAmount, tempShield.Amount);
        var dmgTempShieldPierce = mechDmgEvent.DamageAmount - dmgTempShieldAbsorb;

        mechDmgEvent.DamageAmount -= dmgTempShieldAbsorb;
        tempShield.Amount -= dmgTempShieldPierce;
    }

    public static bool TryAddDamageEvent(MechDamageEvent damageEvent, EcsPackedEntity mechEntityPacked,
        EcsWorld world)
    {
        if (!mechEntityPacked.TryUnpack(world, out var mechEntity))
        {
            return false;
        }
        
        var dmgApplyComponentsPool = world.GetPool<MechDamageApplyComponent>();
        if (!dmgApplyComponentsPool.Has(mechEntity))
        {
            return false;
        }

        ref var dmgApplyComp = ref dmgApplyComponentsPool.Get(mechEntity);
        dmgApplyComp.Events.Add(damageEvent);
        return true;
    }
}
}