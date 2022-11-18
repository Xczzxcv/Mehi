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
        if (mechDamageEvent.DamageTargetRoom.TryUnpack(World, out var mechRoomEntity))
        {
            ref var roomHpComp = ref World.GetComponent<HealthComponent>(mechRoomEntity);
            roomHpComp.Health = Math.Max(roomHpComp.Health - mechDamageEvent.DamageAmount, 0);
        }
        else
        {
            Debug.LogError("Has no room already");
        }
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