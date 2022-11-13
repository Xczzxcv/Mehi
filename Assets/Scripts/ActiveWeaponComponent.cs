using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct ActiveWeaponComponent
{
    public EcsPackedEntity WeaponUser;
    public WeaponTarget WeaponTarget;
}
}