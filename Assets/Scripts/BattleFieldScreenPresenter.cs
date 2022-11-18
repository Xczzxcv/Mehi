using System;
using System.Collections.Generic;
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
    public event Action<List<int>> RoomsChoiceConfirmed;
    
    private ViewInfo _viewInfo;
    
    public void Init()
    {
        nextPhaseButton.onClick.AddListener(OnNextPhaseBtnClick);
        selectedUnitPresenter.Init();
        roomsPresenter.Init();
        roomsPresenter.RoomsChoiceConfirmed += OnRoomsChoiceConfirmed;
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

    private void OnRoomsChoiceConfirmed(List<int> selectedRooms)
    {
        RoomsChoiceConfirmed?.Invoke(selectedRooms);
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
}