using System;
using UnityEngine;

[Serializable]
public struct WeaponTargetConfig
{
    public WeaponTargetType TargetType;
    public Vector2Int RoomTargetsSize;
    public int UnitTargetsCount;
    public Vector2Int TileTargetsSize;
}