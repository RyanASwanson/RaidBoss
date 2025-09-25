using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlacialLord_FrostFiend : BossMinionBase
{
    [SerializeField] private bool _canBeFrozenDuringFreeze;
    
    [SerializeField] private Animator _frostFiendAnimator;

    private const string _fiendFrozenAnimTrigger = "FiendFrozen";
    private const string _fiendUnfrozenAnimTrigger = "FiendUnfrozen";

    private const string _fiendBlizzardAnimTrigger = "BlizzardAttack";
    private const string _fiendBlizzardFailedAnimTrigger = "BlizzardFailed";
    private const string _fiendFrostbiteAnimTrigger = "FrostbiteAttack";
    
    private const string _fiendDeathAnimTrigger = "FiendDeath";

    private bool _minionFrozen;
    private float _freezeDuration;
    
    private WaitForSeconds _freezeWait;

    public void AdditionalSetUp(float freezeDuration)
    {
        _freezeDuration = freezeDuration;
        _freezeWait = new WaitForSeconds(_freezeDuration);
    }

    public void BlizzardAttack()
    {
        BlizzardAttackAnim();
    }

    public void BlizzardFailed()
    {
        BlizzardFailedAnim();
    }

    public void FrostbiteAttack()
    {
        FrostbiteAttackAnim();
    }

    public void FrostFiendDeath()
    {
        DeathAnim();
    }

    #region Freezing
    public void FreezeMinion()
    {
        if (!_canBeFrozenDuringFreeze && _minionFrozen)
        {
            return;
        }

        _minionFrozen = true;
        FreezeAnim();

        StartCoroutine(FreezeProcess());
    }

    private IEnumerator FreezeProcess()
    {
        yield return _freezeWait;
        UnfreezeMinion();
    }

    private void UnfreezeMinion()
    {
        _minionFrozen = false;
        UnfreezeAnim();
    }
    #endregion

    #region Animations
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

    private void BlizzardFailedAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendBlizzardFailedAnimTrigger);
    }

    private void FrostbiteAttackAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendFrostbiteAnimTrigger);
    }

    private void DeathAnim()
    {
        _frostFiendAnimator.SetTrigger(_fiendDeathAnimTrigger);
    }

    #endregion

    #region Getters
    public bool IsMinionFrozen() => _minionFrozen;
    #endregion
}
