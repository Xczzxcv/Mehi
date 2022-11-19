using System.Collections.Generic;
using UnityEngine;

internal class BattleFieldScreenController : MonoBehaviour
{
    [SerializeField] private BattleFieldScreenPresenter fieldScreenPrefab;
    
    private Transform _uiRoot;
    private BattleManager _battleManager;
    private BattleFieldScreenPresenter _battleFieldPresenter;
    
    private Vector2Int? _selectedTilePos;

    private struct UseWeaponInfo
    {
        public bool UseWeaponModeActive;
        public int WeaponUserEntity;
        public BattleMechManager.WeaponInfo UsedWeaponInfo;
    }

    private UseWeaponInfo _useWeaponInfo;

    public void Init(Transform uiRoot, BattleManager battleManager)
    {
        _uiRoot = uiRoot;
        _battleManager = battleManager;
        
        GlobalEventManager.BattleField.GridTileSelected.Event += OnBattleFieldTileSelected;
        GlobalEventManager.BattleField.UnitUpdated.Event += OnUnitUpdated;
        GlobalEventManager.Turns.TurnUpdated.Event += OnTurnUpdated;
        GlobalEventManager.BattleField.UseWeaponBtnClicked.Event += OnUseWeaponBtnClicked;
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
        _battleFieldPresenter.RoomsChoiceConfirmed += OnPresenterRoomsChoiceConfirmed;
        _battleFieldPresenter.RepairButtonClick += OnPresenterRepairButtonClick;
    }

    private void OnPresenterNextTurnPhaseBtnClicked()
    {
        _battleManager.NextPhase();
    }

    private void OnPresenterRoomsChoiceConfirmed(List<int> selectedRooms)
    {
        _battleManager.BuildUseWeaponOrder(
            _useWeaponInfo.WeaponUserEntity,
            _useWeaponInfo.UsedWeaponInfo,
            selectedRooms
            );
        _useWeaponInfo.UseWeaponModeActive = false;
    }

    private void OnPresenterRepairButtonClick(int unitEntity)
    {
        _battleManager.BuildRepairSelfOrder(unitEntity);
    }

    private void OnBattleFieldTileSelected(BattleFieldManager.Tile selectedTile, Vector2Int tilePos)
    {
        _selectedTilePos = tilePos;
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
            
            if (IsSelectedUnitThatCanBeAttacked(battleUnitInfo))
            {
                selectedUnitViewInfo = SelectedUnitPresenter.ViewInfo.BuildEmpty();
                roomViews = RoomListPresenter.ViewInfo.BuildFromBattleInfo(battleUnitInfo, true);
            }
            else
            {
                selectedUnitViewInfo = SelectedUnitPresenter.ViewInfo.BuildFromBattleInfo(battleUnitInfo);
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

    private bool IsSelectedUnitThatCanBeAttacked(BattleMechManager.BattleUnitInfo battleUnitInfo)
    {
        if (!_useWeaponInfo.UseWeaponModeActive)
        {
            return false;
        }

        var attackerControl = _battleManager.GetBattleUnitInfo(
            _useWeaponInfo.WeaponUserEntity).ControlledBy;
        var victimControl = battleUnitInfo.ControlledBy;
        return BattleMechManager.CanAttack(attackerControl, victimControl);
    }

    private void OnTurnUpdated(int newTurnIndex, TurnsManager.TurnPhase turnPhase)
    {
        _battleFieldPresenter.UpdateTurnInfo(newTurnIndex, turnPhase);
        UpdateSelectedUnit();
    }

    private void OnUseWeaponBtnClicked(int unitEntity, BattleMechManager.WeaponInfo weaponInfo)
    {
        _useWeaponInfo.WeaponUserEntity = unitEntity;
        _useWeaponInfo.UsedWeaponInfo = weaponInfo;
        _useWeaponInfo.UseWeaponModeActive = true;
    }
}