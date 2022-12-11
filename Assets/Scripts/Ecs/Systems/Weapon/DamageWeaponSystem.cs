using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;
using Leopotam.EcsLite;

namespace Ecs.Systems.Weapon
{
public class DamageWeaponSystem : WeaponSystemBase<DamageWeaponComponent>
{
    public DamageWeaponSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveWeaponComponent activeWeapon, 
        ref DamageWeaponComponent damageComp, int entity)
    {
        var weaponTarget = activeWeapon.WeaponTarget;
        var targetRooms = weaponTarget.TargetMechRooms;
        foreach (var targetRoomEntityPacked in targetRooms)
        {
            if (!TryGetRoomComp(targetRoomEntityPacked, World, 
                    out var targetRoomEntity, out var targetRoom))
            {
                continue;
            }

            ApplyDamageToRoom(activeWeapon.WeaponUser, damageComp.DamageAmount, World, 
                targetRoomEntity, targetRoom.MechEntity);
        }
    }

    public static bool TryGetRoomComp(EcsPackedEntity targetRoomEntityPacked, EcsWorld world,
        out int targetRoomEntity, out MechRoomComponent targetRoom)
    {
        var roomPool = world.GetPool<MechRoomComponent>();
        targetRoom = default;
        if (!targetRoomEntityPacked.TryUnpack(world, out targetRoomEntity))
        {
            return false;
        }

        if (!roomPool.Has(targetRoomEntity))
        {
            return false;
        }

        targetRoom = roomPool.Get(targetRoomEntity);
        return true;
    }

    public static void ApplyDamageToRoom(EcsPackedEntity weaponUser, int damageAmount, EcsWorld world,
        int targetRoomEntity, EcsPackedEntity targetMech)
    {
        var newDmgEvent = MechDamageEvent.BuildFromRoom(weaponUser, targetRoomEntity,
            damageAmount, world, GetHitChance(weaponUser, world));
        DamageApplySystem.TryAddDamageEvent(newDmgEvent, targetMech, world);
    }

    public static float GetHitChance(EcsPackedEntity weaponUserPacked, EcsWorld world)
    {
        if (!weaponUserPacked.Unpack(world, out var weaponUserEntity))
        {
            return 0;
        }

        var mechSystemTypes = BattleMechManager.GetMechSystemTypes(weaponUserEntity, world);
        return mechSystemTypes.Contains(MechSystemType.AimSystem_Head)
            ? 1f
            : 0.5f;
    }
}
}