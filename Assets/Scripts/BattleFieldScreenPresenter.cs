using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleFieldScreenPresenter : UIBehaviour
{
    [SerializeField] private SelectedUnitPresenter selectedUnitPresenter;

    public event Action<BattleFieldManager.Tile, Vector2Int> FieldTileSelected;
    
    public void Init()
    {
        GlobalEventManager.BattleFieldGridTileSelected.Event += OnFieldGridTileSelected;
    }

    public void Setup()
    {
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