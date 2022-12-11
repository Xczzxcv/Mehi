using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;

namespace Ecs.Systems.Weapon
{
public class AddShieldToSelfWeaponSystem : WeaponSystemBase<AddShieldToSelfWeaponComponent>
{
    public AddShieldToSelfWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref AddShieldToSelfWeaponComponent addShieldComp, int entity)
    {
        if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUser))
        {
            return;
        }

        ref var tempShieldComponent = ref World.GetOrAddComponent<TempShieldComponent>(weaponUser);
        tempShieldComponent.Amount += addShieldComp.ShieldAmount;
        if (tempShieldComponent.InitTurnIndex == TempShieldComponent.DEFAULT_TURN_INDEX)
        {
            tempShieldComponent.InitTurnIndex = Services.BattleManager.TurnIndex;
        }
    }
}
}