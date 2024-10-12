using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLord_FrostFiend : BossMinionBase
{
    [SerializeField] private Animator _frostFiendAnimator;

    private const string _fiendFrozenAnimTrigger = "FiendFrozen";
    private const string _fiendUnfrozenAnimTrigger = "FiendUnfrozen";

    private const string _fiendBlizzardAnimTrigger = "BlizzardAttack";
    private const string _fiendFrostbiteAnimTrigger = "FrostbiteAttack";

    private bool _minionFrozen;
    private float _freezeDuration;

    public void AdditionalSetup(float freezeDuration)
    {
        _freezeDuration = freezeDuration;
    }

    public void BlizzardAttack()
    {
        BlizzardAttackAnim();
    }

    public void FrostbiteAttack()
    {
        FrostbiteAttackAnim();
    }

    #region Freezing
    public void FreezeMinion()
    {
        if (_minionFrozen) return;

        _minionFrozen = true;
        FreezeAnim();

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
        UnfreezeAnim();
    }

    private void FreezeAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendFrozenAnimTrigger);
    }

    private void UnfreezeAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendUnfrozenAnimTrigger);
    }

    private void BlizzardAttackAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendBlizzardAnimTrigger);
    }

    private void FrostbiteAttackAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendFrostbiteAnimTrigger);
    }

    #endregion

    #region Getters
    public bool IsMinionFrozen() => _minionFrozen;
    #endregion
}
