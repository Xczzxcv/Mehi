using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleFieldPresenter : UIBehaviour
{
    [SerializeField] private BattleFieldGridPresenter fieldGridPresenter;
    [SerializeField] private SelectedUnitPresenter selectedUnitPresenter;

    public struct ViewInfo
    {
        public BattleFieldGridPresenter.ViewInfo FieldGridView;
        public SelectedUnitPresenter.ViewInfo SelectedUnitView;
    }

    public event Action<BattleFieldManager.Tile, Vector2Int> FieldTileSelected;
    
    public void Init()
    {
        fieldGridPresenter.TileSelected += OnFieldGridTileSelected;
    }

    public void Setup(ViewInfo viewInfo)
    {
        fieldGridPresenter.Setup(viewInfo.FieldGridView);
        selectedUnitPresenter.Setup(SelectedUnitPresenter.ViewInfo.BuildEmpty());
    }

    public void UpdateSelectedUnit(SelectedUnitPresenter.ViewInfo unitInfo)
    {
        selectedUnitPresenter.Setup(unitInfo);
    }

    private void OnFieldGridTileSelected(BattleFieldManager.Tile selectedTile, Vector2Int tilePos)
    {
        FieldTileSelected?.Invoke(selectedTile, tilePos);
    }
}