using System.Collections.Generic;
using UnityEngine;

namespace Ecs.Components
{
public struct MechHealthComponent
{
    public int Health;
    public int Shield;
}



public struct PositionComponent
{
    public Vector2Int Pos;
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
    public int MaxActionPoints;
    public int MoveSpeed;
}

public struct MechComponent
{
    public List<string> WeaponIds;
    public int RightHandsAmount;
    public int LeftHandsAmount;
    public int RightLegsAmount;
    public int LeftLegsAmount;
}

public struct PlayerControlComponent
{ }

public struct AiControlComponent
{ }
}
