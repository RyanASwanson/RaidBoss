using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _closingBackground;
    [SerializeField] private float _closingBackgroundCloseSpeed;
    [Space]
    [SerializeField] private float _minimumCloseScaleFromBossHealthDecrease;
    [SerializeField] private float _closeScalePerDeadHero;
    [SerializeField] private AnimationCurve _closingBackgroundScaleCurve;

    private const float _startingCloseScale = 1;
    
    private float _targetClosingBackgroundScale;
    private float _currentClosingBackgroundScale;
    private Coroutine _closingBackgroundCoroutine;

    private void BossTookDamage(float damage)
    {
        CalculateTargetBackgroundScale();
        
        StartClosingBackgroundProcess();
    }

    private void HeroDied(HeroBase hero)
    {
        CalculateTargetBackgroundScale();

        StartClosingBackgroundProcess();
    }

    private void StartClosingBackgroundProcess()
    {
        if (!_closingBackgroundCoroutine.IsUnityNull())
        {
            StopCoroutine(_closingBackgroundCoroutine);
        }

        _closingBackgroundCoroutine = StartCoroutine(ClosingBackgroundCloseProcess());
    }

    private void CalculateTargetBackgroundScale()
    {
        _targetClosingBackgroundScale = Mathf.Lerp(_startingCloseScale, _minimumCloseScaleFromBossHealthDecrease,
            _closingBackgroundScaleCurve.Evaluate(1-BossStats.Instance.GetBossHealthPercentage()))
            - (_closeScalePerDeadHero * HeroesManager.Instance.GetAmountOfDeadHeroes());
        Debug.Log(_targetClosingBackgroundScale);
    }

    private IEnumerator ClosingBackgroundCloseProcess()
    {
        while (_closingBackground.transform.localScale.y > _targetClosingBackgroundScale)
        {
            SetClosingBackgroundCloseAmount(_closingBackground.transform.localScale.y - (_closingBackgroundCloseSpeed * Time.deltaTime));
            yield return null;
        }
        SetClosingBackgroundCloseAmount(_targetClosingBackgroundScale);
    }

    private void SetClosingBackgroundCloseAmount(float newScale)
    {
        // Vector set does not work for this
        _closingBackground.transform.localScale = new Vector3(_closingBackground.transform.localScale.x, newScale ,_closingBackground.transform.localScale.z);
    }
    
    #region Base UI Manager

    protected override void SubscribeToEvents()
    {
        BossBase.Instance.GetBossDamagedEvent().AddListener(BossTookDamage);
        HeroesManager.Instance.GetOnHeroDiedEvent().AddListener(HeroDied);
    }

    #endregion
    
}
