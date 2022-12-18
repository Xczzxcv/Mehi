using Ecs.Components;
using Ecs.Systems;
using Ecs.Systems.Weapon;
using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;
#if UNITY_EDITOR
using Leopotam.EcsLite.UnityEditor;
#endif

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
        _environmentServices = new EnvironmentServices(
            World,
            gameManager.BattleManager,
            gameManager.Config,
            gameManager.WeaponConfigs
        );
        SetupTurnSystems();
        SetupSystems();
        SetupWeaponSystems();
    }

    private void SetupTurnSystems()
    {
        _turnSystems = new EcsSystems(World);
        _turnSystems
            .Add(new RefreshActionPointsSystem(_environmentServices))
            .Add(new MechRoomBurningDamageApplySystem(_environmentServices))
            .Add(new StunEffectResetUpdateSystem(_environmentServices))
#if UNITY_EDITOR
            .Add(new EcsWorldDebugSystem())
#endif
            .Init();
    }

    private void SetupSystems()
    {
        _systems = new EcsSystems(World);
        _systems
            .Add(new StunEffectUpdateSystem(_environmentServices))
            .Add(new TempShieldUpdateSystem(_environmentServices))
            .Add(new DelayUsageWeaponUpdateSystem(_environmentServices))
            .Add(new RepairSelfOrderExecutionSystem(_environmentServices))
            .Add(new DamageApplySystem(_environmentServices))
            .Add(new DetectGeneralDeathSystem(_environmentServices))
            .Add(new DetectMechDeathSystem(_environmentServices))
            .Add(new MoveOrdersExecutionSystem(_environmentServices))
            .Add(new MoveCreatureSystem(_environmentServices))
            .Add(new UseWeaponOrdersExecutionSystem(_environmentServices))
            .DelHere<RepairSelfOrderComponent>()
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
            .AddGroup("weapon_requirements", true, null, new []
            {
                new DistanceWeaponRequirementSystem(_environmentServices)
            })
            .Add(new DelayUsageWeaponSystem(_environmentServices))
            .Add(new AddShieldToSelfWeaponSystem(_environmentServices))
            .Add(new DamageWeaponSystem(_environmentServices))
            .Add(new DamageByDistanceWeaponSystem(_environmentServices))
            .Add(new PushWeaponSystem(_environmentServices))
            .Add(new StunWeaponSystem(_environmentServices))
            .DelHere<ActiveWeaponComponent>()
            .Add(new DamageApplySystem(_environmentServices))
            .Add(new DetectMechDeathSystem(_environmentServices))
            .Add(new DetectGeneralDeathSystem(_environmentServices))
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