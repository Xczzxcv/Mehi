using UnityEngine;

internal class BattleFieldScreenController : MonoBehaviour
{
    [SerializeField] private BattleFieldScreenPresenter fieldScreenPrefab;
    
    private Transform _uiRoot;
    private BattleManager _battleManager;
    private BattleFieldScreenPresenter _battleFieldPresenter;
    private UseWeaponManager _useWeaponManager;

    private Vector2Int? _selectedTilePos;

    public void Init(Transform uiRoot, BattleManager battleManager)
    {
        _uiRoot = uiRoot;
        _battleManager = battleManager;
        _useWeaponManager = new UseWeaponManager(_battleManager);
        _useWeaponManager.Init();
        
        GlobalEventManager.BattleField.GridTileSelected.Event += OnBattleFieldTileSelected;
        GlobalEventManager.BattleField.UnitUpdated.Event += OnUnitUpdated;
        GlobalEventManager.Turns.TurnUpdated.Event += OnTurnUpdated;
        GlobalEventManager.BattleField.RoomSelectedAsWeaponTarget.Event += OnRoomSelectedAsWeaponTarget;
        GlobalEventManager.BattleField.UnitSelectedAsWeaponTarget.Event += OnUnitSelectedAsWeaponTarget;
        GlobalEventManager.BattleField.TileSelectedAsWeaponTarget.Event += OnTileSelectedAsWeaponTarget;
    }

    public void Setup()
    {
        _battleFieldPresenter = Instantiate(fieldScreenPrefab, _uiRoot);
        _battleFieldPresenter.Init();
        _battleFieldPresenter.Setup(new BattleFieldScreenPresenter.ViewInfo
        {
            TurnIndex = _battleManager.TurnIndex,
            TurnPhase = _battleManager.TurnPhase,
        });
        
        _battleFieldPresenter.NextTurnPhaseBtnClicked += OnPresenterNextTurnPhaseBtnClicked;
        _battleFieldPresenter.TargetsChoiceConfirmed += OnPresenterTargetsChoiceConfirmed;
        _battleFieldPresenter.RepairButtonClick += OnPresenterRepairButtonClick;
        _battleFieldPresenter.RoomClicked += OnPresenterRoomClicked;
    }

    private void OnPresenterNextTurnPhaseBtnClicked()
    {
        _battleManager.NextPhase();
    }

    private void OnPresenterTargetsChoiceConfirmed()
    {
        _useWeaponManager.BuildUseWeaponOrder();
        _battleFieldPresenter.UpdateSelectedUnit(SelectedUnitPresenter.ViewInfo.BuildEmpty());
        _battleFieldPresenter.UpdateRoomsInfo(RoomListPresenter.ViewInfo.BuildEmpty());
    }

    private void OnPresenterRepairButtonClick(int unitEntity)
    {
        _battleManager.BuildRepairSelfOrder(unitEntity);
    }

    private void OnPresenterRoomClicked(int roomEntity)
    {
        _useWeaponManager.ProcessUnitRoomAsTargetForAttack(roomEntity);
    }

    private void OnBattleFieldTileSelected(BattleFieldManager.Tile selectedTile, Vector2Int tilePos)
    {
        _selectedTilePos = tilePos;

        if (_useWeaponManager.TryProcessTileAsTargetForAttack(_selectedTilePos.Value))
        {
            return;
        }

        UpdateSelectedUnit();
    }

    private void OnUnitUpdated(int updatedUnitEntity)
    {
        if (updatedUnitEntity != _battleFieldPresenter.SelectedUnitEntity)
        {
            return;
        }
        
        UpdateSelectedUnit();
    }

    private void UpdateSelectedUnit()
    {
        if (!_selectedTilePos.HasValue)
        {
            return;
        }

        var selectedPos = _selectedTilePos.Value;

        SelectedUnitPresenter.ViewInfo selectedUnitViewInfo;
        RoomListPresenter.ViewInfo roomViews;
        if (_battleManager.TryGetUnitInPos(selectedPos, out var unitEntity))
        {
            var battleUnitInfo = _battleManager.GetBattleUnitInfo(unitEntity);

            if (_useWeaponManager.TrySelectUnitBodyAsTargetForAttack(battleUnitInfo))
            {
                return;
            }
            
            selectedUnitViewInfo = SelectedUnitPresenter.ViewInfo.BuildFromBattleInfo(battleUnitInfo);
            if (_useWeaponManager.CanSelectUnitRoomAsTargetForAttack(battleUnitInfo))
            {
                selectedUnitViewInfo.AsTarget();
                roomViews = RoomListPresenter.ViewInfo.BuildFromBattleInfo(battleUnitInfo, true);
            }
            else
            {
                roomViews = RoomListPresenter.ViewInfo.BuildFromBattleInfo(battleUnitInfo, false);
            }
        }
        else
        {
            selectedUnitViewInfo = SelectedUnitPresenter.ViewInfo.BuildEmpty();
            roomViews = RoomListPresenter.ViewInfo.BuildEmpty();
        }

        _battleFieldPresenter.UpdateSelectedUnit(selectedUnitViewInfo);
        _battleFieldPresenter.UpdateRoomsInfo(roomViews);
    }

    private void OnTurnUpdated(int newTurnIndex, TurnsManager.TurnPhase turnPhase)
    {
        _battleFieldPresenter.UpdateTurnInfo(newTurnIndex, turnPhase);
        UpdateSelectedUnit();
    }
    
    private void OnRoomSelectedAsWeaponTarget(int roomEntity, bool selected)
    {
        UpdateConfirmBtn();
    }

    private void OnUnitSelectedAsWeaponTarget(int unitEntity, bool selected)
    {
        UpdateConfirmBtn();
    }

    private void OnTileSelectedAsWeaponTarget(Vector2Int selectedTile, bool selected)
    {
        UpdateConfirmBtn();
    }

    private void UpdateConfirmBtn()
    {
        _battleFieldPresenter.UpdateConfirmTargetsBtn(_useWeaponManager.CanConfirmWeaponTargets());
    }
}