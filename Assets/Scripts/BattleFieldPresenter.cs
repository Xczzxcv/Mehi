using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleFieldPresenter : UIBehaviour
{
    [SerializeField] private GridLayoutGroup layout;
    [SerializeField] private ToggleGroup fieldToggleGroup;
    [SerializeField] private BattleFieldTilePresenter tilePrefab;
    [SerializeField] private SerializedDictionary<BattleFieldManager.TileType, Sprite> backgrounds;
    [Space]
    [SerializeField] private BattleUnitPresenter unitPrefab;
    [SerializeField] private SerializedDictionary<BattleMechManager.ControlledBy, Color> unitControlColors;

    public struct ViewInfo
    {
        public int FieldSize;
        public BattleFieldManager.Tile[] Field;
        public List<BattleMechManager.BattleUnitInfo> UnitInfos;
    }

    private readonly List<BattleFieldTilePresenter> _tiles = new();
    private ViewInfo _view;

    public void Setup(ViewInfo view)
    {
        _view = view;

        layout.constraintCount = _view.FieldSize;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        
        AddTiles();
        SetupTiles();

        SetupUnits();
    }

    private void AddTiles()
    {
        for (var i = 0; i < _view.Field.Length; i++)
        {
            var tilePresenter = Instantiate(tilePrefab, layout.transform);
            tilePresenter.Init(fieldToggleGroup);
            _tiles.Add(tilePresenter);
        }
    }

    private void SetupTiles()
    {
        for (var tileIndex = 0; tileIndex < _view.Field.Length; tileIndex++)
        {
            var currTileInfo = _view.Field[tileIndex];
            var tilePos = BattleFieldManager.GetPositionFromIndex(tileIndex, _view.FieldSize);
            var currTile = GetTilePresenter(tilePos);
            if (!backgrounds.TryGetValue(currTileInfo.Type, out var backgroundSprite))
            {
                Debug.LogError($"Can't find background for tile {currTileInfo.Type} ({tileIndex})");
                continue;
            }

            currTile.Setup(backgroundSprite, null);
        }
    }

    private BattleFieldTilePresenter GetTilePresenter(Vector2Int tilePos)
    {
        var tileIndex = tilePos.y * _view.FieldSize + tilePos.x;
        Debug.Assert(tileIndex >= 0 && tileIndex < _tiles.Count);

        return _tiles[tileIndex];
    }

    private void SetupUnits()
    {
        foreach (var unitInfo in _view.UnitInfos)
        {
            var unitPresenter = Instantiate(unitPrefab);
            if (!unitControlColors.TryGetValue(unitInfo.ControlledBy, out var unitColor))
            {
                Debug.LogError($"Can't find unit color for unit control  {unitInfo.ControlledBy}");
                unitColor = Color.magenta;
            }
            unitPresenter.Setup(unitInfo, unitColor);
            
            var unitPos = unitInfo.Position;
            var tileIndex = BattleFieldManager.GetIndexFromPosition(unitPos, _view.FieldSize);
            var tilePresenter = _tiles[tileIndex];
            tilePresenter.SetupContent(unitPresenter.transform);
        }
    }
}