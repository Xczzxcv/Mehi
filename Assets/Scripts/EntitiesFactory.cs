using System;
using System.Collections.Generic;
using Ecs.Components;
using Ext.LeoEcs;
using Leopotam.EcsLite;
using UnityEngine;

public static class EntitiesFactory
{
    [Serializable]
    public struct MechConfig
    {
        public List<MechRoomConfig> Rooms;
        public int RightHandsAmount;
        public int LeftHandsAmount;
        public int RightLegsAmount;
        public int LeftLegsAmount;
        public int Health;
        public int Shield;
        public Vector2Int Position;
        public int RedStats;
        public int GreenStats;
        public int BlueStats;
        public int ActionPoints;
        public int MoveSpeed;
        public List<string> InitWeapons;
    }
    
    public static EcsPackedEntity BuildMechEntity(EcsWorld world, MechConfig config)
    {
        var newMechEntity = world.NewEntity();
        
        ref var mechComp = ref world.AddComponent<MechComponent>(newMechEntity);
        mechComp.WeaponIds = config.InitWeapons;
        mechComp.RightHandsAmount = config.RightHandsAmount;
        mechComp.LeftHandsAmount = config.LeftHandsAmount;
        mechComp.RightLegsAmount = config.RightLegsAmount;
        mechComp.LeftLegsAmount = config.LeftLegsAmount;

        ref var mechHealthComp = ref world.AddComponent<MechHealthComponent>(newMechEntity);
        mechHealthComp.Health = config.Health;
        mechHealthComp.Shield = config.Shield;
        
        ref var positionComp = ref world.AddComponent<PositionComponent>(newMechEntity);
        positionComp.Pos = config.Position;
        
        ref var statsComp = ref world.AddComponent<StatsComponent>(newMechEntity);
        statsComp.Red = config.RedStats;
        statsComp.Green = config.GreenStats;
        statsComp.Blue = config.BlueStats;
        
        ref var activeCreatureComp = ref world.AddComponent<ActiveCreatureComponent>(newMechEntity);
        activeCreatureComp.ActionPoints = config.ActionPoints;
        activeCreatureComp.MaxActionPoints = config.ActionPoints;
        activeCreatureComp.MoveSpeed = config.MoveSpeed;

        var newMechEntityPacked = world.PackEntity(newMechEntity);
        foreach (var mechRoomConfig in config.Rooms)
        {
            var roomConfig = mechRoomConfig;
            roomConfig.MechEntity = newMechEntityPacked;
            BuildMechRoomEntity(world, roomConfig);

            if (roomConfig.SystemConfig.Type != MechSystemType.None)
            {
                BuildMechSystemEntity(world, roomConfig.SystemConfig);
            }
        }

        return newMechEntityPacked;
    }
    
    [Serializable]
    public struct MechSystemConfig
    {
        public MechSystemType Type;
        public int Level;
        public bool IsActive;
        public EcsPackedEntity MechEntity;
    }

    public static EcsPackedEntity BuildMechSystemEntity(EcsWorld world, MechSystemConfig config)
    {
        var newMechSystemEntity = world.NewEntity();
        
        ref var mechSystemComp = ref world.AddComponent<MechSystemComponent>(newMechSystemEntity);
        mechSystemComp.Type = config.Type;
        mechSystemComp.Level = config.Level;
        mechSystemComp.IsActive = config.IsActive;
        mechSystemComp.MechEntity = config.MechEntity;
        
        return world.PackEntity(newMechSystemEntity);
    }

    [Serializable]
    public struct MechRoomConfig
    {
        public MechSystemConfig SystemConfig;
        public EcsPackedEntity MechEntity;
        public int Health;
    }
    
    public static EcsPackedEntity BuildMechRoomEntity(EcsWorld world, MechRoomConfig config)
    {
        var newMechRoomEntity = world.NewEntity();
        
        ref var mechRoomComp = ref world.AddComponent<MechRoomComponent>(newMechRoomEntity);
        mechRoomComp.SystemType = config.SystemConfig.Type;
        mechRoomComp.MechEntity = config.MechEntity;

        ref var healthComp = ref world.AddComponent<HealthComponent>(newMechRoomEntity);
        healthComp.Health = config.Health;
        healthComp.MaxHealth = config.Health;

        return world.PackEntity(newMechRoomEntity);
    }
}