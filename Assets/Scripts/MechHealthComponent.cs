using System.Collections.Generic;
using UnityEngine;

namespace Ecs.Components
{
public struct MechHealthComponent
{
    public int MaxHealth;
    public int Health;
    public int Shield;
}



public struct PositionComponent
{
    public Vector2Int Pos { get; private set; }

    public void Init(Vector2Int initPos)
    {
        Pos = initPos;
    }

    public void SetPos(Vector2Int newPos, int updatedEntity)
    {
        var oldPos = Pos;
        Pos = newPos;

        GlobalEventManager.BattleField.UnitMoved.HappenedWith(updatedEntity, oldPos, Pos);
    }
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
