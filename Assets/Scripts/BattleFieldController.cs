using System.Collections.Generic;
using System.Linq;
using Ext;
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
        public BattleManager BattleManager;
    }

    private readonly Dictionary<Vector2Int, FieldTileController> _tiles = new();
    private readonly Dictionary<int, BattleUnitController> _units = new();
    private Config _config;

    private FieldTileController _lastSelected;
    private Vector2Int? _activeMoveOrderUnitPos;
    private List<Vector2Int> _highlightedTiles = new();

    public void Init()
    {
        GlobalEventManager.BattleField.GridTileSelected.Event += OnFieldGridTileSelected;
        GlobalEventManager.BattleField.UnitMoveOrderSetActive.Event += OnUnitMoveOrderSetActive;
        GlobalEventManager.BattleField.GridTileHovered.Event += OnFieldGridTileHovered;
        GlobalEventManager.BattleField.UnitMoved.Event += OnUnitMoved;
        GlobalEventManager.BattleField.UseWeaponBtnClicked.Event += OnUseWeaponBtnClicked;
        GlobalEventManager.Turns.TurnUpdated.Event += OnTurnUpdated;
    }

    public void Setup(Config config)
    {
        _config = config;

        AddTiles();
        SetupTiles();

        SetupUnits();

        SetupCamera();
    }

    private void OnFieldGridTileSelected(BattleFieldManager.Tile tile, Vector2Int tilePos)
    {
        if (_activeMoveOrderUnitPos.HasValue)
        {
            BuildMoveOrder(tilePos);
            return;
        }

        var selectedTile = GetTileController(tilePos);

        if (_lastSelected)
        {
            _lastSelected.SetSelected(false);
        }

        selectedTile.SetSelected(true);
        _lastSelected = selectedTile;
    }

    private void BuildMoveOrder(Vector2Int destPos)
    {
        if (!_activeMoveOrderUnitPos.HasValue)
        {
            Debug.LogError("Chto za cert!");
            return;
        }

        if (!_config.BattleManager.TryGetPath(_activeMoveOrderUnitPos.Value, destPos, out var path))
        {
            Debug.LogError($"Can't find path between {_activeMoveOrderUnitPos.Value} and {destPos}");
            return;
        }

        if (!_config.BattleManager.TryGetUnitInPos(_activeMoveOrderUnitPos.Value, out int unitEntity))
        {
            Debug.LogError($"Can't find unit in pos {_activeMoveOrderUnitPos.Value}");
            return;
        }

        var unitController = GetUnitController(unitEntity);
        if (path.Parts.Count > unitController.UnitInfo.MoveSpeed)
        {
            return;
        }

        _config.BattleManager.BuildMoveOrder(unitEntity, path);
        _activeMoveOrderUnitPos = null;
    }

    private void OnUnitMoveOrderSetActive(Vector2Int unitPos, bool isActive)
    {
        ClearHighlightedTiles();
        if (!isActive)
        {
            _activeMoveOrderUnitPos = null;
            return;
        }

        _activeMoveOrderUnitPos = unitPos;
    }

    private void OnFieldGridTileHovered(BattleFieldManager.Tile tile, Vector2Int tilePos)
    {
        if (!_activeMoveOrderUnitPos.HasValue)
        {
            return;
        }
        
        HighlightPath(tilePos);
    }

    private void OnUnitMoved(int unitEntity, Vector2Int srcPos, Vector2Int destPos)
    {
        UpdateUnit(unitEntity);
    }

    private void OnUseWeaponBtnClicked(int unitEntity, BattleMechManager.WeaponInfo weaponInfo)
    {
        _activeMoveOrderUnitPos = null;
        HighlightWeaponDistanceArea(unitEntity, weaponInfo);
    }

    private void HighlightWeaponDistanceArea(int unitEntity, BattleMechManager.WeaponInfo weaponInfo)
    {
        ClearHighlightedTiles();
        
        var unitController = GetUnitController(unitEntity);
        var weaponPos = unitController.UnitInfo.Position;

        var weaponUseDistance = weaponInfo.UseDistance;
        for (int i = -weaponInfo.UseDistance; i <= weaponInfo.UseDistance; i++)
        {
            for (int j = -weaponUseDistance; j <= weaponUseDistance; j++)
            {
                var posDiff = new Vector2Int(i, j);
                var posToCheck = weaponPos + posDiff;
                if (!BattleFieldManager.IsValidFieldPos(posToCheck, _config.BattleManager.FieldSize))
                {
                    continue;
                }

                if (posDiff.magnitude > weaponUseDistance)
                {
                    continue;
                }

                var tileController = GetTileController(posToCheck);
                tileController.SetHighlighted(true);
                _highlightedTiles.Add(posToCheck);
            }
        }
    }

    private void OnTurnUpdated(int newTurnIndex, TurnsManager.TurnPhase turnPhase)
    {
        SetupUnits();
    }

    private void UpdateUnit(int unitEntity)
    {
        var unitController = GetUnitController(unitEntity);
        var unitInfo = _config.BattleManager.GetBattleUnitInfo(unitEntity);
        unitController.Setup(unitInfo, GetUnitColor(unitInfo));

        var unitPos = unitInfo.Position;
        var unitTile = GetTileController(unitPos);
        unitTile.SetupContent(unitController.transform);
    }

    private void AddTiles()
    {
        for (var tileIndex = 0; tileIndex < _config.BattleManager.GetField().Length; tileIndex++)
        {
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileIndex, _config.BattleManager.FieldSize);
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
        var field = _config.BattleManager.GetField();
        for (var tileIndex = 0; tileIndex < field.Length; tileIndex++)
        {
            var currTile = field[tileIndex];
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileIndex, _config.BattleManager.FieldSize);
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
        Debug.Assert(BattleFieldManager.IsValidFieldPos(tilePos, _config.BattleManager.FieldSize));

        return _tiles[tilePos];
    }

    private BattleUnitController GetUnitController(int unitEntity)
    {
        return _units[unitEntity];
    }

    private Color GetUnitColor(BattleMechManager.BattleUnitInfo unitInfo)
    {
        if (!unitControlColors.TryGetValue(unitInfo.ControlledBy, out var unitColor))
        {
            Debug.LogError($"Can't find unit color for unit control  {unitInfo.ControlledBy}");
            unitColor = Color.magenta;
        }

        return unitColor;
    }

    private void SetupUnits()
    {
        var unitInfos = _config.BattleManager.GetPlayerUnitInfos();
        foreach (var unitInfo in unitInfos)
        {
            if (!_units.TryGetValue(unitInfo.Entity, out var unitController))
            {
                unitController = Instantiate(unitPrefab);
                _units.Add(unitInfo.Entity, unitController);
            }

            UpdateUnit(unitInfo.Entity);
        }
    }

    private void SetupCamera()
    {
        mainCam.transform.position = new Vector3(
            (_config.BattleManager.FieldSize - 1) * tileSize / 2f,
            -(_config.BattleManager.FieldSize - 1) * tileSize / 2f * (1 + cameraScreenShareCoeff - 0.5f),
            mainCam.transform.position.z
        );
        mainCam.orthographicSize = _config.BattleManager.FieldSize * tileSize / 2f * (1 + cameraScreenShareCoeff - 0.5f);
    }

    private void HighlightPath(Vector2Int destPos)
    {
        ClearHighlightedTiles();

        if (!_activeMoveOrderUnitPos.HasValue)
        {
            return;
        }
        
        if (!_config.BattleManager.TryGetPath(_activeMoveOrderUnitPos.Value, destPos, out var path))
        {
            return;
        }

        if (!_config.BattleManager.TryGetUnitInPos(_activeMoveOrderUnitPos.Value, out int unitEntity))
        {
            return;
        }

        var unitController = GetUnitController(unitEntity);
        if (path.Parts.Count > unitController.UnitInfo.MoveSpeed)
        {
            return;
        }
            
        foreach (var pathPart in path.Parts)
        {
            var tilePos = pathPart.Node.Position.ToV2I();
            var tileControllerToHighlight = GetTileController(tilePos);
            tileControllerToHighlight.SetHighlighted(true);
        }

        _highlightedTiles = path.Parts.Select(pathPart => pathPart.Node.Position.ToV2I()).ToList();
    }

    private void ClearHighlightedTiles()
    {
        foreach (var tilePos in _highlightedTiles)
        {
            var tileControllerToHighlight = GetTileController(tilePos);
            tileControllerToHighlight.SetHighlighted(false);
        }

        _highlightedTiles.Clear();
    }
}