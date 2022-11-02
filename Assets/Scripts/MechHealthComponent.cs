using UnityEngine;

namespace Ecs.Components
{
public struct MechHealthComponent
{
    public int Shield;
}



public struct PositionComponent
{
    public Vector2 Pos;
}

public struct StatsComponent
{
    public int Red;
    public int Green;
    public int Blue;
}

public struct ActiveCreatureComponent
{
    public int ActionPoints;
    public int MoveSpeed;
}

public struct MechComponent
{
    public int RightHandsAmount;
    public int LeftHandsAmount;
    public int RightLegsAmount;
    public int LeftLegsAmount;
}
}
