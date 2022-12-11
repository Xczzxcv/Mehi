using Ecs.Components;
using Ecs.Systems;

public class DeactivateWeaponSystem : EcsRunSystemBase<ActiveWeaponComponent>
{
    public DeactivateWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, int entity)
    {
        if (activeWeapon.CanBeDeactivated)
        {
            Pool.Del(entity);
        }
    }
}