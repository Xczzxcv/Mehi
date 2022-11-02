using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace Ext.LeoEcs
{
public static class LeoEcsExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TComponent GetComponent<TComponent>(this EcsWorld world, int entity)
        where TComponent : struct
    {
        var components = world.GetPool<TComponent>();
        return ref components.Get(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TComponent AddComponent<TComponent>(this EcsWorld world, int entity)
        where TComponent : struct
    {
        var components = world.GetPool<TComponent>();
        return ref components.Add(entity);
    }
}
}
