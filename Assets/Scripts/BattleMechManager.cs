using System;
using System.Collections.Generic;
using Ecs.Components;
using Ecs.Components.Weapon;
using Ecs.Systems;
using Ext.LeoEcs;
using Extension;
using Leopotam.EcsLite;
using UnityEngine;

public class BattleMechManager
{
    public struct Config
    {
        public EcsWorld World;
        public TurnsManager TurnsManager;
    }

    public enum ControlledBy
    {
        None,
        Player,
        AI,
    }
    
    public struct BattleUnitInfo
    {
        public ControlledBy ControlledBy;
        public int MaxActionPoints;
        public int ActionPoints;
        public Vector2Int Position;
        public int MoveSpeed;
        public int MaxHealth;
        public int Health;
        public int Shield;
        public int? TempShield;
        public int Entity;
        public bool CanMove;
        public bool CanUseWeapons;
        public bool CanRepairSelf;
        public List<RoomInfo> Rooms;
        public List<WeaponInfo> Weapons;
    }

    public struct WeaponInfo
    {
        public string WeaponId;
        public WeaponTargetConfig WeaponTarget;
        public WeaponProjectileType ProjectileType;
        public WeaponGripType GripType;
        public int UseDistance;
        public bool CanUse;
        public int? Cooldown;
        public Dictionary<string, object> Stats;
    }

    public struct RoomInfo
    {
        public int Entity;
        public int Health;
        public int MaxHealth;
        public MechSystemType SystemType;
        public int SystemLvl;
    }

    private readonly Config _config;

    public BattleMechManager(Config config)
    {
        _config = config;
    }

    public List<BattleUnitInfo> GetUnitInfos()
    {
        var resultList = new List<BattleUnitInfo>();
        var activeUnitsFilter = _config.World
            .Filter<ActiveCreatureComponent>()
            .End();
        foreach (var unitEntity in activeUnitsFilter)
        {
            var unitInfo = GetUnitInfo(unitEntity);
            resultList.Add(unitInfo);
        }

        return resultList;
    }

    public BattleUnitInfo GetUnitInfo(int unitEntity)
    {
        return new BattleUnitInfo
        {
            ControlledBy = GetUnitControl(unitEntity, _config.World),
            MaxHealth = GetUnitHealth(unitEntity).MaxHealth,
            Health = GetUnitHealth(unitEntity).Health,
            Shield = GetUnitHealth(unitEntity).Shield,
            TempShield = GetUnitTempShield(unitEntity),
            MoveSpeed = GetUnitMoveSpeed(unitEntity),
            Position = GetUnitPosition(unitEntity),
            MaxActionPoints = GetUnitMaxActionPoints(unitEntity),
            ActionPoints = GetUnitActionPoints(unitEntity),
            CanMove = GetUnitCanMove(unitEntity),
            CanUseWeapons = GetUnitCanUseWeapons(unitEntity),
            CanRepairSelf = GetUnitCanRepairSelf(unitEntity),
            Entity = unitEntity,
            Rooms = GetUnitRoomInfos(unitEntity),
            Weapons = GetUnitWeapons(unitEntity),
        };
    }

    public static ControlledBy GetUnitControl(int unitEntity, EcsWorld world)
    {
        if (world.HasComponent<PlayerControlComponent>(unitEntity))
        {
            return ControlledBy.Player;
        }

        if (world.HasComponent<AiControlComponent>(unitEntity))
        {
            return ControlledBy.AI;
        }

        return ControlledBy.None;
    }

    private MechHealthComponent GetUnitHealth(int unitEntity)
    {
        var mechHealthComp = _config.World.GetComponent<MechHealthComponent>(unitEntity);
        return mechHealthComp;
    }

    private int? GetUnitTempShield(int unitEntity)
    {
        var tempShieldPool = _config.World.GetPool<TempShieldComponent>();
        return tempShieldPool.Has(unitEntity)
            ? tempShieldPool.Get(unitEntity).Amount
            : null;
    }

    private int GetUnitMoveSpeed(int unitEntity)
    {
        var activeCreatureComp = _config.World.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.MoveSpeed;
    }

    public Vector2Int GetUnitPosition(int unitEntity)
    {
        var positionComp = _config.World.GetComponent<PositionComponent>(unitEntity);
        return positionComp.Pos;
    }

    private int GetUnitMaxActionPoints(int unitEntity)
    {
        var activeCreatureComp = _config.World.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.MaxActionPoints;
    }

    private int GetUnitActionPoints(int unitEntity)
    {
        var activeCreatureComp = _config.World.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.ActionPoints;
    }

    private bool GetUnitCanMove(int unitEntity)
    {
        var unitActionPoints = GetUnitActionPoints(unitEntity);
        if (unitActionPoints < MoveOrdersExecutionSystem.MOVE_UNIT_ACTION_COST 
            || !IsUnitTurnNow(unitEntity))
        {
            return false;
        }

        if (_config.World.HasComponent<StunEffectComponent>(unitEntity))
        {
            return false;
        }

        var mechSystems = GetMechSystemTypes(unitEntity, _config.World);
        return MechSystemComponent.CanMechMove(mechSystems);
    }

    public static List<MechSystemType> GetMechSystemTypes(int unitEntity, EcsWorld world)
    {
        var systemPool = world.GetPool<MechSystemComponent>();
        var systemEntities = GetSystemEntities(unitEntity, world);
        var mechSystems = new List<MechSystemType>();
        foreach (var systemEntity in systemEntities)
        {
            if (!systemPool.Has(systemEntity))
            {
                continue;
            }

            var mechSystem = systemPool.Get(systemEntity);
            mechSystems.Add(mechSystem.Type);
        }

        return mechSystems;
    }

    private bool GetUnitCanUseWeapons(int unitEntity)
    {
        if (_config.World.HasComponent<StunEffectComponent>(unitEntity))
        {
            return false;
        }

        var unitActionPoints = GetUnitActionPoints(unitEntity);
        return unitActionPoints >= UseWeaponOrdersExecutionSystem.USE_WEAPON_ACTION_COST && IsUnitTurnNow(unitEntity);
    }

    private bool GetUnitCanRepairSelf(int unitEntity)
    {
        if (_config.World.HasComponent<StunEffectComponent>(unitEntity))
        {
            return false;
        }
        
        var unitActionPoints = GetUnitActionPoints(unitEntity);
        return unitActionPoints >= RepairSelfOrderExecutionSystem.REPAIR_ALL_ROOMS_ACTION_COST && IsUnitTurnNow(unitEntity);
    }

    private bool IsUnitTurnNow(int unitEntity)
    {
        var controlledBy = GetUnitControl(unitEntity, _config.World);
        switch (_config.TurnsManager.Phase)
        {
            case TurnsManager.TurnPhase.PlayerMove when controlledBy == ControlledBy.Player:
            case TurnsManager.TurnPhase.AIMove when controlledBy == ControlledBy.AI:
                return true;
            default:
                return false;
        }
    }

    private List<RoomInfo> GetUnitRoomInfos(int unitEntity)
    {
        var roomInfos = new List<RoomInfo>();
        var roomEntities = GetRoomEntities(unitEntity, _config.World);

        var roomsPool = _config.World.GetPool<MechRoomComponent>();
        var healthsPool = _config.World.GetPool<HealthComponent>();
        
        foreach (var roomEntity in roomEntities)
        {
            ref var roomComp = ref roomsPool.Get(roomEntity);
            ref var roomHealthComp = ref healthsPool.Get(roomEntity);

            var roomInfo = new RoomInfo
            {
                Entity = roomEntity,
                Health = roomHealthComp.Health,
                MaxHealth = roomHealthComp.MaxHealth,
                SystemType = roomComp.SystemType,
                SystemLvl = -1
            };
            roomInfos.Add(roomInfo);
        }

        return roomInfos;
    }

    public static List<int> GetRoomEntities(int unitEntity, EcsWorld world)
    {
        var roomFilter = world.Filter<MechRoomComponent>().End();
        var roomsPool = world.GetPool<MechRoomComponent>();
        var roomEntities = new List<int>();
        foreach (var roomEntity in roomFilter)
        {
            ref var roomComp = ref roomsPool.Get(roomEntity);
            if (!roomComp.MechEntity.TryUnpack(world, out var roomMechEntity))
            {
                continue;
            }

            if (unitEntity != roomMechEntity)
            {
                continue;
            }

            roomEntities.Add(roomEntity);
        }

        return roomEntities;
    }
    
    public static List<int> GetSystemEntities(int unitEntity, EcsWorld world)
    {
        var systemsFilter = world.Filter<MechSystemComponent>().End();
        var systemPool = world.GetPool<MechSystemComponent>();
        var systemEntities = new List<int>();
        foreach (var systemEntity in systemsFilter)
        {
            ref var systemComp = ref systemPool.Get(systemEntity);
            if (!systemComp.MechEntity.TryUnpack(world, out var roomMechEntity))
            {
                continue;
            }

            if (unitEntity != roomMechEntity)
            {
                continue;
            }

            systemEntities.Add(systemEntity);
        }

        return systemEntities;
    }

    private List<WeaponInfo> GetUnitWeapons(int unitEntity)
    {
        var weapons = new List<WeaponInfo>();
        
        var mechPool = _config.World.GetPool<MechComponent>();
        if (!mechPool.Has(unitEntity))
        {
            return weapons;
        }

        var unitWeaponIds = GetUnitWeaponIds(unitEntity, _config.World);
        foreach (var weaponId in unitWeaponIds)
        {
            if (TryGetWeaponInfo(unitEntity, weaponId, _config.World, out var weaponInfo))
            {
                weapons.Add(weaponInfo);
            }
        }

        return weapons;
    }

    public static List<string> GetUnitWeaponIds(int unitEntity, EcsWorld world)
    {
        var systemEntities = GetSystemEntities(unitEntity, world);
        var unitWeaponIds = new List<string>();
        var weaponSlotPool = world.GetPool<MechWeaponSlotComponent>();
        foreach (var systemEntity in systemEntities)
        {
            if (!weaponSlotPool.Has(systemEntity))
            {
                continue;
            }

            var weaponSlot = weaponSlotPool.Get(systemEntity);
            if (weaponSlot.WeaponId.IsNullOrEmpty())
            {
                continue;
            }

            unitWeaponIds.Add(weaponSlot.WeaponId);
        }

        return unitWeaponIds;
    }

    private bool TryGetWeaponInfo(int unitEntity, string weaponId, EcsWorld world,
        out WeaponInfo weaponInfo)
    {
        if (!UseWeaponOrdersExecutionSystem.TryGetWeaponEntity(weaponId, unitEntity, _config.World,
                out var weaponEntity))
        {
            Debug.LogError($"Unit {unitEntity} has no weapon {weaponId}");
            weaponInfo = default;
            return false;
        }

        var weaponMainComp = world.GetComponent<WeaponMainComponent>(weaponEntity);
        weaponInfo = new WeaponInfo
        {
            WeaponId = weaponId,
            WeaponTarget = weaponMainComp.TargetConfig,
            UseDistance = weaponMainComp.UseDistance,
            ProjectileType = weaponMainComp.ProjectileType,
            GripType = weaponMainComp.GripType,
            Cooldown = TryGetWeaponCooldown(weaponEntity, out var cd) 
                ? cd 
                : null,
            
            Stats = new Dictionary<string, object>()
        };
        weaponInfo.CanUse = GetCanUseWeapon(weaponInfo, weaponEntity);
        
        AddDamageInfo(ref weaponInfo, weaponEntity);
        AddPushInfo(ref weaponInfo, weaponEntity);
        AddStunInfo(ref weaponInfo, weaponEntity);
        AddDelayInfo(ref weaponInfo, weaponEntity);
        
        return true;
    }

    private bool TryGetWeaponCooldown(int weaponEntity, out int cd)
    {
        var cooldownPool = _config.World.GetPool<HasCooldownComponent>();
        if (!cooldownPool.Has(weaponEntity))
        {
            cd = default;
            return false;
        }

        var weaponCdComp = cooldownPool.Get(weaponEntity);
        var cooldownLastUntilTurn = weaponCdComp.LastUseTurn + weaponCdComp.Cooldown;
        cd = Math.Max(0, _config.TurnsManager.TurnIndex - cooldownLastUntilTurn);
        return true;
    }

    private bool GetCanUseWeapon(WeaponInfo weaponInfo, int weaponEntity)
    {
        return !weaponInfo.Cooldown.HasValue || weaponInfo.Cooldown.Value <= 0;
    }

    private void AddDamageInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var damageWeaponPool = _config.World.GetPool<DamageWeaponComponent>();
        if (damageWeaponPool.Has(weaponEntity))
        {
            weaponInfo.Stats.Add("Damage", damageWeaponPool.Get(weaponEntity).DamageAmount);
        }
    }

    private void AddPushInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var pushWeaponPool = _config.World.GetPool<PushWeaponComponent>();
        if (pushWeaponPool.Has(weaponEntity))
        {
            weaponInfo.Stats.Add("Push distance", pushWeaponPool.Get(weaponEntity).PushDistance);
        }
    }

    private void AddStunInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var stunWeaponPool = _config.World.GetPool<StunWeaponComponent>();
        if (stunWeaponPool.Has(weaponEntity))
        {
            weaponInfo.Stats.Add("Stun duration", stunWeaponPool.Get(weaponEntity).StunDuration);
        }
    }

    private void AddDelayInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var delayWeaponPool = _config.World.GetPool<DelayUsageWeaponComponent>();
        if (delayWeaponPool.Has(weaponEntity))
        {
            weaponInfo.Stats.Add("Usage delay", delayWeaponPool.Get(weaponEntity).DelayAmount);
        }
    }

    public bool TryGetUnitInPos(Vector2Int pos, out int unitEntity)
    {
        var positionPool = _config.World.GetPool<PositionComponent>();
        var positionEntities = _config.World.Filter<PositionComponent>().End();
        foreach (var positionCompEntity in positionEntities)
        {
            var positionComp = positionPool.Get(positionCompEntity);
            if (positionComp.Pos == pos)
            {
                unitEntity = positionCompEntity;
                return true;
            }
        }

        unitEntity = default;
        return false;
    }

    public void BuildMoveOrder(int unitEntity, Graph.Path path)
    {
        var moveSpeed = GetUnitMoveSpeed(unitEntity);
        
        Debug.Assert(path.Parts.Count <= moveSpeed);
        while (path.Parts.Count > moveSpeed)
        {
            path.Parts.RemoveAt(path.Parts.Count - 1);
        }

        EntitiesFactory.BuildMoveOrder(unitEntity, path, _config.World);
    }

    public void BuildUseWeaponOrder(int userUnitEntity, WeaponInfo usedWeaponInfo, InputWeaponTarget weaponTarget)
    {
        EntitiesFactory.BuildUseWeaponOrder(userUnitEntity, usedWeaponInfo, weaponTarget, _config.World);
    }

    public void BuildRepairSelfOrder(int unitEntity)
    {
        EntitiesFactory.BuildRepairSelfOrder(unitEntity, _config.World);
    }

    public static bool CanAttack(ControlledBy attackerSide, ControlledBy victimSide)
    {
        return attackerSide != victimSide;
    }
}