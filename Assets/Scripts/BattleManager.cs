using System.Collections.Generic;
using Leopotam.EcsLite;

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

    public void Next()
    {
        switch (Phase)
        {
            case TurnPhase.PlayerMove:
                Phase = TurnPhase.AIMove;
                break;
            case TurnPhase.AIMove:
                Phase = TurnPhase.PlayerMove;
                TurnIndex++;
                break;
        }
    }

    public BattleFieldManager.Tile[] GetField()
    {
        return _fieldManager.GetTiles();
    }

    public List<BattleMechManager.BattleUnitInfo> GetPlayerUnitInfos()
    {
        return _mechManager.GetPlayerUnitInfos();
    }
}