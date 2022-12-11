using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct StunEffectComponent : IEffectComponent
{
    public EcsPackedEntity EffectSource { get; set; }
    public int Duration;
    public bool AlreadyUpdated;
}
}