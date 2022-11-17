using System;
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

    public struct ViewInfo
    {
        public int TurnIndex;
        public TurnsManager.TurnPhase TurnPhase;
    }

    public event Action NextTurnPhaseBtnClicked;
    
    private ViewInfo _viewInfo;
    
    public void Init()
    {
        nextPhaseButton.onClick.AddListener(OnNextPhaseBtnClick);
        selectedUnitPresenter.Init();
    }

    public void Setup(ViewInfo viewInfo)
    {
        _viewInfo = viewInfo;
        selectedUnitPresenter.Setup(SelectedUnitPresenter.ViewInfo.BuildEmpty());
        UpdateTurnInfo(_viewInfo.TurnIndex, _viewInfo.TurnPhase);
    }

    private void OnNextPhaseBtnClick()
    {
        NextTurnPhaseBtnClicked?.Invoke();
    }

    public void UpdateSelectedUnit(SelectedUnitPresenter.ViewInfo unitInfo)
    {
        selectedUnitPresenter.Setup(unitInfo);
    }

    public void UpdateTurnInfo(int turnInd, TurnsManager.TurnPhase phase)
    {
        turnIndex.text = $"TURN: {turnInd}";
        turnPhase.text = $"PHASE: {phase}";
    }
}