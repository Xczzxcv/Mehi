using System.Collections.Generic;
using UnityEngine;

internal class BattleFieldScreenController : MonoBehaviour
{
    [SerializeField] private BattleFieldScreenPresenter fieldScreenPrefab;
    
    private Transform _uiRoot;
    private BattleManager _battleManager;
    private BattleFieldScreenPresenter _battleFieldPresenter;

    public void Init(Transform uiRoot, BattleManager battleManager)
    {
        _uiRoot = uiRoot;
        _battleManager = battleManager;
    }

    public void Setup()
    {
        _battleFieldPresenter = Instantiate(fieldScreenPrefab, _uiRoot);
        _battleFieldPresenter.Init();
        _battleFieldPresenter.Setup();
        _battleFieldPresenter.FieldTileSelected += OnBattleFieldTileSelected;
    }

    private void OnBattleFieldTileSelected(BattleFieldManager.Tile selectedTile, Vector2Int tilePos)
    {
        SelectedUnitPresenter.ViewInfo selectedUnitViewInfo;
        if (_battleManager.TryGetUnitInPos(tilePos.x, tilePos.y, out var unitEntity))
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