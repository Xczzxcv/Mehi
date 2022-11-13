using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleFieldTilePresenter : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Transform contentParent;

    public struct ViewInfo
    {
        public BattleFieldManager.Tile Tile;
        public Sprite Background;
        public Sprite Foreground;
        public Vector2Int TilePosition;
    }
    
    public event Action<BattleFieldManager.Tile, Vector2Int> Selected;
    private ViewInfo _viewInfo;

    public void Init(ToggleGroup parentGroup)
    {
        toggle.group = parentGroup;
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void Setup(ViewInfo viewInfo)
    {
        _viewInfo = viewInfo;
        backgroundImage.sprite = _viewInfo.Background;
        foregroundImage.sprite = _viewInfo.Foreground;
    }

    private void OnToggleValueChanged(bool newValue)
    {
        if (newValue)
        {
            Selected?.Invoke(_viewInfo.Tile, _viewInfo.TilePosition);
        }
    }

    public void SetupContent(Transform contentRoot)
    {
        contentRoot.SetParent(contentParent, false);
    }
}
