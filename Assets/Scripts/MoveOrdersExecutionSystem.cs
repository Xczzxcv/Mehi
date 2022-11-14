using Ecs.Components;
using UnityEngine;

namespace Ecs.Systems
{
public class MoveOrdersExecutionSystem : EcsRunSystemBase2<MoveOrderComponent, PositionComponent>
{
    public MoveOrdersExecutionSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MoveOrderComponent moveOrderComp, 
        ref PositionComponent posComp, int entity)
    {
        var resultPos = posComp.Pos + moveOrderComp.PositionShift;
        Debug.Assert(BattleFieldManager.IsValidFieldPos(resultPos, Services.BattleManager.FieldSize));
        
        posComp.Pos = resultPos;
    }
}
}