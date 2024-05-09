using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossAbilityFramework : MonoBehaviour
{
    [SerializeField] private bool _isTargeted;
    [Space]

    [SerializeField] private float _targetZoneDuration;
    [SerializeField] private float _abilityWindUpTime;

    protected GameObject _currentTargetZone;

    protected BossBase _ownerBossBase;
    protected SpecificBossFramework _mySpecificBoss;


    public virtual void AbilitySetup(BossBase bossBase)
    {
        _ownerBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();
    }

    public virtual void AbilityPrep(Vector3 targetLocation)
    {
        StartShowTargetZone(targetLocation);
        StartAbilityWindUp(targetLocation);
    }

    #region Target Zone
    public virtual void StartShowTargetZone(Vector3 targetLocation)
    {
        StartCoroutine(ShowTargetZone());
    }

    public virtual IEnumerator ShowTargetZone()
    {
        yield return new WaitForSeconds(_targetZoneDuration);
        RemoveTargetZone();
    }

    public virtual void RemoveTargetZone()
    {
        if (_currentTargetZone == null) return;

        Destroy(_currentTargetZone);
    }

    #endregion
    public virtual void StartAbilityWindUp(Vector3 targetLocation)
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

    public float GetAbilityWindUpTime() => _abilityWindUpTime;

    #endregion
}
