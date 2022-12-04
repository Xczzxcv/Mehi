using System;
using System.Collections.Generic;
using Ecs.Components.Weapon;
using UnityEngine;

[Serializable]
public struct WeaponConfig
{
    public string WeaponId;
    public int UseDistance;
    public WeaponTargetConfig WeaponTarget;
    public WeaponProjectileType ProjectileType;
    public WeaponGripType GripType;
    [SerializeReference, ReferencePicker]
    public List<IWeaponComponent> WeaponComponents;
}