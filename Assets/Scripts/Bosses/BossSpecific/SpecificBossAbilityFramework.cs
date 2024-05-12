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

    [SerializeField] private float _timeUntilNextAbility;
    [SerializeField] private float _targetZoneDuration;
    [SerializeField] private float _abilityWindUpTime;

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
    }

    /// <summary>
    /// Function that is called to activate the boss ability
    /// </summary>
    /// <param name="targetLocation"></param>
    public virtual void ActivateAbility(Vector3 targetLocation, HeroBase targetHeroBase)
    {
        _storedTargetLocation = targetLocation;
        _storedTarget = targetHeroBase;

        AbilityPrep();
    }

    /// <summary>
    /// Does the initial prep for the ability
    /// Shows the target zone if it has one
    /// </summary>
    /// <param name="targetLocation"></param>
    public virtual void AbilityPrep()
    {
        StartShowTargetZone();
        StartAbilityWindUp();
    }

    #region Target Zone
    /// <summary>
    /// Starts the process of visualizing the target zone
    /// </summary>
    /// <param name="targetLocation"></param>
    public virtual void StartShowTargetZone()
    {
        StartCoroutine(TargetZonesProcess());
    }

    /// <summary>
    /// Wait for the target zone duration
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator TargetZonesProcess()
    {
        yield return new WaitForSeconds(_targetZoneDuration);
        RemoveTargetZone();
    }

    /// <summary>
    /// Removes all target zones
    /// </summary>
    public virtual void RemoveTargetZone()
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
    public virtual void StartAbilityWindUp()
    {
        StartCoroutine(AbilityWindUp());
    }


    public virtual IEnumerator AbilityWindUp()
    {
        yield return new WaitForSeconds(_abilityWindUpTime);

        AbilityStart();
    }

    public virtual void AbilityStart()
    {
        
    }

    

    public virtual void ProgressToNextBossAttack()
    {

    }

    #region Getters
    public bool GetIsTargeted() => _isTargeted;

    public float GetTimeUntilNextAbility() => _timeUntilNextAbility;
    public float GetAbilityWindUpTime() => _abilityWindUpTime;

    #endregion
}
