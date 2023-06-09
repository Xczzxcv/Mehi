﻿using System;
using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;
using UnityEngine;

namespace Ecs.Systems.Weapon
{
public class PushWeaponSystem : WeaponSystemBase<PushWeaponComponent>
{
    public PushWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref PushWeaponComponent pushWeapon, int entity)
    {
        var positionPool = World.GetPool<PositionComponent>();
        foreach (var targetMechPacked in activeWeapon.WeaponTarget.TargetMechEntities)
        {
            if (!activeWeapon.WeaponUser.TryUnpack(World, out var weaponUserEntity))
            {
                continue;
            }

            if (!positionPool.Has(weaponUserEntity))
            {
                Debug.LogError("Weapon user has no position");
                continue;
            }

            if (!targetMechPacked.TryUnpack(World, out var targetMechEntity))
            {
                continue;
            }

            if (!positionPool.Has(targetMechEntity))
            {
                Debug.LogError("Mech has no position");
                continue;
            }

            ref var weaponUserMechPosition = ref positionPool.Get(weaponUserEntity);
            ref var targetMechPosition = ref positionPool.Get(targetMechEntity);

            var posDiff = targetMechPosition.Pos - weaponUserMechPosition.Pos;
            var pushDirection = GetPushDirection(posDiff);
            var pushDistance = GetPushDistance(in pushWeapon, in targetMechPosition, pushDirection);

            if (pushDistance <= 0)
            {
                continue;
            }

            var resultTargetPos = targetMechPosition.Pos + pushDirection * pushDistance;
            targetMechPosition.SetPos(resultTargetPos, targetMechEntity, Services.BattleManager.FieldSize);
        }
    }

    private static Vector2Int GetPushDirection(Vector2Int posDiff)
    {
        Vector2Int pushDir;
        if (Math.Abs(posDiff.x) > Math.Abs(posDiff.y))
        {
            pushDir = new Vector2Int(
                Math.Sign(posDiff.x),
                0
            );
        }
        else if (Math.Abs(posDiff.x) < Math.Abs(posDiff.y))
        {
            pushDir = new Vector2Int(
                0,
                Math.Sign(posDiff.y)
            );
        }
        else
        {
            pushDir = new Vector2Int(
                Math.Sign(posDiff.x),
                Math.Sign(posDiff.y)
            );
        }

        return pushDir;
    }

    private int GetPushDistance(in PushWeaponComponent pushWeapon, in PositionComponent targetMechPosition,
        Vector2Int pushDirection)
    {
        for (var i = 1; i <= pushWeapon.PushDistance; i++)
        {
            var tileToCheckPos = targetMechPosition.Pos + pushDirection * i;
            var isValidFieldPos = BattleFieldManager.IsValidFieldPos(tileToCheckPos, Services.BattleManager.FieldSize);
            var noUnitAtPos = !Services.BattleManager.TryGetUnitInPos(tileToCheckPos, out _);
            var tileAtPos = Services.BattleManager.GetFieldTile(tileToCheckPos);
            var canWalkOnTileAtPos = tileAtPos.Walkable;
            if (isValidFieldPos && noUnitAtPos && canWalkOnTileAtPos)
            {
                continue;
            }

            var pushDistance = i - 1;
            return pushDistance;
        }

        return pushWeapon.PushDistance;
    }
}
}