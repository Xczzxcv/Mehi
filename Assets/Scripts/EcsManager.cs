using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;

public class EcsManager : MonoBehaviour
{
    public EcsWorld World { get; private set; }
    private IEcsSystems _systems;
    private EnvironmentServices _environmentServices;

    private void Awake()
    {
        World = new EcsWorld();
        _environmentServices = new EnvironmentServices(World);
        _systems = new EcsSystems(World);
        _systems
            // .AddGroup("kek", true, null, GetCollisionSystems())
            .Add(new DamageApplySystem(_environmentServices))
#if UNITY_EDITOR
            .Add(new EcsWorldDebugSystem())
#endif
            .Init();
    }

    private IEcsSystem[] GetCollisionSystems()
    {
        return new IEcsSystem[]
        {
        };
    }

    private void Update()
    {
        OnUpdate();
    }

    public void OnUpdate()
    {
        _systems.Run();
    }
}