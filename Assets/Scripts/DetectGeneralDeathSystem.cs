using Ecs.Components;
using Ext.LeoEcs;

public class DetectGeneralDeathSystem : EcsRunSystemBase<HealthComponent>
{
    public DetectGeneralDeathSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref HealthComponent healthComp, int entity)
    {
        var isAlive = healthComp.Health > 0;
        if (isAlive)
        {
            return;
        }

        if (healthComp.StayAfterDeath)
        {
            return;
        }

        Services.BattleManager.ProcessGeneralDeath(entity);
    }
}