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
        public string WeaponId;
        public int? Damage;
        public int? StunDuration;
        public int? PushDistance;
    }

    private ViewInfo _viewInfo;

    public void Init()
    {
        useWeaponButton.onClick.AddListener(OnUseWeaponBtnClick);
    }

    private void OnUseWeaponBtnClick()
    {
        Debug.Log($"Use weapon {weaponName} placeholder");
    }

    public void Setup(ViewInfo viewInfo, bool canUseWeapon)
    {
        _viewInfo = viewInfo;

        weaponName.text = _viewInfo.WeaponId;
        weaponStats.text = GetWeaponStatsText(_viewInfo);
        useWeaponButton.interactable = canUseWeapon;
    }

    private static string GetWeaponStatsText(ViewInfo viewInfo)
    {
        const string statPlaceHolder = "—";
        var dmgText = viewInfo.Damage.HasValue ? viewInfo.Damage.Value.ToString() : statPlaceHolder;
        var stunText = viewInfo.StunDuration.HasValue ? viewInfo.StunDuration.Value.ToString() : statPlaceHolder;
        var pushText = viewInfo.PushDistance.HasValue ? viewInfo.PushDistance.Value.ToString() : statPlaceHolder;
        return $"Stats: dmg {dmgText}, stun {stunText}, push {pushText}";
    }
}