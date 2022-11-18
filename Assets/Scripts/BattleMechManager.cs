using System.Collections.Generic;
using Ecs.Components;
using Ecs.Components.Weapon;
using Ecs.Systems;
using Ext.LeoEcs;
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
        public int Entity;
        public bool CanMove;
        public bool CanUseWeapon;
        public List<RoomInfo> Rooms;
        public List<WeaponInfo> Weapons;
    }

    public struct WeaponInfo
    {
        public string WeaponId;
        public WeaponProjectileType ProjectileType;
        public WeaponGripType GripType;
        public int UseDistance;
        public int? Damage;
        public int? PushDistance;
        public int? StunDuration;
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
            MoveSpeed = GetUnitMoveSpeed(unitEntity),
            Position = GetUnitPosition(unitEntity),
            MaxActionPoints = GetUnitMaxActionPoints(unitEntity),
            ActionPoints = GetUnitActionPoints(unitEntity),
            CanMove = GetUnitCanMove(unitEntity),
            CanUseWeapon = GetUnitCanUseWeapon(unitEntity),
            Entity = unitEntity,
            Rooms = GetUnitRoomInfos(unitEntity),
            Weapons = GetUnitWeapons(unitEntity),
        };
    }

    public static ControlledBy GetUnitControl(int unitEntity, EcsWorld world)
    {
        var playerControlPool = world.GetPool<PlayerControlComponent>();
        if (playerControlPool.Has(unitEntity))
        {
            return ControlledBy.Player;
        }

        var aiControlPool = world.GetPool<AiControlComponent>();
        if (aiControlPool.Has(unitEntity))
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

    private int GetUnitMoveSpeed(int unitEntity)
    {
        var activeCreatureComp = _config.World.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.MoveSpeed;
    }

    private Vector2Int GetUnitPosition(int unitEntity)
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
        return unitActionPoints > 0 && IsUnitTurnNow(unitEntity);
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

    private bool GetUnitCanUseWeapon(int unitEntity)
    {
        var unitActionPoints = GetUnitActionPoints(unitEntity);
        return unitActionPoints > 0 && IsUnitTurnNow(unitEntity);
    }

    private List<RoomInfo> GetUnitRoomInfos(int unitEntity)
    {
        var roomInfos = new List<RoomInfo>();
        var roomFilter = _config.World.Filter<MechRoomComponent>().Inc<HealthComponent>().End();
        var roomsPool = _config.World.GetPool<MechRoomComponent>();
        var healthsPool = _config.World.GetPool<HealthComponent>();
        foreach (var roomEntity in roomFilter)
        {
            ref var roomComp = ref roomsPool.Get(roomEntity);
            if (!roomComp.MechEntity.TryUnpack(_config.World, out var roomMechEntity))
            {
                continue;
            }

            if (unitEntity != roomMechEntity)
            {
                continue;
            }

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

    private List<WeaponInfo> GetUnitWeapons(int unitEntity)
    {
        var weapons = new List<WeaponInfo>();
        
        var mechPool = _config.World.GetPool<MechComponent>();
        if (!mechPool.Has(unitEntity))
        {
            return weapons;
        }

        var unitMechComp = mechPool.Get(unitEntity);
        foreach (var weaponId in unitMechComp.WeaponIds)
        {
            if (TryGetWeaponInfo(unitEntity, weaponId, _config.World, out var weaponInfo))
            {
                weapons.Add(weaponInfo);
            }
        }

        return weapons;
    }

    private bool TryGetWeaponInfo(int unitEntity, string weaponId, EcsWorld world,
        out WeaponInfo weaponInfo)
    {
        if (!UseWeaponOrdersExecutionSystem.TryGetWeaponEntity(weaponId, _config.World,
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
            UseDistance = weaponMainComp.UseDistance,
            ProjectileType = weaponMainComp.ProjectileType,
            GripType = weaponMainComp.GripType,
        };
        AddDamageInfo(ref weaponInfo, weaponEntity);
        AddPushInfo(ref weaponInfo, weaponEntity);
        AddStunInfo(ref weaponInfo, weaponEntity);
        
        return true;
    }

    private void AddDamageInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var damageWeaponPool = _config.World.GetPool<DamageWeaponComponent>();
        weaponInfo.Damage = damageWeaponPool.Has(weaponEntity)
            ? damageWeaponPool.Get(weaponEntity).DamageAmount
            : null;
    }

    private void AddPushInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var pushWeaponPool = _config.World.GetPool<PushWeaponComponent>();
        weaponInfo.PushDistance = pushWeaponPool.Has(weaponEntity)
            ? pushWeaponPool.Get(weaponEntity).PushDistance
            : null;
    }

    private void AddStunInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var stunWeaponPool = _config.World.GetPool<StunWeaponComponent>();
        weaponInfo.StunDuration = stunWeaponPool.Has(weaponEntity)
            ? stunWeaponPool.Get(weaponEntity).StunDuration
            : null;
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

    public void BuildUseWeaponOrder(int userUnitEntity, WeaponInfo usedWeaponInfo, List<int> targetRooms)
    {
        EntitiesFactory.BuildUseWeaponOrder(userUnitEntity, usedWeaponInfo, targetRooms, _config.World);
    }

    public static bool CanAttack(ControlledBy attackerSide, ControlledBy victimSide)
    {
        return attackerSide != victimSide;
    }
}