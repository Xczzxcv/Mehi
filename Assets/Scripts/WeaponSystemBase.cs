using Ecs.Components;
using Ecs.Systems;

public abstract class WeaponSystemBase<TComponent> : EcsRunSystemBase2<ActiveWeaponComponent, TComponent> 
    where TComponent : struct
{
    protected WeaponSystemBase(EnvironmentServices services) : base(services)
    { }
}