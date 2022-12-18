using Leopotam.EcsLite;
using UnityEngine;

public static partial class GlobalEventManager
{
    public static class BattleField
    {
        public static EventManager<BattleFieldManager.Tile, Vector2Int> GridTileSelected;
        public static EventManager<BattleFieldManager.Tile, Vector2Int> GridTileHovered;
        public static EventManager<Vector2Int, bool> UnitMoveOrderSetActive;
        public static EventManager<int, Vector2Int, Vector2Int> UnitMoved;
        public static EventManager<int> UnitUpdated;
        public static EventManager<int, BattleMechManager.WeaponInfo> UseWeaponBtnClicked;
        public static EventManager<int, bool> RoomSelectedAsWeaponTarget;
        public static EventManager<int, bool> UnitSelectedAsWeaponTarget;
        public static EventManager<Vector2Int, bool> TileSelectedAsWeaponTarget;
    }

    public static class Turns
    {
        public static EventManager<int, TurnsManager.TurnPhase> TurnUpdated;
    }
}