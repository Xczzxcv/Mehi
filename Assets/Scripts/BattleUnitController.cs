using UnityEngine;

public class BattleUnitController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer img;

    public BattleMechManager.BattleUnitInfo UnitInfo { get; private set; }

    public void Setup(BattleMechManager.BattleUnitInfo unitInfo, Color unitColor)
    {
        UnitInfo = unitInfo;

        img.color = unitColor;
    }
}