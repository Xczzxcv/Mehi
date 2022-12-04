using System;
using UnityEngine;

public class BattleUnitController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer img;
    [SerializeField] private Color selectedAsTargetColor;
    
    public enum HighlightType
    {
        None,
        AsWeaponTarget
    }    
    
    public BattleMechManager.BattleUnitInfo UnitInfo { get; private set; }
    private Color _currentColor;

    public void Setup(BattleMechManager.BattleUnitInfo unitInfo, Color unitColor)
    {
        UnitInfo = unitInfo;
        _currentColor = unitColor;

        UpdateView();
    }

    public void SetHighlighted(HighlightType highlightType)
    {
        switch (highlightType)
        {
            case HighlightType.None:
                UpdateView();
                break;
            case HighlightType.AsWeaponTarget:
                SetColor(selectedAsTargetColor);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void UpdateView()
    {
        SetColor(_currentColor);
    }

    private void SetColor(Color newColor)
    {
        img.color = newColor;
    }
}