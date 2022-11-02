using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EcsManager ecsManager;
    [SerializeField] private List<EntitiesFactory.MechConfig> mechConfigs;

    private void Start()
    {

        foreach (var mechConfig in mechConfigs)
        {
            EntitiesFactory.BuildMechEntity(ecsManager.World, mechConfig);
        }
    }
}