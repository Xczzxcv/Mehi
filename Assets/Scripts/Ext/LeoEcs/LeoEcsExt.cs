﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using UnityEngine;

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
    public static bool HasComponent<TComponent>(this EcsWorld world, int entity)
        where TComponent : struct
    {
        var components = world.GetPool<TComponent>();
        return components.Has(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TComponent AddComponent<TComponent>(this EcsWorld world, int entity)
        where TComponent : struct
    {
        var components = world.GetPool<TComponent>();
        return ref components.Add(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void DelComponent<TComponent>(this EcsWorld world, int entity)
        where TComponent : struct
    {
        var components = world.GetPool<TComponent>();
        components.Del(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryUnpack(this in EcsPackedEntity packed, EcsWorld world, out int entity)
    {
        if (!packed.Unpack(world, out entity))
        {
            Debug.LogError($"Can't unpack entity {packed}");
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<EcsPackedEntity> PackEntities(this EcsWorld world, IEnumerable<int> entities)
    {
        var resultEntities = new List<EcsPackedEntity>();
        foreach (var entity in entities)
        {
            var packedEntity = world.PackEntity(entity);
            resultEntities.Add(packedEntity);
        }

        return resultEntities;
    }

    public static ref TComponent GetOrAddComponent<TComponent>(this EcsWorld world, int entity)
        where TComponent : struct
    {
        var pool = world.GetPool<TComponent>();
        return ref pool.GetOrAdd(entity);
    }

    public static ref TComponent GetOrAdd<TComponent>(this EcsPool<TComponent> pool, int entity) 
        where TComponent : struct
    {
        return ref pool.Has(entity)
            ? ref pool.Get(entity)
            : ref pool.Add(entity);
    }

    private static int[] _entityBuffer = new int[2048];
    public static bool HasEntity(this EcsWorld world, int entity)
    {
        var entitiesCount = world.GetAllEntities(ref _entityBuffer);
        for (int i = 0; i < entitiesCount; i++)
        {
            if (_entityBuffer[i] == entity)
            {
                return true;
            }
        }

        return false;
    }
}
}
