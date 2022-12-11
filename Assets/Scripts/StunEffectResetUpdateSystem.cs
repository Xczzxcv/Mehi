using Ecs.Components;
using Ecs.Systems;

public class StunEffectResetUpdateSystem : EcsRunSystemBase<StunEffectComponent>
{
    public StunEffectResetUpdateSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref StunEffectComponent stunComp, int entity)
    {
        stunComp.AlreadyUpdated = false;
    }
}