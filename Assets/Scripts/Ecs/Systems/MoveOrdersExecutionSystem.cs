using System.Linq;
using Ecs.Components;
using Ext;
using Ext.LeoEcs;
using UnityEngine;

namespace Ecs.Systems
{
public class MoveOrdersExecutionSystem : EcsRunSystemBase4<MoveOrderComponent, MoveCreatureComponent, 
    PositionComponent, ActiveCreatureComponent>
{
    public const int MOVE_UNIT_ACTION_COST = 1;

    public MoveOrdersExecutionSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref MoveOrderComponent moveOrderComp,
        ref MoveCreatureComponent moveCreatureComp, ref PositionComponent posComp,
        ref ActiveCreatureComponent activeCreatureComp, int entity)
    {
        Debug.Assert(posComp.Pos == moveOrderComp.Path.Parts.First().Node.Position.ToV2I());

        if (!moveOrderComp.Path.Parts.Any())
        {
            return;
        }

        Debug.Assert(posComp.Pos == moveOrderComp.Path.Parts.First().Node.Position.ToV2I());
        
        for (var i = 1; i < moveOrderComp.Path.Parts.Count; i++)
        {
            var pathPart = moveOrderComp.Path.Parts[i];
            var moveCreaturePathPart = new MoveCreatureComponent.PathPart
            {
                DestPos = pathPart.Node.Position.ToV2I(),
            };
            moveCreatureComp.Path.Enqueue(moveCreaturePathPart);
        }

        activeCreatureComp.ActionPoints -= MOVE_UNIT_ACTION_COST;
    }
}
}