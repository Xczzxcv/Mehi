using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct DamageEventComponent
{
    public EcsPackedEntity DamageSource;
    public EcsPackedEntity DamageTarget;
    public int DamageAmount;
}
}