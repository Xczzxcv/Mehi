using Ecs.Components;

namespace Ecs.Systems
{
public class MechRoomBurningDamageApplySystem : EcsRunSystemBase3<MechRoomComponent, HealthComponent, 
    BurningComponent>
{
    public const int BURNING_DAMAGE = 1;
    
    public MechRoomBurningDamageApplySystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MechRoomComponent roomComp, ref HealthComponent hpComp, 
        ref BurningComponent burningComp, int entity)
    {
        var damageEvent = MechDamageEvent.BuildFromRoom(burningComp.BurningSrc, entity, 
            BURNING_DAMAGE, World);
        DamageApplySystem.TryAddDamageEvent(damageEvent, roomComp.MechEntity, World);
    }
}
}