using UnityEngine;

internal class BattleFieldController : MonoBehaviour
{
    [SerializeField] private BattleFieldPresenter fieldPrefab;
    
    private Transform _uiRoot;

    public void Init(Transform uiRoot)
    {
        _uiRoot = uiRoot;
    }

    public void Setup(GameManager.GameConfig config, BattleManager battleManager)
    {
        var battleFieldPresenter = Instantiate(fieldPrefab, _uiRoot);
        battleFieldPresenter.Setup(new BattleFieldPresenter.ViewInfo
        {
            FieldSize = config.BattleFieldSize,
            Field = battleManager.GetField(),
            UnitInfos = battleManager.GetPlayerUnitInfos(),
        });
    }
}