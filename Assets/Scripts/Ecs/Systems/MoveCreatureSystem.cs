﻿using System.Linq;
using Ecs.Components;
using Ext.LeoEcs;
using UnityEngine;

namespace Ecs.Systems
{
public class MoveCreatureSystem : EcsRunSystemBase2<PositionComponent, MoveCreatureComponent>
{
    public MoveCreatureSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref PositionComponent posComp,
        ref MoveCreatureComponent moveCreatureComp, int entity)
    {
        if (!moveCreatureComp.Path.Any())
        {
            return;
        }

        var destPos = moveCreatureComp.Path.Dequeue().DestPos;
        var posShift = destPos - posComp.Pos;
        
        var resultPos = posComp.Pos + posShift;

        posComp.SetPos(resultPos, entity, Services.BattleManager.FieldSize);
    }
}
}