using UnityEngine;

namespace Ecs.Components
{
public struct PositionComponent
{
    public Vector2Int Pos { get; private set; }

    public void Init(Vector2Int initPos)
    {
        Pos = initPos;
    }

    public void SetPos(Vector2Int newPos, int updatedEntity, int fieldSize)
    {
        Debug.Assert(BattleFieldManager.IsValidFieldPos(newPos, fieldSize));

        var oldPos = Pos;
        Pos = newPos;

        GlobalEventManager.BattleField.UnitMoved.HappenedWith(updatedEntity, oldPos, Pos);
        GlobalEventManager.BattleField.UnitUpdated.HappenedWith(updatedEntity);
    }
}
}