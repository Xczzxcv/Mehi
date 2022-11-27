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
        var roomPool = World.GetPool<MechRoomComponent>();
        foreach (var targetRoomEntityPacked in targetRooms)
        {
            if (!targetRoomEntityPacked.TryUnpack(World, out var targetRoomEntity))
            {
                continue;
            }

            if (!roomPool.Has(targetRoomEntity))
            {
                continue;
            }

            ref var targetRoom = ref roomPool.Get(targetRoomEntity);

            var newDmgEvent = MechDamageEvent.BuildFromRoom(activeWeapon.WeaponUser, targetRoomEntity,
                damageComp.DamageAmount, World, GetHitChance(activeWeapon.WeaponUser, World));
            DamageApplySystem.TryAddDamageEvent(newDmgEvent, targetRoom.MechEntity, World);
        }
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