using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;

namespace Ecs.Systems
{
public class DelayUsageWeaponUpdateSystem : EcsRunSystemBase<DelayUsageWeaponComponent>
{
    public DelayUsageWeaponUpdateSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref DelayUsageWeaponComponent delayUsageComp, int entity)
    {
        if (delayUsageComp.Stage != DelayUsageWeaponComponent.DelayStage.DelayStarted)
        {
            return;
        }

        ProcessDelay(ref delayUsageComp, entity);
    }
    
    private void ProcessDelay(ref DelayUsageWeaponComponent delayUsageComp, int entity)
    {
        var elapsedTurns = Services.BattleManager.TurnIndex - delayUsageComp.StartDelayTurn;
        if (elapsedTurns < delayUsageComp.DelayAmount)
        {
            return;
        }

        var oldActiveWeapon = delayUsageComp.ActiveWeaponComponent;
        if (!oldActiveWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
        {
            return;
        }

        if (!Services.BattleManager.IsUnitTurnPhase(weaponUserEntity))
        {
            return;
        }

        ref var newActiveWeapon = ref World.AddComponent<ActiveWeaponComponent>(entity);
        newActiveWeapon = oldActiveWeapon;

        delayUsageComp.Stage = DelayUsageWeaponComponent.DelayStage.DelayProcessed;
    }
}
}