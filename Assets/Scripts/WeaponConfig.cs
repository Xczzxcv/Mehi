using System;
using System.Collections.Generic;
using Ecs.Components.Weapon;
using UnityEngine;

[Serializable]
public struct WeaponConfig
{
    public string WeaponId;
    public bool IsFriendlyFireEnabled;
    public int UseDistance;
    public WeaponTargetConfig WeaponTarget;
    public WeaponProjectileType ProjectileType;
    public WeaponGripType GripType;
    [SerializeReference, ReferencePicker(typeof(IWeaponComponent))]
    public List<IWeaponComponentBase> WeaponComponents;
    [SerializeReference, ReferencePicker(typeof(IWeaponRequirementComponent))]
    public List<IWeaponComponentBase> WeaponRequirements;
}