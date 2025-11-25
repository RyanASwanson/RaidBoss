using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GlacialLord_FrostFiend : BossMinionBase
{
    [SerializeField] private bool _canBeFrozenDuringFreeze;
    [SerializeField] private float _refreezeCooldown;

    [Space]
    [SerializeField] private float _frozenMaxScaleOverFreeze;
    [SerializeField] private float _frozenMinScaleOverFreeze;
    [SerializeField] private GameObject _frozenEffectBase;

    [Space] 
    [SerializeField] private float _timeFreezeCrackedAtEnd;
    [SerializeField] private GameObject _frozenEffect;
    [SerializeField] private GameObject _frozenEffectCracked;
    [SerializeField] private ParticleSystem _frozenEffectCrackedVFX;
    
    [Space]
    [SerializeField] private Animator _frostFiendAnimator;

    private const string _fiendFrozenAnimTrigger = "FiendFrozen";
    private const string _fiendUnfrozenAnimTrigger = "FiendUnfrozen";

    private const string _fiendBlizzardAnimTrigger = "BlizzardAttack";
    private const string _fiendBlizzardFailedAnimTrigger = "BlizzardFailed";
    private const string _fiendFrostbiteAnimTrigger = "FrostbiteAttack";
    
    private const string _fiendDeathAnimTrigger = "FiendDeath";
    
    private bool _isMinionFrozen;
    private static float _freezeDuration;
    private static float _timeBeforeFreezeCrack;
    
    private float _timeFrozen = 0;
    private bool _isFreezeCracked = false;
    private Coroutine _frozenCoroutine;
    
    private UnityEvent _onMinionFrozen = new UnityEvent();

    private BossTargetZoneParent _currentTargetZone;

    private const int FROST_FIEND_ABILITY_ID = 4;
    
    private const int FROST_FIEND_FROZEN_AUDIO_ID = 0;
    private const int FROST_FIEND_FREEZE_CRACKED_AUDIO_ID = 1;
    private const int FROST_FIEND_UNFROZEN_AUDIO_ID = 2;

    public void AdditionalSetUp(float freezeDuration)
    {
        _freezeDuration = freezeDuration;
        _timeBeforeFreezeCrack = freezeDuration - _timeFreezeCrackedAtEnd;
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

    public void SetCurrentTargetZone(BossTargetZoneParent target)
    {
        _currentTargetZone = target;
    }

    #region Freezing
    public void FreezeMinion()
    {
        if (_isMinionFrozen)
        {
            if (!_canBeFrozenDuringFreeze)
            {
                return;
            }

            if (_timeFrozen < _refreezeCooldown)
            {
                return;
            }
        }

        _isMinionFrozen = true;
        InvokeOnMinionFrozen();

        if (!_currentTargetZone.IsUnityNull())
        {
            _currentTargetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(true);
        }
        
        _isFreezeCracked = false;
        CrackFreezeEffect(_isFreezeCracked);
        
        PlayMinionFrozenAudio();
        FreezeAnim();
        
        StopFreezeProcess();
        _frozenCoroutine = StartCoroutine(FreezeProcess());
    }
    

    private IEnumerator FreezeProcess()
    {
        _timeFrozen = 0;
        while (_timeFrozen < _freezeDuration)
        {
            if (!_isFreezeCracked && _timeFrozen > _timeBeforeFreezeCrack)
            {
                _isFreezeCracked = true;
                CrackFreezeEffect(_isFreezeCracked);
                _frozenEffectCrackedVFX.Play();
                PlayMinionFreezeCrackedAudio();
            }
            _timeFrozen += Time.deltaTime;
            float scaleProgress = Mathf.Lerp(_frozenMaxScaleOverFreeze, _frozenMinScaleOverFreeze, _timeFrozen / _freezeDuration);
            _frozenEffectBase.transform.localScale = new Vector3(scaleProgress, scaleProgress, scaleProgress);
            yield return null;
        }
        
        UnfreezeMinion();
    }

    private void StopFreezeProcess()
    {
        if (!_frozenCoroutine.IsUnityNull())
        {
            StopCoroutine(_frozenCoroutine);
        }
    }

    private void UnfreezeMinion()
    {
        _isMinionFrozen = false;
        PlayMinionUnfrozenAudio();
        UnfreezeAnim();
    }

    private void CrackFreezeEffect(bool isCracked)
    {
        _frozenEffectCracked.SetActive(isCracked);
    }
    #endregion
    
    #region Audio

    private void PlayMinionFrozenAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[FROST_FIEND_ABILITY_ID].GeneralAbilityAudio[FROST_FIEND_FROZEN_AUDIO_ID]);
    }

    private void PlayMinionFreezeCrackedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[FROST_FIEND_ABILITY_ID].GeneralAbilityAudio[FROST_FIEND_FREEZE_CRACKED_AUDIO_ID]);
    }

    private void PlayMinionUnfrozenAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[FROST_FIEND_ABILITY_ID].GeneralAbilityAudio[FROST_FIEND_UNFROZEN_AUDIO_ID]);
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
    
    #region Events

    private void InvokeOnMinionFrozen()
    {
        _onMinionFrozen?.Invoke();
    }
    #endregion

    #region Getters
    public bool IsMinionFrozen() => _isMinionFrozen;
    
    public UnityEvent GetOnMinionFrozen() => _onMinionFrozen;
    #endregion
}
