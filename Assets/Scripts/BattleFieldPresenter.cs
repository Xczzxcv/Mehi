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

    public struct ViewInfo
    {
        public int FieldSize;
        public BattleFieldManager.Tile[] Field;
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
}