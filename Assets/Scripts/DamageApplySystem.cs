using System;
using Ecs.Components;
using Ecs.Systems;
using Ext.LeoEcs;
using Leopotam.EcsLite;
using UnityEngine;

public class DamageApplySystem : EcsRunSystemBase2<MechHealthComponent, MechDamageApplyComponent>
{
    public DamageApplySystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MechHealthComponent mechHealthComp, 
        ref MechDamageApplyComponent mechDmgApplyComp, int entity)
    {
        foreach (var mechDamageEvent in mechDmgApplyComp.Events)
        {
            ProcessDamageEvent(mechDamageEvent, ref mechHealthComp, entity);
        }
        
        mechDmgApplyComp.Events.Clear();
    }

    private void ProcessDamageEvent(MechDamageEvent mechDamageEvent, 
        ref MechHealthComponent mechHealthComp, int entity)
    {
        RoomDamageApply(mechDamageEvent);
        MechDamageApply(mechDamageEvent, ref mechHealthComp);
    }

    private void MechDamageApply(MechDamageEvent mechDmgEvent, ref MechHealthComponent mechHpComp)
    {
        var dmgShieldAbsorb = Math.Min(mechDmgEvent.DamageAmount, mechHpComp.Shield);
        mechHpComp.Shield -= dmgShieldAbsorb;
        mechDmgEvent.DamageAmount -= dmgShieldAbsorb;

        mechHpComp.Health -= mechDmgEvent.DamageAmount;
        mechDmgEvent.DamageAmount = 0;
    }

    private void RoomDamageApply(MechDamageEvent mechDamageEvent)
    {
        HealthComponent roomHpComp = default;
        if (TryGetRoomHealth(mechDamageEvent.DamageTargetRoom, ref roomHpComp))
        {
            roomHpComp.Health = Math.Max(roomHpComp.Health - mechDamageEvent.DamageAmount, 0);
        }
        else
        {
            Debug.LogError("Has no room already");
        }
    }

    private bool TryGetRoomHealth(EcsPackedEntity damageTargetRoom, ref HealthComponent healthComp)
    {
        if (!damageTargetRoom.TryUnpack(World, out var mechRoomEntity))
        {
            return false;
        }

        healthComp = World.GetComponent<HealthComponent>(mechRoomEntity);
        return true;
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

        var dmgApplyComp = dmgApplyComponentsPool.Get(mechEntity);
        dmgApplyComp.Events.Add(damageEvent);
        return true;
    }
}