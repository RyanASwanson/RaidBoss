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
    [Range(0,1)][SerializeField] private float _freezeProgressRequiredForIceCrack;
    [SerializeField] private GameObject _frozenEffect;
    [SerializeField] private GameObject _frozenEffectCracked;
    [SerializeField] private ParticleSystem _frozenEffectCrackedVFX;
    
    [Space]
    [SerializeField] private GeneralVFXFunctionality[] _blizzardVFXFunctionality;
    private int _blizzardVFXUsed = 0;
    private int _blizzardSucessCounter = 0;
    
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
    
    private float _timeFrozen = 0;
    private bool _isFreezeCracked = false;
    private Coroutine _frozenCoroutine;
    
    private UnityEvent _onMinionFrozen = new UnityEvent();
    
    private List<BossTargetZoneParent> _currentTargetZones = new List<BossTargetZoneParent>();

    private const int FROST_FIEND_ABILITY_ID = 4;
    
    private const int FROST_FIEND_FROZEN_AUDIO_ID = 0;
    private const int FROST_FIEND_FREEZE_CRACKED_AUDIO_ID = 1;
    private const int FROST_FIEND_UNFROZEN_AUDIO_ID = 2;

    private bool _hasFrostFiendAttacked = false;

    public void SetUpMinionFreezeDuration(float freezeDuration)
    {
        _freezeDuration = freezeDuration;
    }

    public void BlizzardGroupAttackSucceeded()
    {
        _blizzardSucessCounter++;
    }

    public void BlizzardGroupAttackFailed()
    {
        _blizzardSucessCounter--;
    }

    public void DetermineBlizzardStateFromSuccessCounter()
    {
        if (_isMinionFrozen)
        {
            return;
        }
        
        if (_blizzardSucessCounter >= 0)
        {
            BlizzardAttack();
            return;
        }
        BlizzardFailed();
    }
    
    private void BlizzardAttack()
    {
        BlizzardAttackAnim();
    }

    private void BlizzardFailed()
    {
        BlizzardFailedAnim();
    }

    public void PlayBlizzardMinionEffect(Vector3 targetLocation)
    {
        _blizzardVFXFunctionality[_blizzardVFXUsed].transform.LookAt(targetLocation);
        _blizzardVFXFunctionality[_blizzardVFXUsed].transform.eulerAngles = new Vector3(0, _blizzardVFXFunctionality[_blizzardVFXUsed].transform.eulerAngles.y, 0);

        _blizzardVFXFunctionality[_blizzardVFXUsed].PlayAllParticleSystems();
        _blizzardVFXUsed++;
    }

    public void BlizzardEnded()
    {
        ResetTargetZones();
        _blizzardVFXUsed = 0;
        _blizzardSucessCounter = 0;
    }

    public void FrostbiteAttack()
    {
        FrostbiteAttackAnim();
        GeneralMinionAttack();
    }

    private void GeneralMinionAttack()
    {
        ResetTargetZones();
    }

    public void FrostFiendDeath()
    {
        DeathAnim();
    }

    public void AddTargetZone(BossTargetZoneParent target)
    {
        _currentTargetZones.Add(target);
    }

    public void ResetTargetZones()
    {
        _currentTargetZones.Clear();
    }

    #region Freezing
    public bool FreezeMinion()
    {
        if (_isMinionFrozen)
        {
            if (!_canBeFrozenDuringFreeze)
            {
                return false;
            }

            if (_timeFrozen < _refreezeCooldown)
            {
                return false;
            }
        }

        _isMinionFrozen = true;
        InvokeOnMinionFrozen();

        DeactivateAssociatedTargetZones();
        
        _isFreezeCracked = false;
        CrackFreezeEffect(_isFreezeCracked);
        
        PlayMinionFrozenAudio();
        FreezeAnim();
        
        StopFreezeProcess();
        _frozenCoroutine = StartCoroutine(FreezeProcess());

        return true;
    }

    private void DeactivateAssociatedTargetZones()
    {
        foreach (BossTargetZoneParent targetZone in _currentTargetZones)
        {
            if (targetZone.IsUnityNull())
            {
                continue;
            }
            targetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(true);
        }
    }
    

    private IEnumerator FreezeProcess()
    {
        _timeFrozen = 0;
        while (_timeFrozen < _freezeDuration)
        {
            if (!_isFreezeCracked && (_timeFrozen/_freezeDuration > _freezeProgressRequiredForIceCrack))
            {
                _isFreezeCracked = true;
                CrackFreezeEffect(_isFreezeCracked);
                _frozenEffectCrackedVFX.Play();
                PlayMinionFreezeCrackedAudio();
            }
            
            _timeFrozen +=  SB_GlacialLord.Instance.GetMinionUnfreezeSpeedMultiplier() * Time.deltaTime;
            
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

    private void ResetBlizzardAnims()
    {
        _frostFiendAnimator.ResetTrigger(_fiendBlizzardAnimTrigger);
        _frostFiendAnimator.ResetTrigger(_fiendBlizzardFailedAnimTrigger);
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
    
    public bool GetHasFrostFiendAttacked() => _hasFrostFiendAttacked;
    #endregion
    
    #region Setters
    public void SetHasFrostFiendAttacked(bool hasAttacked) => _hasFrostFiendAttacked = hasAttacked;
    #endregion 
}
