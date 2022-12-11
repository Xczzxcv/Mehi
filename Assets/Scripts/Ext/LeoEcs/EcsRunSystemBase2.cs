using Leopotam.EcsLite;
using UnityEngine.Profiling;

namespace Ext.LeoEcs
{
public abstract class EcsRunSystemBase2<TComponent1, TComponent2> : EcsRunSystemBaseInternal
    where TComponent1 : struct
    where TComponent2 : struct 
{
    private EcsPool<TComponent1> _components1;
    private EcsPool<TComponent2> _components2;

    protected EcsRunSystemBase2(EnvironmentServices services) 
        : base(services)
    { }

    public override void Init(IEcsSystems systems)
    {
        base.Init(systems);
        Filter = World.Filter<TComponent1>().Inc<TComponent2>().End();
        _components1 = World.GetPool<TComponent1>();
        _components2 = World.GetPool<TComponent2>();
    }

    public override void Run(IEcsSystems systems)
    {
        Profiler.BeginSample(GetType().Name);
        ProcessComponents(Filter, _components1, _components2, World);
        Profiler.EndSample();
    }

    private void ProcessComponents(EcsFilter filter, EcsPool<TComponent1> components1, 
        EcsPool<TComponent2> components2, EcsWorld world)
    {
        PreProcess();
        
        foreach (var entity in filter)
        {
            ref var component1 = ref components1.Get(entity);
            ref var component2 = ref components2.Get(entity);
            ProcessComponent(ref component1, ref component2, entity);
        }

        PostProcess();
    }

    protected virtual void PreProcess()
    { }

    protected virtual void PostProcess()
    { }

    protected abstract void ProcessComponent(ref TComponent1 component1, ref TComponent2 component2, 
        int entity);
}
}