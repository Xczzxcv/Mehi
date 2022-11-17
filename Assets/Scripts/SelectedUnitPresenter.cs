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
    
    public struct ViewInfo
    {
        public BattleMechManager.ControlledBy ControlledBy;
        public int MaxHp;
        public int CurrentHp;
        public int ShieldAmount;
        public int MaxActionPoints;
        public int CurrentActionPoints;
        public bool CanMove;
        public bool CanUseWeapon;
        public Vector2Int UnitPosition;
        public List<WeaponPresenter.ViewInfo> Weapons;
        public List<SystemPresenter.ViewInfo> Systems;

        public static ViewInfo BuildEmpty()
        {
            return new ViewInfo
            {
                MaxHp = 0,
                CurrentHp = 0,
                CurrentActionPoints = 0,
                MaxActionPoints = 0,
                CanMove = false,
                CanUseWeapon = false,
                UnitPosition = Vector2Int.zero,
                ControlledBy = BattleMechManager.ControlledBy.None,
                Systems = new List<SystemPresenter.ViewInfo>(),
                Weapons = new List<WeaponPresenter.ViewInfo>()
            };
        }
    }

    private ViewInfo _viewInfo;
    private readonly List<WeaponPresenter> _weapons = new();
    private readonly List<SystemPresenter> _systems = new();

    private bool _isMoveUnitOrderStateActive;

    public void Init()
    {
        moveUnitBtn.onClick.AddListener(OnMoveUnitBtnClick);
    }

    private void OnMoveUnitBtnClick()
    {
        UpdateMoveUnitOrderState(!_isMoveUnitOrderStateActive);
    }

    public void Setup(ViewInfo viewInfo)
    {
        _viewInfo = viewInfo;

        UpdateHpView();
        UpdateActionsView();
        UpdateWeaponsView();
        UpdateSystemsView();

        UpdateMoveUnitOrderState(false);
    }

    private void UpdateMoveUnitOrderState(bool isActive)
    {
        _isMoveUnitOrderStateActive = isActive;
        GlobalEventManager.BattleField.UnitMoveOrderSetActive.HappenedWith(
            _viewInfo.UnitPosition, _isMoveUnitOrderStateActive
        );
    }

    private void UpdateHpView()
    {
        var hpInfo = $"Sh: {_viewInfo.ShieldAmount} HP: {_viewInfo.CurrentHp} / {_viewInfo.MaxHp}";
        hpText.text = hpInfo;
    }

    private void UpdateActionsView()
    {
        var actionPointsInfo = $"Act. pts: {_viewInfo.CurrentActionPoints} / {_viewInfo.MaxActionPoints}";
        actionPointsText.text = actionPointsInfo;

        moveUnitBtn.interactable = _viewInfo.CanMove;
    }

    private void UpdateWeaponsView()
    {
        foreach (var weapon in _weapons)
        {
            Destroy(weapon.gameObject);
        }

        _weapons.Clear();

        foreach (var weaponViewInfo in _viewInfo.Weapons)
        {
            var newWeapon = Instantiate(weaponPrefab, weaponsRoot);
            newWeapon.Init();
            newWeapon.Setup(weaponViewInfo, _viewInfo.CanUseWeapon);
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

        foreach (var systemViewInfo in _viewInfo.Systems)
        {
            var newSystem = Instantiate(systemPrefab, systemsRoot);
            newSystem.Setup(systemViewInfo);
            _systems.Add(newSystem);
        }
    }
}