using Ecs.Components;
using Ext.LeoEcs;

namespace Ecs.Systems
{
public class RepairSelfOrderExecutionSystem : EcsRunSystemBase2<RepairSelfOrderComponent, ActiveCreatureComponent>
{
    public const int REPAIR_ALL_ROOMS_ACTION_COST = 1;

    public RepairSelfOrderExecutionSystem(EnvironmentServices services) : base(services)
    { }

    protected override void ProcessComponent(ref RepairSelfOrderComponent _,  
        ref ActiveCreatureComponent activeCreature, int entity)
    {
        var roomEntities = BattleMechManager.GetRoomEntities(entity, World);

        var healthPool = World.GetPool<HealthComponent>();
        foreach (var roomEntity in roomEntities)
        {
            ref var roomHp = ref healthPool.Get(roomEntity);
            roomHp.Health = roomHp.MaxHealth;
        }

        activeCreature.ActionPoints -= REPAIR_ALL_ROOMS_ACTION_COST;
        GlobalEventManager.BattleField.UnitUpdated.HappenedWith(entity);
    }
}
}