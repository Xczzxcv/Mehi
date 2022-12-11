using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;

namespace Ecs.Systems.Weapon
{
public class StunWeaponSystem : WeaponSystemBase<StunWeaponComponent>
{
    public StunWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref StunWeaponComponent stunWeapon, int entity)
    {
        var stunPool = World.GetPool<StunEffectComponent>();
        foreach (var targetMechPacked in activeWeapon.WeaponTarget.TargetMechEntities)
        {
            if (!targetMechPacked.TryUnpack(World, out var targetMechEntity))
            {
                continue;
            }

            ref var stunEffectComp = ref stunPool.GetOrAdd(targetMechEntity);
            stunEffectComp.Duration += stunWeapon.StunDuration;
        }
    }
}
}