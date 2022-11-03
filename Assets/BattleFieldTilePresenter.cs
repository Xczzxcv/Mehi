using UnityEngine;
using UnityEngine.UI;

public class BattleFieldTilePresenter : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image foregroundImage;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Transform contentParent;

    public void Init(ToggleGroup parentGroup)
    {
        toggle.group = parentGroup;
    }

    public void Setup(Sprite background, Sprite foreground)
    {
        backgroundImage.sprite = background;
        foregroundImage.sprite = foreground;
    }

    public void SetupContent(Transform contentRoot)
    {
        contentRoot.SetParent(contentParent, false);
    }
}
