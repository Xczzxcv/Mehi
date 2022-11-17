using Extension;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCam;

    private FieldTileController _lastHoveredTile;

    private void Update()
    {
        ProcessTileMouseClick();
        ProcessTileMouseHover();
    }

    private void ProcessTileMouseClick()
    {
        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        var ray = mainCam.ScreenPointToRay(Input.mousePosition);
        var raycastHit2D = Physics2D.GetRayIntersection(ray, float.PositiveInfinity,
            Constants.Layers.GRID_TILES.ToLayerMaskInt());
        if (!raycastHit2D.collider)
        {
            return;
        }

        if (!raycastHit2D.collider.TryGetComponent<FieldTileController>(out var tileController))
        {
            Debug.LogError("Smth wrong i can feel it");
        }

        GlobalEventManager.BattleField.GridTileSelected.HappenedWith(
            tileController.Tile, tileController.Pos);
    }

    private void ProcessTileMouseHover()
    {
        var ray = mainCam.ScreenPointToRay(Input.mousePosition);
        var raycastHit2D = Physics2D.GetRayIntersection(ray, float.PositiveInfinity,
            Constants.Layers.GRID_TILES.ToLayerMaskInt());
        if (!raycastHit2D.collider)
        {
            return;
        }

        if (!raycastHit2D.collider.TryGetComponent<FieldTileController>(out var tileController))
        {
            Debug.LogError("Smth wrong i can feel it");
        }

        if (_lastHoveredTile && _lastHoveredTile == tileController)
        {
            return;
        }
        
        GlobalEventManager.BattleField.GridTileHovered.HappenedWith(
            tileController.Tile, tileController.Pos);

        _lastHoveredTile = tileController;
    }
}