using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the speed at which the game plays
/// </summary>
public class TimeManager : BaseUniversalManager
{
    private List<float> _appliedSlowedTimeVariations = new List<float>();




    /// <summary>
    /// Adds a new value to the appliedTimeVariations
    /// </summary>
    /// <param name="timeVariation"></param> the speed the time is being set to
    /// <param name="duration"></param> duration is relative to the current time scale
    public void AddNewTimeVariationForDuration(float timeVariation, float duration)
    {
        StartCoroutine(AddTimeVariationProcess(timeVariation, duration));
    }

    private IEnumerator AddTimeVariationProcess(float timeVariation, float duration)
    {
        _appliedSlowedTimeVariations.Add(timeVariation);
        DetermineCurrentTimeSpeedBasedOnList();
        yield return new WaitForSeconds(duration);
        _appliedSlowedTimeVariations.Remove(timeVariation);
        DetermineCurrentTimeSpeedBasedOnList();
    }

    private void DetermineCurrentTimeSpeedBasedOnList()
    {
        float currentLowestTime = 1;

        foreach(float timeVar in _appliedSlowedTimeVariations)
        {
            if(timeVar < currentLowestTime)
            {
                currentLowestTime = timeVar;
            }
        }

        SetTimeScale(currentLowestTime);
    }

    public void SetTimeToNormalSpeedOverride()
    {
        SetTimeScale(1);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void FreezeTime()
    {
        SetTimeScale(0);
    }

    public void UnfreezeTime()
    {
        DetermineCurrentTimeSpeedBasedOnList();
    }

    public override void SetupUniversalManager()
    {

    }

    public override void SubscribeToEvents()
    {
        throw new System.NotImplementedException();
    }
}
