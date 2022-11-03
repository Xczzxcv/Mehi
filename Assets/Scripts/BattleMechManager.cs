using System.Collections.Generic;
using Ecs.Components;
using Ext.LeoEcs;
using Leopotam.EcsLite;
using UnityEngine;

public class BattleMechManager
{
    public struct Config
    {
        public EcsWorld World { get; set; }
    }
    
    public struct BattleUnitInfo
    {
        public int ActionPoints;
        public Vector2Int Position;
        public int MoveSpeed;
        public int Shield;
        public List<RoomInfo> Rooms;
        public List<WeaponInfo> Weapons;
    }

    public struct WeaponInfo
    {
        public int Damage;
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

    public List<BattleUnitInfo> GetPlayerUnitInfos()
    {
        var resultList = new List<BattleUnitInfo>();
        var playerControlledUnitsFilter = _world
            .Filter<ActiveCreatureComponent>()
            .Inc<PlayerControlComponent>()
            .End();
        foreach (var playerUnitEntity in playerControlledUnitsFilter)
        {
            var playerUnitInfo = GetUnitInfo(playerUnitEntity);
            resultList.Add(playerUnitInfo);
        }

        return resultList;
    }

    private BattleUnitInfo GetUnitInfo(int unitEntity)
    {
        return new BattleUnitInfo
        {
            Shield = GetUnitShield(unitEntity),
            MoveSpeed = GetUnitMoveSpeed(unitEntity),
            Position = GetUnitPosition(unitEntity),
            ActionPoints = GetUnitActionPoints(unitEntity),
            Rooms = GetUnitRoomInfos(unitEntity),
            Weapons = new List<WeaponInfo>
            {
                new WeaponInfo
                {
                    Damage = 1
                }
            },
        };
    }

    private int GetUnitShield(int unitEntity)
    {
        var mechHealthComp = _world.GetComponent<MechHealthComponent>(unitEntity);
        return mechHealthComp.Shield;
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
            var roomComp = roomsPool.Get(roomEntity);
            if (!roomComp.MechEntity.Unpack(_world, out var roomMechEntity))
            {
                continue;
            }

            if (unitEntity != roomMechEntity)
            {
                continue;
            }

            var roomHealthComp = healthsPool.Get(roomEntity);

            var roomInfo = new RoomInfo
            {
                Health = roomHealthComp.Health,
                SystemType = roomComp.SystemType,
            };
            roomInfos.Add(roomInfo);
        }

        return roomInfos;
    }
}