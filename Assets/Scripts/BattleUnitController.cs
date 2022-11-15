using UnityEngine;

public class BattleUnitController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer img;

    private BattleMechManager.BattleUnitInfo _unitInfo;
    
    public void Setup(BattleMechManager.BattleUnitInfo unitInfo, Color unitColor)
    {
        _unitInfo = unitInfo;

        img.color = unitColor;
    }
}