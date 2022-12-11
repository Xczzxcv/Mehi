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
        var isHit = mechDamageEvent.HitChance <= Random.value;
        if (!isHit)
        {
            return;
        }
        
        RoomDamageApply(mechDamageEvent);
        MechDamageApply(mechDamageEvent, ref mechHealthComp, entity);
    }

    private void MechDamageApply(MechDamageEvent mechDmgEvent, ref MechHealthComponent mechHpComp,
        int mechEntity)
    {
        var dmgTempShieldPierce = ProcessTempShield(mechDmgEvent, mechEntity);

        var dmgShieldAbsorb = Math.Min(dmgTempShieldPierce, mechHpComp.Shield);
        mechHpComp.Shield -= dmgShieldAbsorb;
        mechDmgEvent.DamageAmount -= dmgShieldAbsorb;

        mechHpComp.Health -= mechDmgEvent.DamageAmount;
        mechDmgEvent.DamageAmount = 0;
    }

    private int ProcessTempShield(MechDamageEvent mechDmgEvent, int mechEntity)
    {
        var tempShieldPool = World.GetPool<TempShieldComponent>();
        int dmgTempShieldPierce;
        if (tempShieldPool.Has(mechEntity))
        {
            ref var tempShield = ref tempShieldPool.Get(mechEntity);
            var dmgTempShieldAbsorb = Math.Min(mechDmgEvent.DamageAmount, tempShield.Amount);
            dmgTempShieldPierce = mechDmgEvent.DamageAmount - dmgTempShieldAbsorb;

            tempShield.Amount -= dmgTempShieldPierce;
        }
        else
        {
            dmgTempShieldPierce = 0;
        }

        return dmgTempShieldPierce;
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
}