using Leopotam.EcsLite;

public class EnvironmentServices
{
    public readonly EcsWorld World;
    public readonly BattleManager BattleManager;
    public readonly GameManager.GameConfig GameConfig;

    public EnvironmentServices(
        EcsWorld world,
        BattleManager battleManager, 
        GameManager.GameConfig gameConfig
        )
    {
        World = world;
        BattleManager = battleManager;
        GameConfig = gameConfig;
    }

    public double Time => UnityEngine.Time.timeAsDouble;
    public float DeltaTime => UnityEngine.Time.deltaTime;
}