using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class GameConfig
    {
        public int BattleFieldSize;
        [TextArea] public string BattleFieldConfig;
        public BattleFieldManager.Tile[] TileConfigs;
    }
    
    [SerializeField] private EcsManager ecsManager;
    [SerializeField] private UIManager uiManager;
    [Space]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private List<EntitiesFactory.MechConfig> mechConfigs;

    public BattleManager BattleManager { get; private set; }
    public GameConfig Config => gameConfig;

    private void Start()
    {
        BattleManager = new BattleManager(new BattleManager.Config
        {
            World = ecsManager.World,
            FieldSize = gameConfig.BattleFieldSize,
            FieldConfig = gameConfig.BattleFieldConfig,
            TileConfigs = gameConfig.TileConfigs,
        });
        foreach (var mechConfig in mechConfigs)
        {
            EntitiesFactory.BuildMechEntity(ecsManager.World, mechConfig);
        }
        
        uiManager.Init(gameConfig, BattleManager);
    }
}