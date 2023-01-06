using System;
using System.Collections.Generic;
using Ecs.Components;
using Ecs.Components.Weapon;
using Ecs.Systems;
using Ext.LeoEcs;
using Extension;
using Leopotam.EcsLite;
using UnityEngine;

public static class EntitiesFactory
{
    [Serializable]
    public struct MechConfig
    {
        public UnitControl Control;
        public List<MechRoomConfig> Rooms;
        public int Health;
        public int Shield;
        public Vector2Int Position;
        public int RedStats;
        public int GreenStats;
        public int BlueStats;
        public int ActionPoints;
        public int MoveSpeed;
    }
    
    public static EcsPackedEntity BuildMechEntity(EcsWorld world, MechConfig config, 
        Dictionary<string, WeaponConfig> weaponConfigs)
    {
        var newMechEntity = world.NewEntity();

        SetupControl(world, config.Control, newMechEntity);
        
        ref var mechComp = ref world.AddComponent<MechComponent>(newMechEntity);

        ref var mechHealthComp = ref world.AddComponent<MechHealthComponent>(newMechEntity);
        mechHealthComp.MaxHealth = config.Health;
        mechHealthComp.Health = config.Health;
        mechHealthComp.Shield = config.Shield;
        
        ref var mechDmgApplyComp = ref world.AddComponent<MechDamageApplyComponent>(newMechEntity);
        mechDmgApplyComp.Events = new List<MechDamageEvent>();

        ref var positionComp = ref world.AddComponent<PositionComponent>(newMechEntity);
        positionComp.Init(config.Position);
        
        ref var moveCreatureComp = ref world.AddComponent<MoveCreatureComponent>(newMechEntity);
        moveCreatureComp.Path = new Queue<MoveCreatureComponent.PathPart>();
        
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
            var newRoomEntity = BuildMechRoomEntity(world, roomConfig);

            if (roomConfig.SystemConfig.Type != MechSystemType.None)
            {
                AddSystemToRoomEntity(newRoomEntity, world, newMechEntityPacked, 
                    roomConfig.SystemConfig, weaponConfigs);
            }
        }

        return newMechEntityPacked;
    }

    private static void SetupControl(EcsWorld world, UnitControl control, int unitEntity)
    {
        switch (control)
        {
            case UnitControl.Player:
                world.AddComponent<PlayerControlComponent>(unitEntity);
                break;
            case UnitControl.AI:
                world.AddComponent<AiControlComponent>(unitEntity);
                break;
        }
    }

    [Serializable]
    public struct MechSystemConfig
    {
        [SearchableEnum]
        public MechSystemType Type;
        public int Level;
        public bool IsActive;
        public string WeaponId;
    }

    public static void AddSystemToRoomEntity(int mechRoomEntity, EcsWorld world,
        EcsPackedEntity mechEntityPacked, MechSystemConfig config,
        Dictionary<string, WeaponConfig> weaponConfigs)
    {
        ref var mechSystemComp = ref world.AddComponent<MechSystemComponent>(mechRoomEntity);
        mechSystemComp.Type = config.Type;
        mechSystemComp.Level = config.Level;
        mechSystemComp.IsActive = config.IsActive;
        mechSystemComp.MechEntity = mechEntityPacked;

        if (MechSystemComponent.IsWeaponHandlingSystem(mechSystemComp.Type))
        {
            ref var mechWeaponSlot = ref world.AddComponent<MechWeaponSlotComponent>(mechRoomEntity);

            var containsWeapon = !config.WeaponId.IsNullOrEmpty();
            if (containsWeapon)
            {
                mechWeaponSlot.WeaponId = config.WeaponId;
            
                var weaponConfig = weaponConfigs[config.WeaponId];
                BuildWeapon(world, mechEntityPacked, weaponConfig);
            }
        }
    }

    [Serializable]
    public struct MechRoomConfig
    {
        public MechSystemConfig SystemConfig;
        public EcsPackedEntity MechEntity;
        public int Health;
    }
    
    public static int BuildMechRoomEntity(EcsWorld world, MechRoomConfig config)
    {
        var newMechRoomEntity = world.NewEntity();
        
        ref var mechRoomComp = ref world.AddComponent<MechRoomComponent>(newMechRoomEntity);
        mechRoomComp.SystemType = config.SystemConfig.Type;
        mechRoomComp.MechEntity = config.MechEntity;

        ref var healthComp = ref world.AddComponent<HealthComponent>(newMechRoomEntity);
        healthComp.Health = config.Health;
        healthComp.MaxHealth = config.Health;
        healthComp.StayAfterDeath = true;

        return newMechRoomEntity;
    }

    public static EcsPackedEntity BuildWeapon(EcsWorld world, EcsPackedEntity weaponOwnerEntity, 
        WeaponConfig weaponConfig)
    {
        var newWeaponEntity = world.NewEntity();
        
        var weaponPool = world.GetPool<WeaponMainComponent>();
        ref var weaponMainComp = ref weaponPool.Add(newWeaponEntity);
        weaponMainComp.WeaponId = weaponConfig.WeaponId;
        weaponMainComp.IsFriendlyFireEnabled = weaponConfig.IsFriendlyFireEnabled;
        weaponMainComp.OwnerUnitEntity = weaponOwnerEntity;
        weaponMainComp.TargetConfig = weaponConfig.WeaponTarget;
        weaponMainComp.UseDistance = weaponConfig.UseDistance;
        weaponMainComp.ProjectileType = weaponConfig.ProjectileType;
        weaponMainComp.GripType = weaponConfig.GripType;

        AddWeaponComponents(world, weaponConfig.WeaponComponents, newWeaponEntity);
        AddWeaponComponents(world, weaponConfig.WeaponRequirements, newWeaponEntity);

        return world.PackEntity(newWeaponEntity);
    }

    private static void AddWeaponComponents(EcsWorld world, List<IWeaponComponentBase> weaponComponents, int newWeaponEntity)
    {
        foreach (var weaponComponent in weaponComponents)
        {
            var weaponComponentPool = world.GetPoolByType(weaponComponent.GetType());
            if (weaponComponentPool == null)
            {
                Debug.LogError($"Can't get pool for type {weaponComponent}");
                continue;
            }

            weaponComponentPool.AddRaw(newWeaponEntity, weaponComponent);
        }
    }

    public static void BuildMoveOrder(int unitEntity, Graph.Path path, EcsWorld world)
    {
        ref var moveOrder = ref world.AddComponent<MoveOrderComponent>(unitEntity);
        moveOrder.Path = path;
    }

    public static void BuildUseWeaponOrder(int userUnitEntity, BattleMechManager.WeaponInfo usedWeaponInfo,
        InputWeaponTarget weaponTarget, EcsWorld world)
    {
        ref var useWeaponOrder = ref world.AddComponent<UseWeaponOrderComponent>(userUnitEntity);
        if (!UseWeaponOrdersExecutionSystem.TryGetWeaponEntity(usedWeaponInfo.WeaponId, userUnitEntity,
                world, out var weaponEntity))
        {
            Debug.LogError($"Can't find weapon {usedWeaponInfo.WeaponId}");
            return;
        }

        useWeaponOrder.WeaponEntity = world.PackEntity(weaponEntity);
        useWeaponOrder.WeaponTarget = weaponTarget;
    }

    public static void BuildRepairSelfOrder(int unitEntity, EcsWorld world)
    {
        ref var repairSelfOrder = ref world.AddComponent<RepairSelfOrderComponent>(unitEntity);
    }
}