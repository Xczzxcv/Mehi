using Leopotam.EcsLite;

namespace Ecs.Systems
{
public class TurnUpdateSystem : EcsRunSystemBase
{
    public TurnUpdateSystem(EnvironmentServices services) : base(services)
    { }

    public override void Run(IEcsSystems systems)
    {
        Services.BattleManager.NextTurn();
    }
}
}