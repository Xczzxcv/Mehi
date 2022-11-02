using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace Ecs.Systems
{
public abstract class EcsRunSystemBaseInternal : IEcsRunSystem , IEcsInitSystem
{
    protected readonly EnvironmentServices Services;
    protected EcsWorld World
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Services.World;
    }

    protected EcsFilter Filter;

    protected EcsRunSystemBaseInternal(EnvironmentServices services)
    {
        Services = services;
    }

    public virtual void Init(IEcsSystems systems)
    { }

    public abstract void Run(IEcsSystems systems);
}
}