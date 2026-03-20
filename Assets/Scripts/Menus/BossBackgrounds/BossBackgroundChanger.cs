using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBackgroundChanger : MonoBehaviour
{
    [SerializeField] private int _startingBackground;
    
    [SerializeField] private CurveProgression[] _backgroundCurveProgressions;
    private CurveProgression _currentBackgroundCurveProgression;
    
    [SerializeField] private GeneralVFXFunctionality[] _backgroundParticles;
    private GeneralVFXFunctionality _currentBackgroundParticles;

    private bool _hasPerformedSetUp = false;

    private void OnEnable()
    {
        if (!_hasPerformedSetUp)
        {
            PerformSetUp();
        }
    }

    private void PerformSetUp()
    {
        if (_startingBackground >= 0)
        {
            ShowStartingBackground(_startingBackground);
        }
        else
        {
            HideAllBackgroundParticles();
        }
        
        _hasPerformedSetUp = true;
    }

    public void ShowStartingBackground(int background)
    {
        for (int i = 0; i < _backgroundCurveProgressions.Length; i++)
        {
            if (_backgroundCurveProgressions[i].IsUnityNull())
            {
                continue;
            }
            
            if (i == background)
            {
                _backgroundCurveProgressions[i].ForceSetCurveProgress(1);
                _backgroundCurveProgressions[i].SetHasStartingValue(false);
            }
        }

        SetUpBackgroundParticles();
    }

    public void SetUpBackgroundParticles()
    {
        HideAllBackgroundParticles();
        ShowStartingBackgroundParticles();
    }
    
    public void UpdateBackground(LevelSO level)
    {
        if (level.IsUnityNull())
        {
            return;
        }

        HideCurrentBackground();
        ShowBackground(level);
        ShowBackgroundParticles(level);
    }

    public void HideCurrentBackground()
    {
        RemoveCurrentBackground();
        RemoveCurrentBackgroundParticles();
    }

    private void ShowBackground(LevelSO level)
    {
        _currentBackgroundCurveProgression = _backgroundCurveProgressions[level.GetLevelNumber()];
        
        if (_currentBackgroundCurveProgression.IsUnityNull())
        {
            return;
        }
        
        _currentBackgroundCurveProgression.StartMovingUpOnCurve();
    }

    private void RemoveCurrentBackground()
    {
        if (_currentBackgroundCurveProgression.IsUnityNull())
        {
            return;
        }
        _currentBackgroundCurveProgression.StartMovingDownOnCurve();

    }

    private void ShowBackgroundParticles(LevelSO level)
    {
        _currentBackgroundParticles = _backgroundParticles[level.GetLevelNumber()];
        _currentBackgroundParticles.gameObject.SetActive(true);
    }

    private void RemoveCurrentBackgroundParticles()
    {
        if (_currentBackgroundParticles.IsUnityNull())
        {
            return;
        }
        _currentBackgroundParticles.gameObject.SetActive(false);
    }
    
    public void HideAllBackgroundParticles()
    {
        foreach (GeneralVFXFunctionality particle in _backgroundParticles)
        {
            particle.gameObject.SetActive(false);
        }
    }

    public void ShowStartingBackgroundParticles()
    {
        ShowBackgroundParticles(SaveManager.Instance.GetMissionsInGame()[0].GetAssociatedLevel());
    }
}
