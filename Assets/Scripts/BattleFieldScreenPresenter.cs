using System;
using Ecs.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleFieldScreenPresenter : UIBehaviour
{
    [SerializeField] private SelectedUnitPresenter selectedUnitPresenter;
    [SerializeField] private TextMeshProUGUI turnIndex;
    [SerializeField] private TextMeshProUGUI turnPhase;
    [SerializeField] private Button nextPhaseButton;
    [SerializeField] private RoomListPresenter roomsPresenter;

    public struct ViewInfo
    {
        public int TurnIndex;
        public TurnsManager.TurnPhase TurnPhase;
    }

    public event Action NextTurnPhaseBtnClicked;
    public event Action TargetsChoiceConfirmed;
    public event Action<int> RepairButtonClick;
    public event Action<int> RoomClicked;
    
    private ViewInfo _viewInfo;
    public int SelectedUnitEntity => selectedUnitPresenter.View.Entity;

    public void Init()
    {
        nextPhaseButton.onClick.AddListener(OnNextPhaseBtnClick);
        selectedUnitPresenter.Init();
        roomsPresenter.Init();
        
        selectedUnitPresenter.RepairBtnClick += OnRepairBtnClick;
        selectedUnitPresenter.ConfirmTargetsBtnClick += OnConfirmTargetsBtnClick;
        roomsPresenter.RoomClicked += OnRoomClicked;
    }

    public void Setup(ViewInfo viewInfo)
    {
        _viewInfo = viewInfo;
        selectedUnitPresenter.Setup(SelectedUnitPresenter.ViewInfo.BuildEmpty());
        roomsPresenter.Setup(RoomListPresenter.ViewInfo.BuildEmpty());
        UpdateTurnInfo(_viewInfo.TurnIndex, _viewInfo.TurnPhase);
    }

    private void OnNextPhaseBtnClick()
    {
        NextTurnPhaseBtnClicked?.Invoke();
    }

    private void OnRepairBtnClick(int unitEntity)
    {
        RepairButtonClick?.Invoke(unitEntity);
    }

    private void OnConfirmTargetsBtnClick()
    {
        TargetsChoiceConfirmed?.Invoke();
    }

    private void OnRoomClicked(int roomEntity)
    {
        RoomClicked?.Invoke(roomEntity);
    }

    public void UpdateSelectedUnit(SelectedUnitPresenter.ViewInfo unitInfo)
    {
        selectedUnitPresenter.Setup(unitInfo);
    }

    public void UpdateRoomsInfo(RoomListPresenter.ViewInfo roomsView)
    {
        roomsPresenter.Setup(roomsView);
    }

    public void UpdateTurnInfo(int turnInd, TurnsManager.TurnPhase phase)
    {
        turnIndex.text = $"TURN: {turnInd}";
        turnPhase.text = $"PHASE: {phase}";
    }

    public void UpdateConfirmTargetsBtn(bool canConfirmTargets)
    {
        selectedUnitPresenter.UpdateConfirmTargetsBtn(canConfirmTargets);
    }
}