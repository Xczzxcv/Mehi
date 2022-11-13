using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SystemPresenter : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI systemName;
    [SerializeField] private TextMeshProUGUI systemLevel;
 
    public struct ViewInfo
    {
        public string SystemId;
        public int SystemLvl;
    }

    private ViewInfo _viewInfo;

    public void Setup(ViewInfo viewInfo)
    {
        _viewInfo = viewInfo;

        systemName.text = _viewInfo.SystemId;
        systemLevel.text = $"{_viewInfo.SystemLvl} Lvl";
    }
}