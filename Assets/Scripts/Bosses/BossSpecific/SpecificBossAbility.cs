using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecificBossAbility : MonoBehaviour
{
    [SerializeField] private bool _isTargeted;
    [Space]

    [SerializeField] private float _abilityWindUpTime;



    public virtual void AbilityPrep()
    {
        ShowTargetZone();
        StartCoroutine(AbilityWindUp());
    }

    public virtual void ShowTargetZone()
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
