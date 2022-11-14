using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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
    
    public struct ViewInfo
    {
        public BattleMechManager.ControlledBy ControlledBy;
        public int MaxHp;
        public int CurrentHp;
        public int ShieldAmount;
        public int MaxActionPoints;
        public int CurrentActionPoints;
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
                ControlledBy = BattleMechManager.ControlledBy.None,
                Systems = new List<SystemPresenter.ViewInfo>(),
                Weapons = new List<WeaponPresenter.ViewInfo>()
            };
        }
    }

    private ViewInfo _viewInfo;
    private List<WeaponPresenter> _weapons = new();
    private List<SystemPresenter> _systems = new();

    public void Setup(ViewInfo viewInfo)
    {
        _viewInfo = viewInfo;

        UpdateHpView();
        UpdateActionPointsView();
        UpdateWeaponsView();
        UpdateSystemsView();
    }

    private void UpdateHpView()
    {
        var hpInfo = $"Sh: {_viewInfo.ShieldAmount} HP: {_viewInfo.CurrentHp} / {_viewInfo.MaxHp}";
        hpText.text = hpInfo;
    }

    private void UpdateActionPointsView()
    {
        var actionPointsInfo = $"Act. pts: {_viewInfo.CurrentActionPoints} / {_viewInfo.MaxActionPoints}";
        actionPointsText.text = actionPointsInfo;
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
            newWeapon.Setup(weaponViewInfo);
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