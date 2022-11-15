using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BattleFieldScreenController battleFieldScreen;
    [SerializeField] private Transform uiRoot;

    public void Init(BattleManager battleManager)
    {
        battleFieldScreen.Init(uiRoot, battleManager);
        battleFieldScreen.Setup();
    }
}