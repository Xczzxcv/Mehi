using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;

namespace Ecs.Systems.Weapon
{
public class DelayUsageWeaponSystem : WeaponSystemBase<DelayUsageWeaponComponent>
{
    public DelayUsageWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon,
        ref DelayUsageWeaponComponent delayUsageComp, int entity)
    {
        switch (delayUsageComp.Stage)
        {
            case DelayUsageWeaponComponent.DelayStage.WeaponUsage:
                SetupDelay(activeWeapon, ref delayUsageComp, entity);
                break;
            case DelayUsageWeaponComponent.DelayStage.DelayProcessed:
                Reset(ref delayUsageComp);
                break;
        }
    }

    private void SetupDelay(ActiveWeaponComponent activeWeapon, ref DelayUsageWeaponComponent delayUsageComp, int entity)
    {
        delayUsageComp.ActiveWeaponComponent = activeWeapon;
        delayUsageComp.Stage = DelayUsageWeaponComponent.DelayStage.DelayStarted;
        delayUsageComp.StartDelayTurn = Services.BattleManager.TurnIndex;

        World.DelComponent<ActiveWeaponComponent>(entity);
    }

    private static void Reset(ref DelayUsageWeaponComponent delayUsageComp)
    {
        delayUsageComp.Stage = DelayUsageWeaponComponent.DelayStage.WeaponUsage;
    }
}
}