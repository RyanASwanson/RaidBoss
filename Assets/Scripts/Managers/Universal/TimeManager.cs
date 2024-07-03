using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the speed at which the game plays
/// </summary>
public class TimeManager : BaseUniversalManager
{
    [SerializeField] private bool _useSlowestTime;

    private List<float> _appliedTimeVariations = new List<float>();


    public override void SetupUniversalManager()
    {
        
    }

    /// <summary>
    /// Adds a new value to the appliedTimeVariations
    /// </summary>
    /// <param name="timeVariation"></param> the speed the time is being set to
    /// <param name="duration"></param> duration is relative to the current time scale
    public void AddNewTimeVariationForDuration(float timeVariation, float duration)
    {

    }

    public void SetTimeToNormalSpeed()
    {
        Time.timeScale = 1;
    }

    public void ReturnToPreviousSpeed()
    {

    }

    public void FreezeTime()
    {
        Time.timeScale = 0;
    }
}
