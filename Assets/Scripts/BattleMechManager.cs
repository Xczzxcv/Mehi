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
        public EcsWorld World { get; set; }
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
        public List<RoomInfo> Rooms;
        public List<WeaponInfo> Weapons;
    }

    public struct WeaponInfo
    {
        public string WeaponId;
        public int? Damage;
        public int? PushDistance;
        public int? StunDuration;
    }

    public struct RoomInfo
    {
        public int Health;
        public int MaxHealth;
        public MechSystemType SystemType;
    }

    private readonly EcsWorld _world;

    public BattleMechManager(Config config)
    {
        _world = config.World;
    }

    public List<BattleUnitInfo> GetUnitInfos()
    {
        var resultList = new List<BattleUnitInfo>();
        var activeUnitsFilter = _world
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
            ControlledBy = GetUnitControl(unitEntity),
            MaxHealth = GetUnitHealth(unitEntity).MaxHealth,
            Health = GetUnitHealth(unitEntity).Health,
            Shield = GetUnitHealth(unitEntity).Shield,
            MoveSpeed = GetUnitMoveSpeed(unitEntity),
            Position = GetUnitPosition(unitEntity),
            MaxActionPoints = GetUnitMaxActionPoints(unitEntity),
            ActionPoints = GetUnitActionPoints(unitEntity),
            Rooms = GetUnitRoomInfos(unitEntity),
            Weapons = GetUnitWeapons(unitEntity),
        };
    }

    private ControlledBy GetUnitControl(int unitEntity)
    {
        var playerControlPool = _world.GetPool<PlayerControlComponent>();
        if (playerControlPool.Has(unitEntity))
        {
            return ControlledBy.Player;
        }

        var aiControlPool = _world.GetPool<PlayerControlComponent>();
        if (aiControlPool.Has(unitEntity))
        {
            return ControlledBy.AI;
        }

        return ControlledBy.None;
    }

    private MechHealthComponent GetUnitHealth(int unitEntity)
    {
        var mechHealthComp = _world.GetComponent<MechHealthComponent>(unitEntity);
        return mechHealthComp;
    }

    private int GetUnitMoveSpeed(int unitEntity)
    {
        var activeCreatureComp = _world.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.MoveSpeed;
    }

    private Vector2Int GetUnitPosition(int unitEntity)
    {
        var positionComp = _world.GetComponent<PositionComponent>(unitEntity);
        return positionComp.Pos;
    }

    private int GetUnitMaxActionPoints(int unitEntity)
    {
        var activeCreatureComp = _world.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.MaxActionPoints;
    }

    private int GetUnitActionPoints(int unitEntity)
    {
        var activeCreatureComp = _world.GetComponent<ActiveCreatureComponent>(unitEntity);
        return activeCreatureComp.ActionPoints;
    }

    private List<RoomInfo> GetUnitRoomInfos(int unitEntity)
    {
        var roomInfos = new List<RoomInfo>();
        var roomFilter = _world.Filter<MechRoomComponent>().Inc<HealthComponent>().End();
        var roomsPool = _world.GetPool<MechRoomComponent>();
        var healthsPool = _world.GetPool<HealthComponent>();
        foreach (var roomEntity in roomFilter)
        {
            ref var roomComp = ref roomsPool.Get(roomEntity);
            if (!roomComp.MechEntity.TryUnpack(_world, out var roomMechEntity))
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
                Health = roomHealthComp.Health,
                SystemType = roomComp.SystemType,
            };
            roomInfos.Add(roomInfo);
        }

        return roomInfos;
    }

    private List<WeaponInfo> GetUnitWeapons(int unitEntity)
    {
        var weapons = new List<WeaponInfo>();
        
        var mechPool = _world.GetPool<MechComponent>();
        if (!mechPool.Has(unitEntity))
        {
            return weapons;
        }

        var unitMechComp = mechPool.Get(unitEntity);

        foreach (var weaponId in unitMechComp.WeaponIds)
        {
            if (!UseWeaponOrdersExecutionSystem.TryGetWeaponEntity(weaponId, _world, 
                    out var weaponEntity))
            {
                Debug.LogError($"Unit {unitEntity} has no weapon {weaponId}");
                continue;
            }

            var weaponInfo = new WeaponInfo
            {
                WeaponId = weaponId
            };
            AddDamageInfo(ref weaponInfo, weaponEntity);
            AddPushInfo(ref weaponInfo, weaponEntity);
            AddStunInfo(ref weaponInfo, weaponEntity);
            
            weapons.Add(weaponInfo);
        }

        return weapons;
    }

    private void AddDamageInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var damageWeaponPool = _world.GetPool<DamageWeaponComponent>();
        weaponInfo.Damage = damageWeaponPool.Has(weaponEntity)
            ? damageWeaponPool.Get(weaponEntity).DamageAmount
            : null;
    }

    private void AddPushInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var pushWeaponPool = _world.GetPool<PushWeaponComponent>();
        weaponInfo.PushDistance = pushWeaponPool.Has(weaponEntity)
            ? pushWeaponPool.Get(weaponEntity).PushDistance
            : null;
    }

    private void AddStunInfo(ref WeaponInfo weaponInfo, int weaponEntity)
    {
        var stunWeaponPool = _world.GetPool<StunWeaponComponent>();
        weaponInfo.StunDuration = stunWeaponPool.Has(weaponEntity)
            ? stunWeaponPool.Get(weaponEntity).StunDuration
            : null;
    }

    public bool TryGetUnitInPos(int x, int y, out int unitEntity)
    {
        var posToCheck = new Vector2Int(x, y);
        var positionPool = _world.GetPool<PositionComponent>();
        var positionEntities = _world.Filter<PositionComponent>().End();
        foreach (var positionCompEntity in positionEntities)
        {
            var positionComp = positionPool.Get(positionCompEntity);
            if (positionComp.Pos == posToCheck)
            {
                unitEntity = positionCompEntity;
                return true;
            }
        }

        unitEntity = default;
        return false;
    }
}