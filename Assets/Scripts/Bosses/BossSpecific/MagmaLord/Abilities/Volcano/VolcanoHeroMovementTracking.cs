using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VolcanoHeroMovementTracking : MonoBehaviour
{
    [SerializeField] private float _volcanoTrackingIncreaseSpeed;
    [SerializeField] private float _volcanoTrackingDecreaseSpeed;
    [SerializeField] private float _volcanoMaxTracking;
    [SerializeField] private float _volcanoDecreaseToSpecificValue;
    [SerializeField] private float _volcanoDecreaseToValueSpeed;
    
    [Space]
    [SerializeField] private float _destroyTime;

    [Space] 
    [SerializeField] private AnimationCurve _volcanoWarningEffectIntensityCurve;
    
    [Space]
    [SerializeField] private FollowObject followObject;
    [SerializeField] private GeneralVFXFunctionality _generalVFXFunctionality;
    [SerializeField] private MaterialSetCustomProperty _materialSetCustomProperty;

    private float _currentVolcanoHeroMovementAmount;
    private float _currentVolcanoHeroTrackingProgress = 0;
    private float _volcanoWarningEffectIntensity;

    private bool _isForcedMovingDown = false;
    private bool _hasHeroMovedSinceLastTargetZoneSpawned = true;
    
    private Coroutine _volcanoHeroTrackingCoroutine;
    private Coroutine _volcanoForceDecreaseCoroutine;
    
    private SBA_Volcano _associatedVolcano;
    private HeroBase _associatedHero;
    private HeroPathfinding _associatedHeroMovement;
    
    public void SetUpVolcanoTracking(SBA_Volcano volcano, HeroBase hero)
    {
        _associatedVolcano = volcano;
        _associatedHero = hero;
        _associatedHeroMovement = hero.GetPathfinding();
        
        followObject.StartFollowingObject(hero.gameObject);
        _generalVFXFunctionality.SetUpParticleSystemEmissionRates();
        _materialSetCustomProperty.SetUp();

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    public void DestroyTrackingObject()
    {
        StopTrackingHeroMovement();
        Destroy(gameObject,_destroyTime);
    }

    public void StartTrackingHeroMovement()
    {
        StopTrackingHeroMovement();

        _volcanoHeroTrackingCoroutine = StartCoroutine(VolcanoHeroTrackingProcess());
    }

    public void StopTrackingHeroMovement()
    {
        if (!_volcanoHeroTrackingCoroutine.IsUnityNull())
        {
            StopCoroutine(_volcanoHeroTrackingCoroutine);
            StartMovingVolcanoProgressDownToValue(0, _volcanoDecreaseToValueSpeed);
        }
    }

    private IEnumerator VolcanoHeroTrackingProcess()
    {
        while (!_associatedHero.IsUnityNull())
        {
            if (_isForcedMovingDown)
            {
                yield return null;
            }
            
            if (_associatedHeroMovement.IsHeroMovingPathfindingOrAbility())
            {
                DecreaseVolcanoMovementAmount();
                _hasHeroMovedSinceLastTargetZoneSpawned = true;
            }
            else if (_hasHeroMovedSinceLastTargetZoneSpawned)
            {
                IncreaseVolcanoMovementAmount();

                if (_currentVolcanoHeroMovementAmount >= _volcanoMaxTracking)
                {
                    HitVolcanoMax();
                }
            }

            UpdateVolcanoMovementAmount();
            
            yield return null;
        }
    }

    private void IncreaseVolcanoMovementAmount()
    {
        _currentVolcanoHeroMovementAmount += _volcanoTrackingIncreaseSpeed * _associatedVolcano.GetSharedVolcanoTrackingMultiplier() * Time.deltaTime;
    }

    private void DecreaseVolcanoMovementAmount()
    {
        _currentVolcanoHeroMovementAmount -= _volcanoTrackingDecreaseSpeed * _associatedVolcano.GetSharedVolcanoTrackingMultiplier() * Time.deltaTime;
    }

    private void UpdateVolcanoMovementAmount()
    {
        _currentVolcanoHeroMovementAmount = Mathf.Clamp(_currentVolcanoHeroMovementAmount, 0, _volcanoMaxTracking);
        _currentVolcanoHeroTrackingProgress = _currentVolcanoHeroMovementAmount / _volcanoMaxTracking;

        _volcanoWarningEffectIntensity = _volcanoWarningEffectIntensityCurve.Evaluate(_currentVolcanoHeroTrackingProgress);
            
        _generalVFXFunctionality.SetEmissionRateMultiplier(_volcanoWarningEffectIntensity);
        _materialSetCustomProperty.UpdateMaterialFloatProperty(_volcanoWarningEffectIntensity);
    }

    private void StartMovingVolcanoProgressDownToValue(float targetValue, float moveTime)
    {
        StopMovingVolcanoProgressDownToValue();
        
        _isForcedMovingDown = true;

        _volcanoForceDecreaseCoroutine = StartCoroutine(MovingVolcanoProgressDownToValue(targetValue, moveTime));
    }

    private void StopMovingVolcanoProgressDownToValue()
    {
        if (!_volcanoForceDecreaseCoroutine.IsUnityNull())
        {
            StopCoroutine(_volcanoForceDecreaseCoroutine);
            _isForcedMovingDown = false;
        }
    }

    private IEnumerator MovingVolcanoProgressDownToValue(float targetValue, float moveSpeed)
    {
        while (_currentVolcanoHeroMovementAmount > targetValue)
        {
            _currentVolcanoHeroMovementAmount -= moveSpeed * Time.deltaTime;
            UpdateVolcanoMovementAmount();
            yield return null;
        }
        
        _currentVolcanoHeroMovementAmount = targetValue;
        UpdateVolcanoMovementAmount();
        _isForcedMovingDown = false;
    }

    private void HitVolcanoMax()
    {
        _associatedVolcano.VolcanoTargetHitMax(this);
        _hasHeroMovedSinceLastTargetZoneSpawned = false;
        StartMovingVolcanoProgressDownToValue(_volcanoDecreaseToSpecificValue,_volcanoDecreaseToValueSpeed);
    }

    public void VolcanoAbilityWasUsed()
    {
        _hasHeroMovedSinceLastTargetZoneSpawned = true;
    }

    private void SubscribeToEvents()
    {
        _associatedHero.GetHeroDiedEvent().AddListener(DestroyTrackingObject);
        
        GameStateManager.Instance.GetBattleWonOrLostEvent().AddListener(DestroyTrackingObject);
    }

    private void UnsubscribeFromEvents()
    {
        _associatedHero.GetHeroDiedEvent().RemoveListener(DestroyTrackingObject);
        
        GameStateManager.Instance.GetBattleWonOrLostEvent().RemoveListener(DestroyTrackingObject);
    }
}
