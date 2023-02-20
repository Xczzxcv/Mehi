using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;

namespace Ecs.Systems.Weapon
{
internal class SetupCooldownWeaponSystem : WeaponSystemBase<CooldownWeaponComponent>
{
    public SetupCooldownWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon,
        ref CooldownWeaponComponent cdWeaponComponent, int entity)
    {
        ref var cdComponent = ref World.AddComponent<CooldownComponent>(entity);
        cdComponent.LastsUntilTurn = Services.BattleManager.TurnIndex + cdWeaponComponent.Cooldown + 1;
    }
}
}