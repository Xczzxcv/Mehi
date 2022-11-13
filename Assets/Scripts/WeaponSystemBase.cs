using Ecs.Components;

namespace Ecs.Systems.Weapon
{
public abstract class WeaponSystemBase<TComponent> : 
    EcsRunSystemBase2<ActiveWeaponComponent, TComponent>, IWeaponSystem
    where TComponent : struct
{
    protected WeaponSystemBase(EnvironmentServices services) : base(services)
    { }
}
}