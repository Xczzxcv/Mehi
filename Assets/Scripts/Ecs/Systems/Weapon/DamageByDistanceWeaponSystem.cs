using System;
using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;
using UnityEngine;

namespace Ecs.Systems.Weapon
{
public class DamageByDistanceWeaponSystem : WeaponSystemBase<DamageByDistanceComponent>
{
    public DamageByDistanceWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref DamageByDistanceComponent dmgByDistance, int entity)
    {
        var weaponTarget = activeWeapon.WeaponTarget;
        var targetRooms = weaponTarget.TargetMechRooms;
        foreach (var targetRoomEntityPacked in targetRooms)
        {
            if (!DamageWeaponSystem.TryGetRoomComp(targetRoomEntityPacked, World, 
                    out var targetRoomEntity, out var targetRoom))
            {
                continue;
            }

            if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
            {
                continue;
            }

            if (!targetRoom.MechEntity.TryUnpack(World, out var targetUnitEntity))
            {
                continue;
            }

            var weaponUserPos = Services.BattleManager.GetUnitPosition(weaponUserEntity);
            var targetUnitPos = Services.BattleManager.GetUnitPosition(targetUnitEntity);
            var distanceToTarget = Mathf.FloorToInt(MathF.Round(Vector2Int.Distance(weaponUserPos, targetUnitPos), 3));
            
            var currentDamage = dmgByDistance.InitialDamageAmount - distanceToTarget + 1;

            DamageWeaponSystem.ApplyDamageToRoom(activeWeapon.WeaponUser, currentDamage,
                World, targetRoomEntity, targetRoom.MechEntity);
        }
    }
}
}