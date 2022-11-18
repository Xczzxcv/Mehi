using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomPresenter : UIBehaviour
{
    [SerializeField] private TextMeshProUGUI roomSystemText;
    [SerializeField] private TextMeshProUGUI roomHpText;
    [SerializeField] private Image background;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Button roomBtn;

    public struct ViewInfo
    {
        public int Entity;
        public MechSystemType System;
        public int SystemLvl;
        public int Hp;
        public int MaxHp;
        public bool Selected;
    }

    public event Action<int> RoomClicked;

    public ViewInfo View;

    public void Init()
    {
        roomBtn.onClick.AddListener(OnRoomBtnClicked);
    }

    private void OnRoomBtnClicked()
    {
        RoomClicked?.Invoke(View.Entity);
    }

    public void Setup(ViewInfo view)
    {
        View = view;

        UpdateSystemText();
        UpdateHpText();
        UpdateSelection();
    }

    private void UpdateSystemText()
    {
        var systemName = View.System.ToString().Replace("System", string.Empty);
        roomSystemText.text = $"{systemName} {View.SystemLvl}";
    }

    private void UpdateHpText()
    {
        roomHpText.text = $"HP: {View.Hp} /  {View.MaxHp}";
    }

    private void UpdateSelection()
    {
        background.color = View.Selected
            ? selectedColor
            : Color.white;
    }

    public void SetSelected(bool selected)
    {
        View.Selected = selected;

        UpdateSelection();
    }
}
