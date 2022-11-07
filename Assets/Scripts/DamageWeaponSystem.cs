using Ecs.Components;
using Ext.LeoEcs;

namespace Ecs.Systems
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

            var targetRoom = roomPool.Get(targetRoomEntity);

            var newDmgEvent = MechDamageEvent.BuildFromRoom(activeWeapon.WeaponUser, targetRoomEntity,
                damageComp.DamageAmount, World);
            DamageApplySystem.TryAddDamageEvent(newDmgEvent, targetRoom.MechEntity, World);
        }
    }
}
}