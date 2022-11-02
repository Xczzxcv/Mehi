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
        HealthComponent healthComp = default;
        if (!TryGetRoomHealth(mechDamageEvent.DamageTargetRoom, ref healthComp))
        {
            Debug.LogError("Has no room already");
            return;
        }

        var dmgShieldAbsorb = Math.Min(mechDamageEvent.DamageAmount, mechHealthComp.Shield);
        mechHealthComp.Shield -= dmgShieldAbsorb;
        mechDamageEvent.DamageAmount -= dmgShieldAbsorb;

        healthComp.Health -= mechDamageEvent.DamageAmount;
        mechDamageEvent.DamageAmount = 0;
    }

    private bool TryGetRoomHealth(EcsPackedEntity damageTargetRoom, ref HealthComponent healthComp)
    {
        if (!damageTargetRoom.Unpack(World, out var mechRoomEntity))
        {
            return false;
        }

        healthComp = World.GetComponent<HealthComponent>(mechRoomEntity);
        return true;
    }
}