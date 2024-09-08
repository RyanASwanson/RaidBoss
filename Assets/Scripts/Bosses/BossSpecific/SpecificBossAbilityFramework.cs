using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework for boss abilities
/// </summary>
public abstract class SpecificBossAbilityFramework : MonoBehaviour
{
    [SerializeField] protected BossAbilityTargetMethod _targetMethod;

    [Space]
    [SerializeField] protected Vector3 _specificAreaTarget;
    [SerializeField] protected Vector3 _specificLookTarget;
    [Space]

    
    [SerializeField] protected float _targetZoneDuration;
    [SerializeField] protected float _abilityWindUpTime;
    [SerializeField] protected float _timeUntilNextAbility;

    [Space]
    [SerializeField] protected string _animationTriggerName;

    protected List<GameObject> _currentTargetZones = new List<GameObject>();

    protected Vector3 _storedTargetLocation;
    protected HeroBase _storedTarget;

    protected BossBase _myBossBase;
    protected SpecificBossFramework _mySpecificBoss;

    /// <summary>
    /// Sets up the ability with the boss base and specific boss framework
    /// </summary>
    /// <param name="bossBase"></param>
    public virtual void AbilitySetup(BossBase bossBase)
    {
        _myBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();

        _timeUntilNextAbility /= UniversalManagers.Instance.GetSelectionManager().GetSpeedMultiplierFromDifficulty();
    }

    /// <summary>
    /// Function that is called to activate the boss ability
    /// Stores the target location / target hero
    /// </summary>
    /// <param name="targetLocation"></param>
    public virtual void ActivateAbility(Vector3 targetLocation, HeroBase targetHeroBase)
    {
        _storedTargetLocation = targetLocation;
        if (targetHeroBase != null)
            _storedTarget = targetHeroBase;

        AbilityPrep();

        StartBossAbilityAnimation();
    }

    /// <summary>
    /// Does the initial preparation for the ability to be used
    /// Shows the target zone if it has one
    /// </summary>
    /// <param name="targetLocation"></param>
    protected virtual void AbilityPrep()
    {
        StartShowTargetZone();
        StartAbilityWindUp();
    }

    /// <summary>
    /// Starts the animation associated with the boss attack
    /// </summary>
    protected virtual void StartBossAbilityAnimation()
    {
        _myBossBase.GetBossVisuals().StartBossSpecificAnimationTrigger(_animationTriggerName);
    }

    #region Target Zone
    /// <summary>
    /// Starts the process of visualizing the target zone
    /// </summary>
    /// <param name="targetLocation"></param>
    protected virtual void StartShowTargetZone()
    {
        StartCoroutine(TargetZonesProcess());
    }

    /// <summary>
    /// Wait for the target zone duration
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator TargetZonesProcess()
    {
        yield return new WaitForSeconds(_targetZoneDuration);
        RemoveTargetZone();
    }

    /// <summary>
    /// Removes all target zones
    /// </summary>
    protected virtual void RemoveTargetZone()
    {
        if (_currentTargetZones.Count == 0) return;

        //Iterates through all target zones and removes them
        foreach(GameObject currentZone in _currentTargetZones)
        {
            Destroy(currentZone.gameObject);
        }
    }

    #endregion

    /// <summary>
    /// Starts the actual activation of the ability
    /// Happens after any prep that is needed such as targeting zones
    /// </summary>
    /// <param name="targetLocation"></param>
    protected virtual void StartAbilityWindUp()
    {
        StartCoroutine(AbilityWindUp());
    }

    /// <summary>
    /// Provides a delay between the ability wind up and the ability start
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AbilityWindUp()
    {
        yield return new WaitForSeconds(_abilityWindUpTime);

        AbilityStart();
    }

    /// <summary>
    /// Starts the ability
    /// Generally spawns the damage area of the attack
    /// </summary>
    protected virtual void AbilityStart()
    {
        
    }


    /// <summary>
    /// Provides the target for an ability with a specific hero target
    /// </summary>
    /// <returns></returns>
    protected virtual HeroBase GetSpecificHeroTarget()
    {
        return null;
    }


    #region Getters
    public BossAbilityTargetMethod GetTargetMethod() => _targetMethod;
    public Vector3 GetSpecificAreaTarget() => _specificAreaTarget;
    public Vector3 GetSpecificLookTarget() => _specificLookTarget;

    public float GetTimeUntilNextAbility() => _timeUntilNextAbility;
    public float GetAbilityWindUpTime() => _abilityWindUpTime;

    #endregion
}

public enum BossAbilityTargetMethod
{
    _heroTarget,
    _specificHeroTarget,
    _specificAreaTarget,
    
};
