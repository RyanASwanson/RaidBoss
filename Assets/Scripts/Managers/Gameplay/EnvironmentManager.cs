using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : BaseGameplayManager
{
    [SerializeField] private List<GameObject> _heroSpawnLocations;

    // Start is called before the first frame update
    void Start()
    {
        
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
