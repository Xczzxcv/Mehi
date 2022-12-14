using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;


namespace Ecs.Systems.Weapon
{
public abstract class WeaponRequirementSystemBase<TRequirementComponent> : 
    EcsRunSystemBase3<ActiveWeaponComponent, WeaponMainComponent, TRequirementComponent>, IWeaponRequirementSystem 
    where TRequirementComponent : struct, IWeaponRequirementComponent
{
    protected WeaponRequirementSystemBase(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref WeaponMainComponent weaponMainComp, ref TRequirementComponent requirementComp, int entity)
    {
        if (!CheckRequirement(in activeWeapon, in weaponMainComp, ref requirementComp, entity))
        {
            World.GetPool<ActiveWeaponComponent>().Del(entity);
        }
    }

    protected abstract bool CheckRequirement(in ActiveWeaponComponent activeWeapon,
        in WeaponMainComponent weaponMainComp, ref TRequirementComponent requirementComp, int entity);
}
}