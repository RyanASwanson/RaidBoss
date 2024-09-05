using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides functionality for the hero UI to be clicked in order to select heroes
/// </summary>
public class IndividualHeroUIExtraFunctionality : MonoBehaviour
{
    [SerializeField] private int _heroID;

    public void HeroUIPressed()
    {
        GameplayManagers.Instance.GetPlayerInputManager().NewControlledHeroByID(_heroID);
    }
}
