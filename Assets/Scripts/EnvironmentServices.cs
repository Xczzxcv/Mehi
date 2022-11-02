using Leopotam.EcsLite;

public class EnvironmentServices
{
    public readonly EcsWorld World;

    public EnvironmentServices(
        EcsWorld world
    )
    {
        World = world;
    }

    public double Time => UnityEngine.Time.timeAsDouble;
    public float DeltaTime => UnityEngine.Time.deltaTime;
}