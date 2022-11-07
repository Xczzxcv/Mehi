using Leopotam.EcsLite;
using UnityEngine.Profiling;

namespace Ecs.Systems
{
public abstract class EcsRunSystemBase : EcsRunSystemBaseInternal
{
    protected EcsRunSystemBase(EnvironmentServices services) : base(services)
    { }
}

public abstract class EcsRunSystemBase<TComponent> : EcsRunSystemBaseInternal
    where TComponent : struct
{
    private EcsPool<TComponent> _components;

    protected EcsRunSystemBase(EnvironmentServices services)
        : base(services)
    { }

    public override void Init(IEcsSystems systems)
    {
        Filter = World.Filter<TComponent>().End();
        _components = World.GetPool<TComponent>();
    }

    public override void Run(IEcsSystems systems)
    {
        Profiler.BeginSample(GetType().Name);
        ProcessComponents(Filter, _components, World);
        Profiler.EndSample();
    }

    private void ProcessComponents(EcsFilter filter, EcsPool<TComponent> components, EcsWorld world)
    {
        foreach (var entity in filter)
        {
            ref var component = ref components.Get(entity);
            ProcessComponent(ref component, entity);
        }
    }

    protected abstract void ProcessComponent(ref TComponent creatureComp, int entity);
}
}