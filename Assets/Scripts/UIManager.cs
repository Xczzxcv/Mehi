using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BattleFieldController battleField;
    [SerializeField] private Transform uiRoot;

    public void Init(GameManager.GameConfig config, BattleManager battleManager)
    {
        battleField.Init(uiRoot);
        battleField.Setup(config, battleManager);
    }
}