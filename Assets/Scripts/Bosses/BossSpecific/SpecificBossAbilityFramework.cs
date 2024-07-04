using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the framework for boss abilities
/// </summary>
public abstract class SpecificBossAbilityFramework : MonoBehaviour
{
    [SerializeField] private bool _isTargeted;
    [Space]

    [SerializeField] protected float _timeUntilNextAbility;
    [SerializeField] protected float _targetZoneDuration;
    [SerializeField] protected float _abilityWindUpTime;

    [Space]
    [SerializeField] protected string _animationTriggerName;

    protected List<GameObject> _currentTargetZones = new List<GameObject>();

    protected Vector3 _storedTargetLocation;
    protected HeroBase _storedTarget;

    protected BossBase _ownerBossBase;
    protected SpecificBossFramework _mySpecificBoss;

    /// <summary>
    /// Sets up the ability with the boss base and specific boss framework
    /// </summary>
    /// <param name="bossBase"></param>
    public virtual void AbilitySetup(BossBase bossBase)
    {
        _ownerBossBase = bossBase;
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
    /// Does the initial prep for the ability
    /// Shows the target zone if it has one
    /// </summary>
    /// <param name="targetLocation"></param>
    protected virtual void AbilityPrep()
    {
        StartShowTargetZone();
        StartAbilityWindUp();
    }

    protected virtual void StartBossAbilityAnimation()
    {
        _ownerBossBase.GetBossVisuals().StartBossSpecificAnimationTrigger(_animationTriggerName);
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


    protected virtual IEnumerator AbilityWindUp()
    {
        yield return new WaitForSeconds(_abilityWindUpTime);

        AbilityStart();
    }

    protected virtual void AbilityStart()
    {
        
    }

    

    protected virtual void ProgressToNextBossAttack()
    {

    }

    #region Getters
    public bool GetIsTargeted() => _isTargeted;

    public float GetTimeUntilNextAbility() => _timeUntilNextAbility;
    public float GetAbilityWindUpTime() => _abilityWindUpTime;

    #endregion
}
