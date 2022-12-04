using System.Collections.Generic;
using Ecs.Components;
using Leopotam.EcsLite;
using UnityEngine;

public class BattleManager
{
    public struct Config
    {
        public int FieldSize;
        public EcsWorld World;
        public string FieldConfig;
        public BattleFieldManager.Tile[] TileConfigs;
    }

    private readonly Config _config;
    private readonly BattleFieldManager _fieldManager;
    private readonly BattleMechManager _mechManager;
    private readonly TurnsManager _turnsManager;

    public int FieldSize => _fieldManager.FieldSize;
    public int TurnIndex => _turnsManager.TurnIndex;
    public TurnsManager.TurnPhase TurnPhase => _turnsManager.Phase;

    public BattleManager(Config config)
    {
        _config = config;

        _turnsManager = new TurnsManager(new TurnsManager.Config
        {
        });

        _fieldManager = new BattleFieldManager(new BattleFieldManager.Config
        {
            Size = _config.FieldSize,
            FieldConfig = _config.FieldConfig,
            TileConfigs = _config.TileConfigs,
        });

        _mechManager = new BattleMechManager(new BattleMechManager.Config
        {
            World = _config.World,
            TurnsManager = _turnsManager,
        });
    }

    public void NextPhase()
    {
        _turnsManager.NextPhase();
    }

    public BattleFieldManager.Tile[] GetField()
    {
        return _fieldManager.GetTiles();
    }

    public List<BattleMechManager.BattleUnitInfo> GetPlayerUnitInfos()
    {
        return _mechManager.GetUnitInfos();
    }

    public BattleFieldManager.Tile GetFieldTile(Vector2Int tilePos)
    {
        return _fieldManager.GetTile(tilePos);
    }

    public BattleMechManager.BattleUnitInfo GetBattleUnitInfo(int unitEntity)
    {
        return _mechManager.GetUnitInfo(unitEntity);
    }

    public bool TryGetUnitInPos(Vector2Int pos, out int unitEntity)
    {
        return _mechManager.TryGetUnitInPos(pos, out unitEntity);
    }

    public bool TryGetPath(Vector2Int src, Vector2Int dest, out Graph.Path path)
    {
        return _fieldManager.TryGetPath(src, dest, out path);
    }

    public void BuildMoveOrder(int unitEntity, Graph.Path path)
    {
        _mechManager.BuildMoveOrder(unitEntity, path);
    }

    public void BuildUseWeaponOrder(int userUnitEntity, BattleMechManager.WeaponInfo usedWeaponInfo,
        InputWeaponTarget weaponTarget)
    {
        _mechManager.BuildUseWeaponOrder(userUnitEntity, usedWeaponInfo, weaponTarget);
    }

    public void BuildRepairSelfOrder(int unitEntity)
    {
        _mechManager.BuildRepairSelfOrder(unitEntity);
    }
}