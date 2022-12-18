using System;
using TMPro;
using UnityEngine;

public class FieldTileController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private Transform contentParent;
    [SerializeField] private BoxCollider2D tileCollider;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color highlightedAsTargetColor;
    [Space]
    [SerializeField] private TextMeshPro tilePosText;
    [SerializeField] private RectTransform tilePosTextRect;
    
    public struct Config
    {
        public BattleFieldManager.Tile Tile;
        public Sprite Background;
        public Vector2Int TilePosition;
        public float TileSize;
    }

    public enum HighlightType
    {
        None,
        Default,
        AsWeaponTarget
    }

    public BattleFieldManager.Tile Tile => _config.Tile;
    public Vector2Int Pos => _config.TilePosition;
    
    private Config _config;
    private HighlightType _currHighlightType = HighlightType.None;
    public HighlightType LastHighlightType { get; private set; } = HighlightType.None;

    public void Setup(Config config)
    {
        _config = config;
        backgroundImage.sprite = _config.Background;
        tileCollider.size = Vector2.one * _config.TileSize;
        tilePosText.text = _config.TilePosition.ToString();
        tilePosTextRect.sizeDelta = Vector2.one * _config.TileSize;
    }

    public void SetSelected(bool selected)
    {
        selectedMark.SetActive(selected);
    }

    public void SetupContent(Transform contentRoot)
    {
        contentRoot.SetParent(contentParent, false);
    }

    public void SetHighlighted(HighlightType highlightType)
    {
        LastHighlightType = _currHighlightType;
        _currHighlightType = highlightType;
        
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        var backgroundColor = _currHighlightType switch
        {
            HighlightType.None => Color.white,
            HighlightType.Default => highlightedColor,
            HighlightType.AsWeaponTarget => highlightedAsTargetColor,
            _ => throw new NotImplementedException()
        };

        backgroundImage.color = backgroundColor;
    }
}