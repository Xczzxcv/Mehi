using UnityEngine;

public class FieldTileController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundImage;
    [SerializeField] private GameObject selectedMark;
    [SerializeField] private Transform contentParent;
    [SerializeField] private BoxCollider2D tileCollider;
    
    public struct Config
    {
        public BattleFieldManager.Tile Tile;
        public Sprite Background;
        public Vector2Int TilePosition;
        public float TileSize;
    }

    public BattleFieldManager.Tile Tile => _config.Tile;
    public Vector2Int Pos => _config.TilePosition;
    
    private Config _config;

    public void Setup(Config config)
    {
        _config = config;
        backgroundImage.sprite = _config.Background;
        tileCollider.size = Vector2.one * _config.TileSize;
    }

    public void SetSelected(bool selected)
    {
        selectedMark.SetActive(selected);
    }

    public void SetupContent(Transform contentRoot)
    {
        contentRoot.SetParent(contentParent, false);
    }
}