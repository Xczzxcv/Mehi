using Ecs.Components;
using Ext.LeoEcs;

namespace Ecs.Systems
{
public class RefreshActionPoints : EcsRunSystemBase<ActiveCreatureComponent>
{
    public RefreshActionPoints(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref ActiveCreatureComponent creatureComp, int entity)
    {
        creatureComp.ActionPoints = creatureComp.MaxActionPoints;
    }
}
}