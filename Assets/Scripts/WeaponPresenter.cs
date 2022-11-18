using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponPresenter : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponStats;
    [SerializeField] private Button useWeaponButton;
 
    public struct ViewInfo
    {
        public int UnitEntity;
        public BattleMechManager.WeaponInfo WeaponInfo;
    }

    private ViewInfo _viewInfo;

    public void Init()
    {
        useWeaponButton.onClick.AddListener(OnUseWeaponBtnClick);
    }

    private void OnUseWeaponBtnClick()
    {
        GlobalEventManager.BattleField.UseWeaponBtnClicked.HappenedWith(
            _viewInfo.UnitEntity, _viewInfo.WeaponInfo);
    }

    public void Setup(ViewInfo viewInfo, bool canUseWeapon)
    {
        _viewInfo = viewInfo;

        weaponName.text = _viewInfo.WeaponInfo.WeaponId;
        weaponStats.text = GetWeaponStatsText(_viewInfo);
        useWeaponButton.interactable = canUseWeapon;
    }

    private static string GetWeaponStatsText(ViewInfo viewInfo)
    {
        const string statPlaceHolder = "—";
        var weaponInfo = viewInfo.WeaponInfo;
        var dmgText = weaponInfo.Damage.HasValue 
            ? weaponInfo.Damage.Value.ToString() 
            : statPlaceHolder;
        var stunText = weaponInfo.StunDuration.HasValue 
            ? weaponInfo.StunDuration.Value.ToString()
            : statPlaceHolder;
        var pushText = weaponInfo.PushDistance.HasValue 
            ? weaponInfo.PushDistance.Value.ToString() 
            : statPlaceHolder;
        return $"Stats: dmg {dmgText}, stun {stunText}, push {pushText}";
    }
}