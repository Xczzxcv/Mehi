using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUnitPresenter : UIBehaviour
{
    [SerializeField] private Image img;

    private BattleMechManager.BattleUnitInfo _unitInfo;
    
    public void Setup(BattleMechManager.BattleUnitInfo unitInfo, Color unitColor)
    {
        _unitInfo = unitInfo;

        img.color = unitColor;
    }
}