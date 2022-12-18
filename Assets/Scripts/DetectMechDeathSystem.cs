using Ecs.Components;
using Ext.LeoEcs;

namespace Ecs.Systems
{
public class DetectMechDeathSystem : EcsRunSystemBase<MechHealthComponent>
{
    public DetectMechDeathSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MechHealthComponent mechHealth, int entity)
    {
        var isAlive = mechHealth.Health > 0;
        if (isAlive)
        {
            return;
        }

        Services.BattleManager.ProcessMechDeath(entity);
    }
}
}