using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomListPresenter : UIBehaviour
{
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private int rowWidth;
    [SerializeField] private Transform roomsParent;
    [SerializeField] private RoomPresenter roomPrefab;
    [SerializeField] private GameObject root;

    public struct ViewInfo
    {
        public List<RoomPresenter.ViewInfo> Rooms;
        public bool RoomsCanBeSelected;

        public static ViewInfo BuildEmpty()
        {
            return new ViewInfo
            {
                Rooms = new List<RoomPresenter.ViewInfo>(),
                RoomsCanBeSelected = false,
            };
        }

        public static ViewInfo BuildFromBattleInfo(BattleMechManager.BattleUnitInfo battleUnitInfo, 
            bool roomsCanBeSelected)
        {
            var roomViews = new ViewInfo
            {
                Rooms = new List<RoomPresenter.ViewInfo>(),
                RoomsCanBeSelected = roomsCanBeSelected,
            };

            foreach (var roomInfo in battleUnitInfo.Rooms)
            {
                var roomView = new RoomPresenter.ViewInfo
                {
                    Entity = roomInfo.Entity,
                    Hp = roomInfo.Health,
                    MaxHp = roomInfo.MaxHealth,
                    System = roomInfo.SystemType,
                    SystemLvl = roomInfo.SystemLvl,
                    Selected = false
                };
                roomViews.Rooms.Add(roomView);
            }

            return roomViews;
        }
    }

    public event Action<int> RoomClicked;

    private ViewInfo _view;
    private readonly Dictionary<int, RoomPresenter> _rooms = new();

    public void Init()
    {
        UpdateGridWidth();

        GlobalEventManager.BattleField.RoomSelectedAsWeaponTarget.Event += OnRoomSelectedAsWeaponTarget;
    }

    public void Setup(ViewInfo view)
    {
        _view = view;
        
        UpdateRooms();
    }

    private void UpdateGridWidth()
    {
        grid.constraintCount = rowWidth;
    }

    private void UpdateRooms()
    {
        foreach (var roomPresenter in _rooms.Values)
        {
            roomPresenter.RoomClicked -= OnRoomClicked;
            Destroy(roomPresenter.gameObject);
        }
        _rooms.Clear();

        if (!_view.Rooms.Any())
        {
            root.SetActive(false);
            return;
        }
        
        foreach (var roomView in _view.Rooms)
        {
            var roomPresenter = Instantiate(roomPrefab, roomsParent);
            roomPresenter.Init();
            roomPresenter.Setup(roomView);
            roomPresenter.RoomClicked += OnRoomClicked;
            _rooms.Add(roomView.Entity, roomPresenter);
        }
        root.SetActive(true);
    }

    private void OnRoomClicked(int roomEntity)
    {
        if (!_view.RoomsCanBeSelected)
        {
            return;
        }
        
        RoomClicked?.Invoke(roomEntity);
    }

    private void OnRoomSelectedAsWeaponTarget(int roomEntity, bool selected)
    {
        var roomPresenter = _rooms[roomEntity];
        roomPresenter.SetSelected(selected);
    }
}
