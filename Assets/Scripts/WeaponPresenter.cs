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

    public void Setup(ViewInfo viewInfo, bool canUseWeapons)
    {
        _viewInfo = viewInfo;

        weaponName.text = _viewInfo.WeaponInfo.WeaponId;
        weaponStats.text = GetWeaponStatsText(_viewInfo);
        useWeaponButton.interactable = CanUseWeapon(viewInfo.WeaponInfo, canUseWeapons);
    }

    private static string GetWeaponStatsText(ViewInfo viewInfo)
    {
        var statsText = string.Empty;
        var weaponInfo = viewInfo.WeaponInfo;
        foreach (var (statId, statValue) in weaponInfo.Stats)
        {
            var statText = $"{statId}: {statValue}\n";
            statsText += statText;
        }

        var cdText = weaponInfo.Cooldown.HasValue 
            ? $"CD: {weaponInfo.Cooldown.Value}\n:" 
            : string.Empty;
        return $"{cdText}Stats:\n{statsText}";
    }

    private static bool CanUseWeapon(BattleMechManager.WeaponInfo weaponInfo, bool canUseWeapons)
    {
        return weaponInfo.CanUse && canUseWeapons;
    }
}