using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SH_Samurai : SpecificHeroFramework
{
    [Space]
    [SerializeField] int _manualAbilityDuration;

    private bool _isParrying;

    private Coroutine _parryCoroutine;

    #region Basic Abilities
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !myHeroBase.GetPathfinding().IsHeroMoving();
    }

    public override void ActivateBasicAbilities()
    {
        
    }

    #endregion

    #region Manual Abilities
    public override void ActivateManualAbilities(Vector3 attackLocation)
    {
        base.ActivateManualAbilities(attackLocation);

        _parryCoroutine = StartCoroutine(ParryCoroutine());
    }

    private IEnumerator ParryCoroutine()
    {
        _isParrying = true;
        yield return new WaitForSeconds(_manualAbilityDuration);
        _isParrying = false;

        _parryCoroutine = null;

        //JUST TO REMOVE WARNING, DELETE LATER
        if (_isParrying)
            Debug.Log("Test");
    }


    #endregion

    #region Passive Abilities
    public void ActivatePassiveAbilities(float cooldownAmount)
    {
        AddToManualAbilityChargeTime(cooldownAmount);
    }
    #endregion


    public override void ActivateHeroSpecificActivity()
    {
        base.ActivateHeroSpecificActivity();
    }

    public override void DeactivateHeroSpecificActivity()
    {
        base.DeactivateHeroSpecificActivity();
    }

    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    
}