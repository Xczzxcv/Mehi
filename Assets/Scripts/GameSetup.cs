using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSetup", fileName = "GameSetup", order = 0)]
internal class GameSetup : ScriptableObject
{
    public GameManager.GameConfig gameConfig;
    public List<EntitiesFactory.MechConfig> mechConfigs;
    public List<WeaponConfig> weaponConfigs;
}