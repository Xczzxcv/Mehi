using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedUnitPresenter : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    [Space]
    [SerializeField] private WeaponPresenter weaponPrefab;
    [SerializeField] private Transform weaponsRoot;
    [Space]
    [SerializeField] private SystemPresenter systemPrefab;
    [SerializeField] private Transform systemsRoot;
    [Space]
    [SerializeField] private Button moveUnitBtn;
    [SerializeField] private Button repairUnitBtn;
    [SerializeField] private Button confirmTargetsBtn;
    
    public struct ViewInfo
    {
        public int Entity;
        public BattleMechManager.ControlledBy ControlledBy;
        public int MaxHp;
        public int CurrentHp;
        public int ShieldAmount;
        public int MaxActionPoints;
        public int CurrentActionPoints;
        public bool CanMove;
        public bool CanUseWeapons;
        public bool CanRepairSelf;
        public bool CanConfirmTargets;
        public Vector2Int UnitPosition;
        public List<WeaponPresenter.ViewInfo> Weapons;
        public List<SystemPresenter.ViewInfo> Systems;

        public static ViewInfo BuildEmpty()
        {
            return new ViewInfo
            {
                Entity = -1,
                MaxHp = 0,
                CurrentHp = 0,
                CurrentActionPoints = 0,
                MaxActionPoints = 0,
                CanMove = false,
                CanUseWeapons = false,
                CanRepairSelf = false,
                CanConfirmTargets = false,
                UnitPosition = Vector2Int.zero,
                ControlledBy = BattleMechManager.ControlledBy.None,
                Systems = new List<SystemPresenter.ViewInfo>(),
                Weapons = new List<WeaponPresenter.ViewInfo>()
            };
        }

        public static ViewInfo BuildFromBattleInfo(BattleMechManager.BattleUnitInfo battleUnitInfo)
        {
            return new ViewInfo
            {
                Entity = battleUnitInfo.Entity,
                ControlledBy = battleUnitInfo.ControlledBy,
                MaxHp = battleUnitInfo.MaxHealth,
                CurrentHp = battleUnitInfo.Health,
                ShieldAmount = battleUnitInfo.Shield,
                MaxActionPoints = battleUnitInfo.MaxActionPoints,
                CurrentActionPoints = battleUnitInfo.ActionPoints,
                CanMove = battleUnitInfo.CanMove,
                CanUseWeapons = battleUnitInfo.CanUseWeapons,
                CanRepairSelf = battleUnitInfo.CanRepairSelf,
                CanConfirmTargets = false,
                UnitPosition = battleUnitInfo.Position,
                Weapons = GetUnitWeapons(battleUnitInfo),
                Systems = GetUnitSystems(battleUnitInfo),
            };
        }
        
        private static List<WeaponPresenter.ViewInfo> GetUnitWeapons(BattleMechManager.BattleUnitInfo battleUnitInfo)
        {
            var unitWeaponViews = new List<WeaponPresenter.ViewInfo>();
            foreach (var weaponInfo in battleUnitInfo.Weapons)
            {
                var weaponView = new WeaponPresenter.ViewInfo
                {
                    UnitEntity = battleUnitInfo.Entity,
                    WeaponInfo = weaponInfo,
                };
                unitWeaponViews.Add(weaponView);
            }

            return unitWeaponViews;
        }

        private static List<SystemPresenter.ViewInfo> GetUnitSystems(BattleMechManager.BattleUnitInfo battleUnitInfo)
        {
            return new List<SystemPresenter.ViewInfo>();
        }
    }

    public event Action<int> RepairBtnClick;
    public event Action ConfirmTargetsBtnClick;

    public ViewInfo View;
    private readonly List<WeaponPresenter> _weapons = new();
    private readonly List<SystemPresenter> _systems = new();

    private bool _isMoveUnitOrderStateActive;

    public void Init()
    {
        moveUnitBtn.onClick.AddListener(OnMoveUnitBtnClick);
        repairUnitBtn.onClick.AddListener(OnRepairUnitBtnClick);
        confirmTargetsBtn.onClick.AddListener(OnConfirmTargetsBtnClick);
    }

    public void Setup(ViewInfo viewInfo)
    {
        View = viewInfo;

        UpdateHpView();
        UpdateActionsView();
        UpdateWeaponsView();
        UpdateSystemsView();

        UpdateMoveUnitOrderState(false);
    }

    private void OnMoveUnitBtnClick()
    {
        UpdateMoveUnitOrderState(!_isMoveUnitOrderStateActive);
    }

    private void OnRepairUnitBtnClick()
    {
        RepairBtnClick?.Invoke(View.Entity);
    }

    private void OnConfirmTargetsBtnClick()
    {
        if (!View.CanConfirmTargets)
        {
            return;
        }

        ConfirmTargetsBtnClick?.Invoke();
    }

    private void UpdateMoveUnitOrderState(bool isActive)
    {
        _isMoveUnitOrderStateActive = isActive;
        GlobalEventManager.BattleField.UnitMoveOrderSetActive.HappenedWith(
            View.UnitPosition, _isMoveUnitOrderStateActive
        );
    }

    private void UpdateHpView()
    {
        var hpInfo = $"Sh: {View.ShieldAmount} HP: {View.CurrentHp} / {View.MaxHp}";
        hpText.text = hpInfo;
    }

    private void UpdateActionsView()
    {
        var actionPointsInfo = $"Act. pts: {View.CurrentActionPoints} / {View.MaxActionPoints}";
        actionPointsText.text = actionPointsInfo;

        moveUnitBtn.interactable = View.CanMove;
        repairUnitBtn.interactable = View.CanRepairSelf;
        confirmTargetsBtn.interactable = View.CanConfirmTargets;
    }

    private void UpdateWeaponsView()
    {
        foreach (var weapon in _weapons)
        {
            Destroy(weapon.gameObject);
        }

        _weapons.Clear();

        foreach (var weaponViewInfo in View.Weapons)
        {
            var newWeapon = Instantiate(weaponPrefab, weaponsRoot);
            newWeapon.Init();
            newWeapon.Setup(weaponViewInfo, View.CanUseWeapons);
            _weapons.Add(newWeapon);
        }
    }

    private void UpdateSystemsView()
    {
        foreach (var system in _systems)
        {
            Destroy(system.gameObject);
        }

        _systems.Clear();

        foreach (var systemViewInfo in View.Systems)
        {
            var newSystem = Instantiate(systemPrefab, systemsRoot);
            newSystem.Setup(systemViewInfo);
            _systems.Add(newSystem);
        }
    }

    public void UpdateConfirmTargetsBtn(bool canConfirmTargets)
    {
        View.CanConfirmTargets = canConfirmTargets;
        confirmTargetsBtn.interactable = View.CanConfirmTargets;
    }
}