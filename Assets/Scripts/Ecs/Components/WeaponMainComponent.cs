using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct WeaponMainComponent
{
    public string WeaponId;
    public bool IsFriendlyFireEnabled;
    public EcsPackedEntity OwnerUnitEntity;
    public WeaponTargetConfig TargetConfig;
    public int UseDistance;
    public WeaponProjectileType ProjectileType;
    public WeaponGripType GripType;
}
}