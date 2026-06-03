using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_ImpendingStorm : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _baseRotationSpeed;
    [SerializeField] private float[] _difficultyRotationMultiplier;

    [Space] 
    [SerializeField] private float _overchargeRotationSpeedMultiplier;
    [SerializeField] private float _overchargeRotationDecayTime;
    
    [SerializeField] private float _enrageOverchargeEnrageSpeedMultiplierFlatIncrease;
    [SerializeField] private float _enrageOverchargeRotationDecayTime;

    [SerializeField] private AnimationCurve _overchargeDecayCurve;

    [Space]
    [SerializeField] private float _maxBossHealthRotationMultiplier;
    [SerializeField] private AnimationCurve _bossHealthRotationMultiplierCurve;

    [Space] 
    [SerializeField] private float _enrageRotationMultplier;
    [SerializeField] private float _enrageScalingRotationMultiplierIncreasePerMinute;
    
    
    private float _battleStartRotationSpeed;
    private float _currentOverchargeRotationMultiplier = 1;
    private float _currentBossHealthRotationMultiplier = 1;
    private float _currentEnrageRotationMultiplier = 1;

    private float _attackRotation = 0;

    [Space] 
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _enrageAttackSpeedMultiplier;
    [SerializeField] private float _attackSpawnOffset;
    private float _attackTimer;
    private float _currentEnrageAttackSpeedMultiplier = 1;
    
    [Space]
    [SerializeField] private GameObject _impendingStormTargetZone;
    [SerializeField] private GameObject _impendingStormProjectile;
    [SerializeField] private SBA_Overcharge _overchargeAbility;
    private BossTargetZoneParent _currentImpendingStormTargetZone;
    
    private Coroutine _rotationCoroutine;
    private Coroutine _attackCoroutine;
    private Coroutine _overchargeSpeedDecayCoroutine;
    
    public const int IMPENDING_STORM_ATTACK_AUDIO_ID = 0;

    public GameObject BattleStart()
    {
        _battleStartRotationSpeed = _baseRotationSpeed * _difficultyRotationMultiplier[(int)SelectionManager.Instance.GetSelectedDifficulty()-1];

        CreateImpendingStormTargetZone();
        
        StartImpendingStorm();

        return _currentImpendingStormTargetZone.gameObject;
    }

    public void ActivateOvercharge(bool wasAbilityActivatedWhileEnraged)
    {
        RotateImpendingStorm(180);
        StartOverchargeRotationMultiplierDecay(wasAbilityActivatedWhileEnraged);
    }

    private void CreateImpendingStormTargetZone()
    {
        _currentImpendingStormTargetZone = 
            Instantiate(_impendingStormTargetZone, transform.position, Quaternion.identity)
                .GetComponent<BossTargetZoneParent>();
        
        _currentImpendingStormTargetZone.transform.position = new Vector3(_currentImpendingStormTargetZone.transform.position.x,
            _specificAreaTarget.y, _currentImpendingStormTargetZone.transform.position.z);
    }

    private void StartImpendingStorm()
    {
        StartRotateImpendingStorm();
        StartImpendingStormAttack();
    }

    private void StopImpendingStorm()
    {
        StopRotateImpendingStorm();
        StopImpendingStormAttack();
    }

    private void StartRotateImpendingStorm()
    {
        if (_rotationCoroutine.IsUnityNull())
        {
            StopRotateImpendingStorm();
        }

        _rotationCoroutine = StartCoroutine(RotateImpendingStormProcess());
    }

    private void StopRotateImpendingStorm()
    {
        if (!_rotationCoroutine.IsUnityNull())
        {
            StopCoroutine(_rotationCoroutine);
        }
    }

    private IEnumerator RotateImpendingStormProcess()
    {
        while (true)
        {
            RotateImpendingStorm(GetCurrentAttackRotation() * Time.deltaTime);
            
            yield return null;
        }
    }

    private void RotateImpendingStorm(float rotationAmount)
    {
        _attackRotation += rotationAmount;
        if (_attackRotation > 360)
        {
            _attackRotation -= 360;
        }
        UpdateTargetZone();
    }

    private void StartOverchargeRotationMultiplierDecay(bool wasAbilityActivatedWhileEnraged)
    {
        StopOverchargeRotationMultiplierDecay();
        
        if (wasAbilityActivatedWhileEnraged)
        {
            _currentOverchargeRotationMultiplier = _overchargeRotationSpeedMultiplier + _enrageOverchargeEnrageSpeedMultiplierFlatIncrease;
            StartCoroutine(OverchargeRotationMultiplierDecayProcess(_enrageOverchargeRotationDecayTime));
        }
        else
        {
            _currentOverchargeRotationMultiplier = _overchargeRotationSpeedMultiplier;
            StartCoroutine(OverchargeRotationMultiplierDecayProcess(_overchargeRotationDecayTime));
        }
    }

    private void StopOverchargeRotationMultiplierDecay()
    {
        if (!_overchargeSpeedDecayCoroutine.IsUnityNull())
        {
            _currentOverchargeRotationMultiplier = _overchargeRotationSpeedMultiplier;
            StopCoroutine(_overchargeSpeedDecayCoroutine);
        }
    }

    private IEnumerator OverchargeRotationMultiplierDecayProcess(float decaySpeed)
    {
        if (_currentOverchargeRotationMultiplier <= 1)
        {
            yield break;
        }
        
        float startRotationMultiplier = _currentOverchargeRotationMultiplier;
        float overchargeDecayProgress = 0;
        
        while (overchargeDecayProgress < 1)
        {
            overchargeDecayProgress += Time.deltaTime / decaySpeed;
            _currentOverchargeRotationMultiplier = Mathf.Lerp(1, startRotationMultiplier,_overchargeDecayCurve.Evaluate(overchargeDecayProgress));
            yield return null;
        }
        
        _currentOverchargeRotationMultiplier = 1;
    }
    
    private void UpdateTargetZone()
    {
        if (!CheckForTargetZone())
        {
            return;
        }
        
        _currentImpendingStormTargetZone.transform.eulerAngles = new Vector3(0, _attackRotation, 0);
    }

    private bool CheckForTargetZone()
    {
        if (_currentImpendingStormTargetZone.IsUnityNull())
        {
            StopImpendingStorm();
            return false;
        }

        return true;
    }
    
    private void StartImpendingStormAttack()
    {
        if (_attackCoroutine.IsUnityNull())
        {
            StopImpendingStormAttack();
        }

        _attackCoroutine = StartCoroutine(ImpendingStormAttack());
    }

    private void StopImpendingStormAttack()
    {
        if (!_attackCoroutine.IsUnityNull())
        {
            StopCoroutine(_attackCoroutine);
        }
    }
    
    private IEnumerator ImpendingStormAttack()
    {
        while (true)
        {
            _attackTimer += _currentEnrageAttackSpeedMultiplier * Time.deltaTime;
            if (_attackTimer >= _attackDelay)
            {
                _attackTimer -= _attackDelay;
                SpawnImpendingStormProjectile();
            }
            yield return null;
        }
    }

    private void SpawnImpendingStormProjectile()
    {
        GameObject impendingStormProjectile = Instantiate(_impendingStormProjectile, _specificLookTarget, Quaternion.identity);
        impendingStormProjectile.transform.eulerAngles = new Vector3(0,_attackRotation,0);
        impendingStormProjectile.transform.position += impendingStormProjectile.transform.forward * _attackSpawnOffset;

        if (impendingStormProjectile.TryGetComponent(out SBP_ImpendingStorm impendingStorm))
        {
            impendingStorm.SetUpProjectile(_myBossBase, _abilityID);
        }
        
        PlayAttackAudio();
    }

    private void PlayAttackAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_ImpendingStorm.IMPENDING_STORM_ATTACK_AUDIO_ID]);
    }

    private void BossDamaged(float damage)
    {
        UpdateHealthMultiplierRotationSpeed();
    }

    private void UpdateHealthMultiplierRotationSpeed()
    {
        _currentBossHealthRotationMultiplier = _bossHealthRotationMultiplierCurve.Evaluate(1 - BossStats.Instance.GetBossHealthPercentage());
        _currentBossHealthRotationMultiplier = Mathf.Lerp(1, _maxBossHealthRotationMultiplier, _currentBossHealthRotationMultiplier);
    }

    private void UpdateEnrageMultiplierRotationSpeed()
    {
        _currentEnrageRotationMultiplier = _enrageRotationMultplier;
        _currentEnrageRotationMultiplier *= 1 + (_enrageScalingRotationMultiplierIncreasePerMinute * BossStats.Instance.GetMinutesSpentEnraged());
    }

    private void BossEnraged()
    {
        _currentEnrageAttackSpeedMultiplier = _enrageAttackSpeedMultiplier;
    }

    private void BossStaggered()
    {
        StopImpendingStormAttack();
    }

    private void BossNoLongerStaggered()
    {
        StartImpendingStormAttack();
    }

    private void BattleOver()
    {
        StopImpendingStorm();
        
        _overchargeAbility.StopBossAbility();
        _currentImpendingStormTargetZone.RemoveBossTargetZones();
    }

    public override void SubscribeToEvents()
    {
        if (_isSubscribedToEvents)
        {
            return;
        }
        
        base.SubscribeToEvents();
        _myBossBase.GetBossDamagedEvent().AddListener(BossDamaged);
        
        _myBossBase.GetBossStaggeredEvent().AddListener(BossStaggered);
        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggered);
        
        _myBossBase.GetBossEnragedEvent().AddListener(BossEnraged);
        _myBossBase.GetSecondPassedEnrageEvent().AddListener(UpdateEnrageMultiplierRotationSpeed);
        
        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(BattleOver);
    }

    public override void UnsubscribeFromEvents()
    {
        if (!_isSubscribedToEvents)
        {
            return;
        }
        
        base.UnsubscribeFromEvents();
        
        _myBossBase.GetBossDamagedEvent().RemoveListener(BossDamaged);
        
        _myBossBase.GetBossStaggeredEvent().RemoveListener(BossStaggered);
        _myBossBase.GetBossNoLongerStaggeredEvent().RemoveListener(BossNoLongerStaggered);
        
        _myBossBase.GetBossEnragedEvent().RemoveListener(BossEnraged);
        _myBossBase.GetSecondPassedEnrageEvent().RemoveListener(UpdateEnrageMultiplierRotationSpeed);
        
        GameStateManager.Instance.GetBattleWonOrLostEvent().RemoveListener(BattleOver);
    }
    
    #region Getters

    public float GetAttackRotation() => _attackRotation;
    public float GetCurrentAttackRotation() => _battleStartRotationSpeed * _currentOverchargeRotationMultiplier 
                                                                         * _currentBossHealthRotationMultiplier 
                                                                         * _currentEnrageRotationMultiplier;

    public float GetOppositeAttackRotation() => _attackRotation + 180;
    
    public Vector3 GetImpendingStormDirection() => _currentImpendingStormTargetZone.transform.forward;

    #endregion
}
