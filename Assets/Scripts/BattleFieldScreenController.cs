using System.Collections.Generic;
using UnityEngine;

internal class BattleFieldScreenController : MonoBehaviour
{
    [SerializeField] private BattleFieldScreenPresenter fieldScreenPrefab;
    
    private Transform _uiRoot;
    private BattleManager _battleManager;
    private BattleFieldScreenPresenter _battleFieldPresenter;
    
    private Vector2Int? _selectedTilePos;

    public void Init(Transform uiRoot, BattleManager battleManager)
    {
        _uiRoot = uiRoot;
        _battleManager = battleManager;
        
        GlobalEventManager.BattleField.GridTileSelected.Event += OnBattleFieldTileSelected;
        GlobalEventManager.Turns.TurnUpdated.Event += OnTurnUpdated;
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
    }

    private void OnPresenterNextTurnPhaseBtnClicked()
    {
        _battleManager.NextPhase();
    }

    private void OnBattleFieldTileSelected(BattleFieldManager.Tile selectedTile, Vector2Int tilePos)
    {
        _selectedTilePos = tilePos;
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
        if (_battleManager.TryGetUnitInPos(selectedPos, out var unitEntity))
        {
            var battleUnitInfo = _battleManager.GetBattleUnitInfo(unitEntity);
            selectedUnitViewInfo = new SelectedUnitPresenter.ViewInfo
            {
                ControlledBy = battleUnitInfo.ControlledBy,
                MaxHp = battleUnitInfo.MaxHealth,
                CurrentHp = battleUnitInfo.Health,
                ShieldAmount = battleUnitInfo.Shield,
                MaxActionPoints = battleUnitInfo.MaxActionPoints,
                CurrentActionPoints = battleUnitInfo.ActionPoints,
                CanMove = battleUnitInfo.CanMove,
                CanUseWeapon = battleUnitInfo.CanUseWeapon,
                UnitPosition = battleUnitInfo.Position,
                Weapons = GetUnitWeapons(battleUnitInfo),
                Systems = GetUnitSystems(battleUnitInfo),
            };
        }
        else
        {
            selectedUnitViewInfo = SelectedUnitPresenter.ViewInfo.BuildEmpty();
        }

        _battleFieldPresenter.UpdateSelectedUnit(selectedUnitViewInfo);
    }

    private void OnTurnUpdated(int newTurnIndex, TurnsManager.TurnPhase turnPhase)
    {
        _battleFieldPresenter.UpdateTurnInfo(newTurnIndex, turnPhase);
        UpdateSelectedUnit();
    }

    private List<WeaponPresenter.ViewInfo> GetUnitWeapons(BattleMechManager.BattleUnitInfo battleUnitInfo)
    {
        var unitWeaponViews = new List<WeaponPresenter.ViewInfo>();
        foreach (var weaponInfo in battleUnitInfo.Weapons)
        {
            var weaponView = new WeaponPresenter.ViewInfo
            {
                WeaponId = weaponInfo.WeaponId,
                Damage = weaponInfo.Damage,
                PushDistance = weaponInfo.PushDistance,
                StunDuration = weaponInfo.StunDuration,
            };
            unitWeaponViews.Add(weaponView);
        }

        return unitWeaponViews;
    }

    private List<SystemPresenter.ViewInfo> GetUnitSystems(BattleMechManager.BattleUnitInfo battleUnitInfo)
    {
        return new List<SystemPresenter.ViewInfo>();
    }
}