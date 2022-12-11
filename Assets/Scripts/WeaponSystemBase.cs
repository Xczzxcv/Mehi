using Ecs.Components;
using Ecs.Components.Weapon;

namespace Ecs.Systems.Weapon
{
public abstract class WeaponSystemBase<TComponent> :
    EcsRunSystemBase2<ActiveWeaponComponent, TComponent>, IWeaponSystem
    where TComponent : struct, IWeaponComponent
{
    protected WeaponSystemBase(EnvironmentServices services) : base(services)
    { }
}
}