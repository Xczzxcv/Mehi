using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct MechDamageEvent
{
    public EcsPackedEntity DamageSource;
    public EcsPackedEntity DamageTargetRoom;
    public int DamageAmount;
}
}