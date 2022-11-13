using Ecs.Components;
using Ecs.Components.Weapon;

namespace Ecs.Systems.Weapon
{
public class StunWeaponSystem : WeaponSystemBase<StunWeaponComponent>
{
    public StunWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref StunWeaponComponent stunWeapon, int entity)
    {
        throw new System.NotImplementedException();
    }
}
}