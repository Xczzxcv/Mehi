using Ecs.Components;
using Ecs.Systems;
using Ecs.Systems.Weapon;
using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.UnityEditor;
using UnityEngine;

public class EcsManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float updatePeriod;
    
    public EcsWorld World { get; private set; }
 
    private IEcsSystems _turnSystems;
    private IEcsSystems _systems;
    private IEcsSystems _weaponSystems;
    private EnvironmentServices _environmentServices;

    private int _lastProcessedTurnIndex;
    private float _lastTimeUpdate;

    private void Awake()
    {
        World = new EcsWorld();
        
        GlobalEventManager.Turns.TurnUpdated.Event += OnTurnUpdated;
    }

    public void Setup()
    {
        _environmentServices = new EnvironmentServices(World, gameManager.BattleManager, gameManager.Config);
        SetupTurnSystems();
        SetupSystems();
        SetupWeaponSystems();
    }

    private void SetupTurnSystems()
    {
        _turnSystems = new EcsSystems(World);
        _turnSystems
            .Add(new RefreshActionPoints(_environmentServices))
            .Add(new MechRoomBurningDamageApplySystem(_environmentServices))
#if UNITY_EDITOR
            .Add(new EcsWorldDebugSystem())
#endif
            .Init();
    }

    private void SetupSystems()
    {
        _systems = new EcsSystems(World);
        _systems
            .Add(new DamageApplySystem(_environmentServices))
            .Add(new MoveOrdersExecutionSystem(_environmentServices))
            .Add(new MoveCreatureSystem(_environmentServices))
            .Add(new UseWeaponOrdersExecutionSystem(_environmentServices))
            .DelHere<MoveOrderComponent>()
            .DelHere<UseWeaponOrderComponent>()
#if UNITY_EDITOR
            .Add(new EcsWorldDebugSystem())
#endif
            .Init();
    }

    private void SetupWeaponSystems()
    {
        _weaponSystems = new EcsSystems(World);
        _weaponSystems
            .Add(new DamageWeaponSystem(_environmentServices))
            .Add(new PushWeaponSystem(_environmentServices))
            .Add(new StunWeaponSystem(_environmentServices))
            .DelHere<ActiveWeaponComponent>()
            .Add(new DamageApplySystem(_environmentServices))
#if UNITY_EDITOR
            .Add(new EcsWorldDebugSystem())
#endif
            .Init();
    }

    private void Update()
    {
        if (Time.time - _lastTimeUpdate < updatePeriod)
        {
            return;
        }

        _lastTimeUpdate = Time.time;
        RunSystems();
    }

    public void RunSystems()
    {
        _systems.Run();
        _weaponSystems.Run();
    }

    private void RunTurnSystems()
    {
        _turnSystems.Run();
    }

    private void OnTurnUpdated(int turnIndex, TurnsManager.TurnPhase turnPhase)
    {
        if (turnIndex == _lastProcessedTurnIndex)
        {
            return;
        }

        RunTurnSystems();
        _lastProcessedTurnIndex = turnIndex;
    }
}