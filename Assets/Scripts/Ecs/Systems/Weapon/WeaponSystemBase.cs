﻿using Ecs.Components;
using Ecs.Components.Weapon;
using Ext.LeoEcs;

namespace Ecs.Systems.Weapon
{
public abstract class WeaponSystemBase : EcsRunSystemBase<ActiveWeaponComponent>, IWeaponSystem
{
    protected WeaponSystemBase(EnvironmentServices services) : base(services)
    { }
}

public abstract class WeaponSystemBase<TComponent> :
    EcsRunSystemBase2<ActiveWeaponComponent, TComponent>, IWeaponSystem
    where TComponent : struct, IWeaponComponent
{
    protected WeaponSystemBase(EnvironmentServices services) : base(services)
    { }
}
}