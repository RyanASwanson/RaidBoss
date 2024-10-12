using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLord_FrostFiend : BossMinionBase
{
    [SerializeField] private Animator _frostFiendAnimator;

    private const string _fiendFrozenAnimTrigger = "FiendFrozen";
    private const string _fiendUnfrozenAnimTrigger = "FiendUnfrozen";

    private bool _minionFrozen;
    private float _freezeDuration;

    public void AdditionalSetup(float freezeDuration)
    {
        _freezeDuration = freezeDuration;
    }

    #region Freezing
    public void FreezeMinion()
    {
        if (_minionFrozen) return;

        _minionFrozen = true;
        FreezeAnimationTrigger();

        StartCoroutine(FreezeProcess());
    }

    private IEnumerator FreezeProcess()
    {
        yield return new WaitForSeconds(_freezeDuration);
        UnfreezeMinion();
    }

    private void UnfreezeMinion()
    {
        _minionFrozen = false;
        UnfreezeAnimationTrigger();
    }

    private void FreezeAnimationTrigger()
    {
        _frostFiendAnimator.SetTrigger(_fiendFrozenAnimTrigger);
    }

    private void UnfreezeAnimationTrigger()
    {
        _frostFiendAnimator.SetTrigger(_fiendUnfrozenAnimTrigger);
    }

    #endregion

    #region Getters
    public bool IsMinionFrozen() => _minionFrozen;
    #endregion
}
