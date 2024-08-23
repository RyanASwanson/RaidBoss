using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualHeroUIExtraFunctionality : MonoBehaviour
{
    [SerializeField] private int _heroID;

    public void HeroUIPressed()
    {
        GameplayManagers.Instance.GetPlayerInputManager().NewControlledHeroByID(_heroID);
    }
}
