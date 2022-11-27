using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct WeaponMainComponent
{
    public EcsPackedEntity OwnerUnitEntity;
    public string WeaponId;
    public int UseDistance;
    public WeaponTargetType TargetType;
    public WeaponProjectileType ProjectileType;
    public WeaponGripType GripType;
}
}