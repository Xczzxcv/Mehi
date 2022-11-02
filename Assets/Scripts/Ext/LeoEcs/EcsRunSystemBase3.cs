using Leopotam.EcsLite;
using UnityEngine.Profiling;

namespace Ecs.Systems
{
public abstract class EcsRunSystemBase3<TComponent1, TComponent2, TComponent3> : EcsRunSystemBaseInternal
    where TComponent1 : struct
    where TComponent2 : struct 
    where TComponent3 : struct 
{
    private EcsPool<TComponent1> _components1;
    private EcsPool<TComponent2> _components2;
    private EcsPool<TComponent3> _components3;
    
    protected EcsRunSystemBase3(EnvironmentServices services) 
        : base(services)
    { }
    
    public override void Init(IEcsSystems systems)
    {
        base.Init(systems);
        Filter = World.Filter<TComponent1>().Inc<TComponent2>().End();
        _components1 = World.GetPool<TComponent1>();
        _components2 = World.GetPool<TComponent2>();
        _components3 = World.GetPool<TComponent3>();
    }

    public override void Run(IEcsSystems systems)
    {
        Profiler.BeginSample(GetType().Name);
        ProcessComponents(Filter, _components1, _components2, _components3);
        Profiler.EndSample();
    }

    private void ProcessComponents(EcsFilter filter, EcsPool<TComponent1> components1, 
        EcsPool<TComponent2> components2, EcsPool<TComponent3> components3)
    {
        PreProcess();

        foreach (var entity in filter)
        {
            ref var component1 = ref components1.Get(entity);
            ref var component2 = ref components2.Get(entity);
            ref var component3 = ref components3.Get(entity);
            ProcessComponent(ref component1, ref component2, ref component3, entity);
        }

        PostProcess();
    }
    
    protected virtual void PreProcess()
    { }
    
    protected virtual void PostProcess()
    { }

    protected abstract void ProcessComponent(ref TComponent1 component1, ref TComponent2 component2, 
        ref TComponent3 component3, int entity);
}
}
