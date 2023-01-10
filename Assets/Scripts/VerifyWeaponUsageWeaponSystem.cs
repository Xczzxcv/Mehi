using System.Collections.Generic;
using Ecs.Components;
using Ecs.Systems.Weapon;
using Ext.LeoEcs;
using UnityEngine;

public class VerifyWeaponUsageWeaponSystem : WeaponSystemBase
{
    public VerifyWeaponUsageWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, int entity)
    {
        if (VerifyWeaponUser(in activeWeapon)
            && VerifyWeaponTarget(in activeWeapon, entity))
        {
            return;
        }

        Pool.Del(entity);
    }

    private bool VerifyWeaponUser(in ActiveWeaponComponent activeWeapon)
    {
        if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
        {
            return false;
        }

        if (World.HasComponent<StunEffectComponent>(weaponUserEntity))
        {
            return false;
        }

        return true;
    }

    private bool VerifyWeaponTarget(in ActiveWeaponComponent activeWeapon, int weaponEntity)
    {
        return VerifyTileAttack(activeWeapon, weaponEntity);
    }

    private bool VerifyTileAttack(ActiveWeaponComponent activeWeapon, int weaponEntity)
    {
        var weaponInfo = Services.BattleManager.GetWeaponInfo(weaponEntity);
        if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
        {
            return false;
        }

        var weaponUserPos = Services.BattleManager.GetUnitPosition(weaponUserEntity);
        var verifiedCounter = 0;
        var rejectedTiles = new List<Vector2Int>();
        foreach (var weaponTargetTilePos in activeWeapon.WeaponTarget.TargetTiles)
        {
            if (Services.BattleManager.IsValidTileToAttack(weaponInfo,
                    weaponUserPos, weaponTargetTilePos))
            {
                verifiedCounter++;
            }
            else
            {
                rejectedTiles.Add(weaponTargetTilePos);
            }
        }

        foreach (var rejectedTile in rejectedTiles)
        {
            activeWeapon.WeaponTarget.TargetTiles.Remove(rejectedTile);
        }

        return verifiedCounter > 0;
    }
}