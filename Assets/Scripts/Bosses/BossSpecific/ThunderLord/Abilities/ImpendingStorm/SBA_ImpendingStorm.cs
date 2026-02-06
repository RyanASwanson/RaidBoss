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

    [SerializeField] private float _maxBossHealthRotationMultiplier;
    [SerializeField] private AnimationCurve _bossHealthRotationMultiplierCurve;

    private float _battleStartRotationSpeed;
    private float _rotationSpeed;

    private float _attackRotation = 0;

    [Space] 
    [SerializeField] private float _attackDelay;
    [SerializeField] private float _attackSpawnOffset;
    private float _attackTimer;
    
    [Space]
    [SerializeField] private GameObject _impendingStormTargetZone;
    [SerializeField] private GameObject _impendingStormProjectile;
    private BossTargetZoneParent _currentImpendingStormTargetZone;
    
    private Coroutine _rotationCoroutine;
    private Coroutine _attackCoroutine;

    public GameObject BattleStart()
    {
        SubscribeToEvents();
        
        _battleStartRotationSpeed = _baseRotationSpeed * _difficultyRotationMultiplier[(int)SelectionManager.Instance.GetSelectedDifficulty()-1];
        _rotationSpeed = _battleStartRotationSpeed;

        CreateImpendingStormTargetZone();
        
        StartImpendingStorm();

        return _currentImpendingStormTargetZone.gameObject;
    }

    public void ActivateOvercharge()
    {
        RotateImpendingStorm(180);
    }

    private void CreateImpendingStormTargetZone()
    {
        _currentImpendingStormTargetZone = 
            Instantiate(_impendingStormTargetZone, transform.position, Quaternion.identity)
                .GetComponent<BossTargetZoneParent>();
        
        //_currentImpendingStormTargetZone.transform.SetParent(BossBase.Instance.GetSpecificBossScript().transform);
        
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
            RotateImpendingStorm(_rotationSpeed * Time.deltaTime);
            
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
    
    private void UpdateTargetZone()
    {
        _currentImpendingStormTargetZone.transform.eulerAngles = new Vector3(0, _attackRotation, 0);
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
            _attackTimer += Time.deltaTime;
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
    }

    private void BossDamaged(float damage)
    {
        UpdateRotationSpeed();
    }

    private void UpdateRotationSpeed()
    {
        float bossHealthRotationSpeedMultiplier = _bossHealthRotationMultiplierCurve.Evaluate(1 - BossStats.Instance.GetBossHealthPercentage());
        bossHealthRotationSpeedMultiplier = Mathf.Lerp(1, _maxBossHealthRotationMultiplier, bossHealthRotationSpeedMultiplier);
        _rotationSpeed = _baseRotationSpeed * bossHealthRotationSpeedMultiplier;

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
        _currentImpendingStormTargetZone.RemoveBossTargetZones();
    }

    private void SubscribeToEvents()
    {
        _myBossBase.GetBossDamagedEvent().AddListener(BossDamaged);
        
        _myBossBase.GetBossStaggeredEvent().AddListener(BossStaggered);
        _myBossBase.GetBossNoLongerStaggeredEvent().AddListener(BossNoLongerStaggered);
        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(BattleOver);
    }
    
    #region Getters

    public float GetAttackRotation() => _attackRotation;

    public float GetOppositeAttackRotation() => _attackRotation + 180;
    
    public Vector3 GetImpendingStormDirection() => _currentImpendingStormTargetZone.transform.forward;

    #endregion
}
