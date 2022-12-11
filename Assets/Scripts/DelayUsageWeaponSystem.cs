using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;
using Leopotam.EcsLite;

namespace Ecs.Systems.Weapon
{
public class DelayUsageWeaponSystem : WeaponSystemBase<DelayUsageWeaponComponent>
{
    public DelayUsageWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon,
        ref DelayUsageWeaponComponent delayUsageComp, int entity)
    {
        if (delayUsageComp.DelayStarted)
        {
            ProcessDelay(ref activeWeapon, ref delayUsageComp, entity);
        }
        else
        {
            delayUsageComp.ActiveWeaponComponent = activeWeapon;
            delayUsageComp.DelayStarted = true;
            delayUsageComp.StartDelayTurn = Services.BattleManager.TurnIndex;

            activeWeapon.CanBeDeactivated = false;
            activeWeapon.WeaponTarget = WeaponTarget.BuildEmpty();
        }
    }

    private void ProcessDelay(ref ActiveWeaponComponent activeWeapon, 
        ref DelayUsageWeaponComponent delayUsageComp, int entity)
    {
        var elapsedTurns = Services.BattleManager.TurnIndex - delayUsageComp.StartDelayTurn;
        if (elapsedTurns < delayUsageComp.DelayAmount)
        {
            return;
        }
        
        
        if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
        {
            return;
        }

        if (!Services.BattleManager.IsUnitTurnPhase(weaponUserEntity))
        {
            return;
        }

        activeWeapon = delayUsageComp.ActiveWeaponComponent;
        activeWeapon.CanBeDeactivated = true;
    }
}
}