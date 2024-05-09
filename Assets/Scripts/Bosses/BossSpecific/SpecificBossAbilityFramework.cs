using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossAbilityFramework : MonoBehaviour
{
    [SerializeField] private bool _isTargeted;
    [Space]

    [SerializeField] private float _abilityWindUpTime;

    protected BossBase _ownerBossBase;
    protected SpecificBossFramework _mySpecificBoss;


    public virtual void AbilitySetup(BossBase bossBase)
    {
        _ownerBossBase = bossBase;
        _mySpecificBoss = bossBase.GetSpecificBossScript();
    }

    public virtual void AbilityPrep(Vector3 targetLocation)
    {
        ShowTargetZone(targetLocation);
        StartCoroutine(AbilityWindUp());
    }

    public virtual void ShowTargetZone(Vector3 targetLocation)
    {

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
