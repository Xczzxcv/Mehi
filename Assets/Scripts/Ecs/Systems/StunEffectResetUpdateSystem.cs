using Ecs.Components;
using Ecs.Systems;
using Ext.LeoEcs;

public class StunEffectResetUpdateSystem : EcsRunSystemBase<StunEffectComponent>
{
    public StunEffectResetUpdateSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref StunEffectComponent stunComp, int entity)
    {
        stunComp.AlreadyUpdated = false;
    }
}