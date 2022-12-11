using Ecs.Components;
using Ecs.Systems;

public class StunEffectUpdateSystem : EcsRunSystemBase<StunEffectComponent>
{
    public StunEffectUpdateSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref StunEffectComponent stunEffect, int entity)
    {
        if (stunEffect.AlreadyUpdated)
        {
            return;
        }

        if (!Services.BattleManager.IsUnitTurnPhase(entity))
        {
            return;
        }

        stunEffect.Duration--;
        stunEffect.AlreadyUpdated = true;

        if (stunEffect.Duration <= 0)
        {
            Pool.Del(entity);
        }
    }
}