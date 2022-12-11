using Leopotam.EcsLite;

namespace Ecs.Components
{
public interface IEffectComponent
{
    public EcsPackedEntity EffectSource { get; }
}
}