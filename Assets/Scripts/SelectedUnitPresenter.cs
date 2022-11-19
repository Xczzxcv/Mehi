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
        public bool CanUseWeapon;
        public bool CanRepairSelf;
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
                CanUseWeapon = false,
                CanRepairSelf = false,
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
                CanUseWeapon = battleUnitInfo.CanUseWeapon,
                CanRepairSelf = battleUnitInfo.CanRepairSelf,
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

    public event Action<int> RepairButtonClick;

    public ViewInfo View;
    private readonly List<WeaponPresenter> _weapons = new();
    private readonly List<SystemPresenter> _systems = new();

    private bool _isMoveUnitOrderStateActive;

    public void Init()
    {
        moveUnitBtn.onClick.AddListener(OnMoveUnitBtnClick);
        repairUnitBtn.onClick.AddListener(OnRepairUnitBtnClick);
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
        RepairButtonClick?.Invoke(View.Entity);
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
            newWeapon.Setup(weaponViewInfo, View.CanUseWeapon);
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
}