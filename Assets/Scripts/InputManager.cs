using System;
using Extension;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    
    private void Update()
    {
        ProcessMouseClick();
    }

    private void ProcessMouseClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var ray = mainCam.ScreenPointToRay(Input.mousePosition);
            var raycastHits2D = new RaycastHit2D[10];
            var size = Physics2D.GetRayIntersectionNonAlloc(ray, raycastHits2D, float.PositiveInfinity);
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

            GlobalEventManager.BattleFieldGridTileSelected.HappenedWith(
                tileController.Tile, tileController.Pos);
        }
    }
}