using System.Collections.Generic;
using Leopotam.EcsLite;

public class EnvironmentServices
{
    public readonly EcsWorld World;
    public readonly BattleManager BattleManager;
    public readonly GameManager.GameConfig GameConfig;
    public readonly Dictionary<string, WeaponConfig> WeaponConfigs;

    public EnvironmentServices(
        EcsWorld world,
        BattleManager battleManager, 
        GameManager.GameConfig gameConfig,
        Dictionary<string, WeaponConfig> weaponConfigs
        )
    {
        World = world;
        BattleManager = battleManager;
        GameConfig = gameConfig;
        WeaponConfigs = weaponConfigs;
    }

    public double Time => UnityEngine.Time.timeAsDouble;
    public float DeltaTime => UnityEngine.Time.deltaTime;
}