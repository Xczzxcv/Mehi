using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct MechDamageEvent
{
    public EcsPackedEntity DamageSource;
    public EcsPackedEntity DamageTargetRoom;
    public int DamageAmount;
    public float HitChance;

    public static MechDamageEvent BuildFromRoom(EcsPackedEntity dmgSrc, int roomEntity, 
        int dmgAmount, EcsWorld world, float hitChance = 1f)
    {
        return new MechDamageEvent
        {
            DamageSource = dmgSrc,
            DamageTargetRoom = world.PackEntity(roomEntity),
            DamageAmount = dmgAmount,
            HitChance = hitChance,
        };
    }
}
}