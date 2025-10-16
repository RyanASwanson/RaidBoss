using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// The functionality for the Mirage clone.
/// Treated as if it was a hero
/// </summary>
public class MirageClone : SpecificHeroFramework
{
    private Coroutine _basicAttackCooldownProcess;

    private SH_Mirage _mirageOwner;

    #region Basic
    /*/// <summary>
    /// Starts the process of the clone casting it's own copy of the Mirage
    ///     basic ability
    /// </summary>
    protected override void StartCooldownBasicAbility()
    {
        base.StartCooldownBasicAbility();
    }

    /// <summary>
    /// Only allows for the clone to use its basic ability while moving
    /// </summary>
    /// <returns></returns>
    public override bool ConditionsToActivateBasicAbilities()
    {
        return !base.ConditionsToActivateBasicAbilities();
    }

    /// <summary>
    /// Disables the ability to cast the Mirage basic ability
    /// Called when it stops moving
    /// </summary>
    private void EndBasicCastProcess()
    {
        StopCooldownBasicAbility();
    }*/

    private IEnumerator ReHitCooldown()
    {
        yield return new WaitForSeconds(_basicAbilityChargeTime);
        _basicAbilityCooldownCoroutine = null;
    }
    #endregion

    #region Manual
    public void CloneSwap(Transform heroTransform)
    {
        TriggerManualAbilityAnimation();

        _myHeroBase.transform.position = heroTransform.position;
        _myHeroBase.transform.rotation = heroTransform.rotation;
    }

    private void CloneCounterAttack(float damage)
    {
        if(_basicAbilityCooldownCoroutine.IsUnityNull())
        {
            ActivateBasicAbilities();
            _basicAbilityCooldownCoroutine = StartCoroutine(ReHitCooldown());
        }
    }

    public override void ActivateBasicAbilities()
    {
        //base.ActivateBasicAbilities();
        TriggerBasicAbilityAnimation();
        _mirageOwner.CloneBasicAbility();
    }
    #endregion

    /// <summary>
    /// The mirage clone adds itself as a possible target for boss attacks
    /// Heroes are already set as targets, but as this is an ability it has to do so manually
    /// </summary>
    private void AssignSelfAsBossTarget()
    {
        BossBase.Instance.GetSpecificBossScript().AddHeroTarget(_myHeroBase);
    }

    #region Base Hero
    /// <summary>
    /// Performs set up unique to the mirage
    /// </summary>
    /// <param name="mirage"> The hero associated with the clone </param>
    public void AdditionalSetUp(SH_Mirage mirage)
    {
        _mirageOwner = mirage;

        _myHeroBase.SetClickColliderStatus(false);
        
        _myHeroBase.GetHeroStats().AddDamageTakenOverrideCounter();
        _myHeroBase.GetHeroStats().AddHealingTakenOverrideCounter();
    }

    public override void ActivateHeroSpecificActivity()
    {
        //Overrides the base to do nothing
    }

    protected override void SubscribeToEvents()
    {
        //_myHeroBase.GetHeroStartedMovingEvent().AddListener(StartCooldownBasicAbility);

        //Stops the clone from using the basic ability after it stops moving
        //_myHeroBase.GetHeroStoppedMovingEvent().AddListener(EndBasicCastProcess);

        BossBase.Instance.GetBossTargetsAssignedEvent().AddListener(AssignSelfAsBossTarget);

        _myHeroBase.GetHeroDamagedOverrideEvent().AddListener(CloneCounterAttack);
        base.SubscribeToEvents();
    }
    #endregion
}
