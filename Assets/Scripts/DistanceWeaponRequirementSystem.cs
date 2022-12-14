using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;
using UnityEngine;

namespace Ecs.Components.Weapon
{
public struct DistanceWeaponRequirementComponent : IWeaponRequirementComponent
{ }
}

namespace Ecs.Systems.Weapon
{
public class DistanceWeaponRequirementSystem : WeaponRequirementSystemBase<DistanceWeaponRequirementComponent>
{
    public DistanceWeaponRequirementSystem(EnvironmentServices services) : base(services)
    { }

    protected override bool CheckRequirement(in ActiveWeaponComponent activeWeapon,
        in WeaponMainComponent weaponMainComp, ref DistanceWeaponRequirementComponent requirementComp, int entity)
    {
        if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
        {
            return false;
        }

        foreach (var targetMechPacked in activeWeapon.WeaponTarget.TargetMechEntities)
        {
            if (!targetMechPacked.TryUnpack(World, out var targetMechEntity))
            {
                return false;
            }

            var targetMechPos = Services.BattleManager.GetUnitPosition(targetMechEntity);
            if (!CheckWeaponDistance(Services.BattleManager, weaponUserEntity, 
                    weaponMainComp.UseDistance, targetMechPos))
            {
                return false;
            }
        }

        return true;
    }

    public static bool CheckWeaponDistance(BattleManager battleManager, int weaponUserEntity, 
        int weaponUseDistance, Vector2Int targetPos)
    {
        var attackerInfo = battleManager.GetBattleUnitInfo(weaponUserEntity);
        var distanceToTarget = Vector2Int.Distance(attackerInfo.Position, targetPos);
        return distanceToTarget <= weaponUseDistance;
    }
}
}