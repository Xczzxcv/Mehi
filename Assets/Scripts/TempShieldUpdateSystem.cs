using Ecs.Components;

namespace Ecs.Systems
{
public class TempShieldUpdateSystem : EcsRunSystemBase<TempShieldComponent>
{
    public const int TEMP_SHIELD_LIFETIME = 1;
    
    public TempShieldUpdateSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref TempShieldComponent tempShield, int entity)
    {
        var removeShieldTurnIndex = tempShield.InitTurnIndex + TEMP_SHIELD_LIFETIME;
        if (Services.BattleManager.TurnIndex != removeShieldTurnIndex)
        {
            return;
        }

        if (!Services.BattleManager.IsUnitTurnPhase(entity))
        {
            return;
        }

        Pool.Del(entity);
    }
}
}