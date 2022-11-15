﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EcsManager ecsManager;
    [SerializeField] private BattleFieldController battleFieldController;
    [SerializeField] private UIManager uiManager;
    [Space]
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private List<EntitiesFactory.MechConfig> mechConfigs;
    [SerializeField] private List<WeaponConfig> weaponConfigs;

    [Serializable]
    public class GameConfig
    {
        public int BattleFieldSize;
        [TextArea] public string BattleFieldConfig;
        public BattleFieldManager.Tile[] TileConfigs;
    }

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

        battleFieldController.Init();
        var hasPath = BattleManager.TryGetPath(
            new Vector2Int(2, 2),
            new Vector2Int(0, 0),
            out var path
        );
        var pathString = hasPath
            ? $"{string.Join(", ", path.Parts.Select(part => Vector2Int.RoundToInt(part.Node.Position)))}"
            : string.Empty;
        Debug.Log($"hasPath: {hasPath} path: {pathString}");
        ecsManager.Setup();
        foreach (var weaponConfig in weaponConfigs)
        {
            EntitiesFactory.BuildWeapon(ecsManager.World, weaponConfig);
        }

        foreach (var mechConfig in mechConfigs)
        {
            EntitiesFactory.BuildMechEntity(ecsManager.World, mechConfig);
        }

        battleFieldController.Setup(new BattleFieldController.Config
        {
            FieldSize = gameConfig.BattleFieldSize,
            Field = BattleManager.GetField(),
            UnitInfos = BattleManager.GetPlayerUnitInfos(),
        });

        uiManager.Init(BattleManager);
    }
}