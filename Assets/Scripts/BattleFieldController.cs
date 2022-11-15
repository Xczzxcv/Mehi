using System.Collections.Generic;
using UnityEngine;

public class BattleFieldController : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    [SerializeField] private float cameraScreenShareCoeff;
    [Space]
    [SerializeField] private FieldTileController tilePrefab;
    [SerializeField] private Transform tilesParent;
    [SerializeField] private float tileSize;
    [SerializeField] private SerializedDictionary<BattleFieldManager.TileType, Sprite> backgrounds;
    [Space]
    [SerializeField] private BattleUnitController unitPrefab;
    [SerializeField] private SerializedDictionary<BattleMechManager.ControlledBy, Color> unitControlColors;

    public struct Config
    {
        public int FieldSize;
        public BattleFieldManager.Tile[] Field;
        public List<BattleMechManager.BattleUnitInfo> UnitInfos;
    }

    private readonly Dictionary<Vector2Int, FieldTileController> _tiles = new();
    private Config _config;

    private FieldTileController _lastSelected;

    public void Init()
    {
        GlobalEventManager.BattleFieldGridTileSelected.Event += OnFieldGridTileSelected;
    }

    private void OnFieldGridTileSelected(BattleFieldManager.Tile tile, Vector2Int tilePos)
    {
        var selectedTile = GetTileController(tilePos);

        if (_lastSelected)
        {
            _lastSelected.SetSelected(false);
        }

        selectedTile.SetSelected(true);
        _lastSelected = selectedTile;
    }

    public void Setup(Config config)
    {
        _config = config;

        AddTiles();
        SetupTiles();

        SetupUnits();

        SetupCamera();
    }

    private void AddTiles()
    {
        for (var tileIndex = 0; tileIndex < _config.Field.Length; tileIndex++)
        {
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileIndex, _config.FieldSize);
            var tileObjectPos = new Vector3(
                tilePos.x * tileSize,
                -tilePos.y * tileSize,
                0
            );
            var tileController = Instantiate(
                tilePrefab,
                tileObjectPos,
                Quaternion.identity,
                tilesParent
            );
            _tiles.Add(tilePos, tileController);
        }
    }

    private void SetupTiles()
    {
        for (var tileIndex = 0; tileIndex < _config.Field.Length; tileIndex++)
        {
            var currTile = _config.Field[tileIndex];
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileIndex, _config.FieldSize);
            var currTileController = GetTileController(tilePos);
            if (!backgrounds.TryGetValue(currTile.Type, out var backgroundSprite))
            {
                Debug.LogError($"Can't find background for tile {currTile.Type} ({tileIndex})");
                continue;
            }

            currTileController.Setup(new FieldTileController.Config
            {
                Tile = currTile,
                TilePosition = tilePos,
                Background = backgroundSprite,
                TileSize = tileSize,
            });
        }
    }

    private FieldTileController GetTileController(Vector2Int tilePos)
    {
        Debug.Assert(BattleFieldManager.IsValidFieldPos(tilePos, _config.FieldSize));

        return _tiles[tilePos];
    }

    private void SetupUnits()
    {
        foreach (var unitInfo in _config.UnitInfos)
        {
            var unitController = Instantiate(unitPrefab);
            if (!unitControlColors.TryGetValue(unitInfo.ControlledBy, out var unitColor))
            {
                Debug.LogError($"Can't find unit color for unit control  {unitInfo.ControlledBy}");
                unitColor = Color.magenta;
            }

            unitController.Setup(unitInfo, unitColor);
            
            var unitPos = unitInfo.Position;
            var unitTile = GetTileController(unitPos);
            unitTile.SetupContent(unitController.transform);
        }
    }

    private void SetupCamera()
    {
        mainCam.transform.position = new Vector3(
            (_config.FieldSize - 1) * tileSize / 2f,
            -(_config.FieldSize - 1) * tileSize / 2f * (1 + cameraScreenShareCoeff - 0.5f),
            mainCam.transform.position.z
        );
        mainCam.orthographicSize = _config.FieldSize * tileSize / 2f * (1 + cameraScreenShareCoeff - 0.5f);
    }
}