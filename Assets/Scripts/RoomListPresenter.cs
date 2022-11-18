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
    [SerializeField] private Button confirmRoomsChoiceBtn;
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

    public event Action<List<int>> RoomsChoiceConfirmed;
    
    private ViewInfo _view;
    private readonly Dictionary<int, RoomPresenter> _rooms = new();

    public void Init()
    {
        UpdateGridWidth();
        confirmRoomsChoiceBtn.onClick.AddListener(OnConfirmRoomsChoiceClick);
    }

    public void Setup(ViewInfo view)
    {
        _view = view;
        
        confirmRoomsChoiceBtn.interactable = _view.RoomsCanBeSelected;

        UpdateRooms();
    }

    private void OnConfirmRoomsChoiceClick()
    {
        if (!_view.RoomsCanBeSelected)
        {
            return;
        }

        var selectedRooms = _rooms
            .Where(pair => pair.Value.View.Selected)
            .Select(pair => pair.Key)
            .ToList();

        foreach (var roomPresenter in _rooms.Values)
        {
            roomPresenter.SetSelected(false);
        }

        RoomsChoiceConfirmed?.Invoke(selectedRooms);
    }

    private void UpdateGridWidth()
    {
        grid.constraintCount = rowWidth;
    }

    private void UpdateRooms()
    {
        foreach (var (roomEntity, roomPresenter) in _rooms)
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

        var roomPresenter = _rooms[roomEntity];
        roomPresenter.SetSelected(!roomPresenter.View.Selected);
    }

    protected override void OnValidate()
    {
        UpdateGridWidth();
    }
}
