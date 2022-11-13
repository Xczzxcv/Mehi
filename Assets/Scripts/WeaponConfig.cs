using System;
using System.Collections.Generic;
using Ecs.Components.Weapon;
using UnityEngine;

[Serializable]
public struct WeaponConfig
{
    public string WeaponId;
    [SerializeReference, ReferencePicker]
    public List<IWeaponComponent> WeaponComponents;
}