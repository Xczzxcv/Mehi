using System.Collections.Generic;
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
    
    private readonly BattleFieldManager _fieldManager;
    private readonly BattleMechManager _mechManager;
    
    public int TurnIndex { get; private set; }
    public TurnPhase Phase { get; private set; }
    public int FieldSize => _fieldManager.FieldSize;

    public const TurnPhase INIT_PHASE = TurnPhase.PlayerMove; 
    public const TurnPhase END_PHASE = TurnPhase.AIMove; 

    public enum TurnPhase
    {
        PlayerMove,
        AIMove,
    }

    public BattleManager(Config config)
    {
        _fieldManager = new BattleFieldManager(new BattleFieldManager.Config
        {
            Size = config.FieldSize,
            FieldConfig = config.FieldConfig,
            TileConfigs = config.TileConfigs,
        });
        _mechManager = new BattleMechManager(new BattleMechManager.Config
        {
            World = config.World
        });
    }

    public void NextTurn()
    {
        TurnIndex++;
        Phase = INIT_PHASE;
    }

    public void NextPhase()
    {
        if (Phase == INIT_PHASE)
        {
            Phase = END_PHASE;
        }
        else
        {
            Debug.LogError("Wrong time, FOOL!");
        }
    }

    public BattleFieldManager.Tile[] GetField()
    {
        return _fieldManager.GetTiles();
    }

    public List<BattleMechManager.BattleUnitInfo> GetPlayerUnitInfos()
    {
        return _mechManager.GetUnitInfos();
    }
}