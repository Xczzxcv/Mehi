using System;

namespace Ecs.Components.Weapon
{
public struct DelayUsageWeaponComponent : IWeaponComponent
{
    public int DelayAmount;
    [NonSerialized] public bool DelayStarted;
    [NonSerialized] public ActiveWeaponComponent ActiveWeaponComponent;
    [NonSerialized] public int StartDelayTurn;
}
}