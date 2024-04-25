using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStats : HeroChildrenFunctionality
{
    private float _heroMaxHealth;
    private float _heroBaseMovespeed;
    private float _heroBaseAggro;

    private float _currentHealth;
    private float _currentMovespeed;
    private float _currentAggro;

    #region Events
    public override void SubscribeToEvents()
    {
        
    }
    #endregion

    #region Getters
    public float GetMaxHealth() => _heroMaxHealth;
    #endregion

    #region Setters

    #endregion
}
