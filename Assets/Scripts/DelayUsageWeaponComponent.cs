using System;

namespace Ecs.Components.Weapon
{
public struct DelayUsageWeaponComponent : IWeaponComponent
{
    public int DelayAmount;
    [NonSerialized] public DelayStage Stage;
    [NonSerialized] public ActiveWeaponComponent ActiveWeaponComponent;
    [NonSerialized] public int StartDelayTurn;

    public enum DelayStage
    {
        WeaponUsage,
        DelayStarted,
        DelayProcessed
    }
}
}