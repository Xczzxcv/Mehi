using Ecs.Components;
using Ext.LeoEcs;

namespace Ecs.Systems
{
public class MechRoomBurningDamageApplySystem : EcsRunSystemBase3<MechRoomComponent, HealthComponent, 
    BurningEffectComponent>
{
    public const int BURNING_DAMAGE = 1;
    
    public MechRoomBurningDamageApplySystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MechRoomComponent roomComp, ref HealthComponent hpComp, 
        ref BurningEffectComponent burningEffectComp, int entity)
    {
        var damageEvent = MechDamageEvent.BuildFromRoom(burningEffectComp.EffectSource, entity, 
            BURNING_DAMAGE, World);
        DamageApplySystem.TryAddDamageEvent(damageEvent, roomComp.MechEntity, World);
    }
}
}