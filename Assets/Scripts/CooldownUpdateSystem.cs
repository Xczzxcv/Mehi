using Ecs.Components;
using Ext.LeoEcs;

internal class CooldownUpdateSystem : EcsRunSystemBase<CooldownComponent>
{
    public CooldownUpdateSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref CooldownComponent cdComponent, int entity)
    {
        if (Services.BattleManager.TurnIndex >= cdComponent.LastsUntilTurn)
        {
            Pool.Del(entity);
        }
    }
}