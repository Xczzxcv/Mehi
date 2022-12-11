using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct UseWeaponOrderComponent
{
    public EcsPackedEntity WeaponEntity;
    public InputWeaponTarget WeaponTarget;
}
}