using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides functionality for the hero UI to be clicked in order to select heroes
/// </summary>
public class IndividualHeroUIExtraFunctionality : MonoBehaviour
{
    [Tooltip("The id of the associated hero")]
    [SerializeField] private int _heroID;

    public void HeroUIPressed()
    {
        PlayerInputGameplayManager.Instance.NewControlledHeroByID(_heroID);
    }
}
