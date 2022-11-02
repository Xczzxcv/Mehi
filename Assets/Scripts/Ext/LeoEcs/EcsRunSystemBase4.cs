using Leopotam.EcsLite;
using UnityEngine.Profiling;

namespace Ecs.Systems
{
public abstract class EcsRunSystemBase4<TComponent1, TComponent2, TComponent3, TComponent4> : EcsRunSystemBaseInternal
    where TComponent1 : struct
    where TComponent2 : struct 
    where TComponent3 : struct 
    where TComponent4 : struct 
{
    private EcsPool<TComponent1> _components1;
    private EcsPool<TComponent2> _components2;
    private EcsPool<TComponent3> _components3;
    private EcsPool<TComponent4> _components4;
    
    protected EcsRunSystemBase4(EnvironmentServices services) 
        : base(services)
    { }
    
    public override void Init(IEcsSystems systems)
    {
        base.Init(systems);
        Filter = World.Filter<TComponent1>().Inc<TComponent2>().End();
        _components1 = World.GetPool<TComponent1>();
        _components2 = World.GetPool<TComponent2>();
        _components3 = World.GetPool<TComponent3>();
        _components4 = World.GetPool<TComponent4>();
    }

    public override void Run(IEcsSystems systems)
    {
        Profiler.BeginSample(GetType().Name);
        ProcessComponents(Filter, _components1, _components2, _components3, _components4);
        Profiler.EndSample();
    }

    private void ProcessComponents(EcsFilter filter, EcsPool<TComponent1> components1, 
        EcsPool<TComponent2> components2, EcsPool<TComponent3> components3, EcsPool<TComponent4> components4)
    {
        PreProcess();

        foreach (var entity in filter)
        {
            ref var component1 = ref components1.Get(entity);
            ref var component2 = ref components2.Get(entity);
            ref var component3 = ref components3.Get(entity);
            ref var component4 = ref components4.Get(entity);
            ProcessComponent(ref component1, ref component2, ref component3, ref component4, entity);
        }

        PostProcess();
    }
    
    protected virtual void PreProcess()
    { }
    
    protected virtual void PostProcess()
    { }

    protected abstract void ProcessComponent(ref TComponent1 component1, ref TComponent2 component2, 
        ref TComponent3 component3, ref TComponent4 component4, int entity);
}
}