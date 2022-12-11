using System;
using Leopotam.EcsLite;

namespace Ecs.Components
{
public struct TempShieldComponent : IEcsAutoReset<TempShieldComponent>
{
    public int Amount;
    [NonSerialized] public int InitTurnIndex;

    public const int DEFAULT_TURN_INDEX = -1;

    public void AutoReset(ref TempShieldComponent c)
    {
        c.InitTurnIndex = DEFAULT_TURN_INDEX;
    }
}
}