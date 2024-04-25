using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : BaseGameplayManager
{
    [SerializeField] private List<GameObject> _heroSpawnLocations;

    public override void SetupGameplayManager()
    {
        base.SetupGameplayManager();
    }

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public List<GameObject> GetSpawnLocations() => _heroSpawnLocations;
    #endregion

    #region Setters

    #endregion
}
