using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct BurningEffectComponent : IEffectComponent
{
    public EcsPackedEntity EffectSource { get; set; }
}
}