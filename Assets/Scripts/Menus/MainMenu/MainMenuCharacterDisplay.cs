using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCharacterDisplay : MonoBehaviour
{
    [SerializeField] private float _displayTime;
    [SerializeField] private float _fadeTime;
    private WaitForSeconds _displayWait;
    private WaitForSeconds _fadeWait;
    
    [Space]
    [SerializeField] private CurveProgression _foregroundTransparencyCurve;
    
    [Space]
    [SerializeField] private GameObject[] _displaySections;
    private int _currentDisplay;
    private int _lastDisplay;


    private void Start()
    {
        _displayWait = new WaitForSeconds(_displayTime);
        _fadeWait = new WaitForSeconds(_fadeTime);

        for (int i = 0; i < _displaySections.Length; i++)
        {
            _displaySections[i].SetActive(i == 0);
        }

        StartChangeCurrentDisplayFadeOut();
    }

    private void StartDisplayRotation()
    {
        StartCoroutine(CharacterDisplayRotation());
    }

    private IEnumerator CharacterDisplayRotation()
    {
        yield return _displayWait;
        MoveToNextDisplaySection();
    }

    public void MoveToNextDisplaySection()
    {
        _lastDisplay = _currentDisplay;
        _currentDisplay++;
        if (_currentDisplay >= _displaySections.Length)
        {
            _currentDisplay = 0;
        }
        StartChangeCurrentDisplayFadeIn();
    }
    
    private void StartChangeCurrentDisplayFadeOut()
    {
        StartCoroutine(CharacterDisplayFadeOut());
    }
    
    private IEnumerator CharacterDisplayFadeOut()
    {
        _foregroundTransparencyCurve.StartMovingDownOnCurve();
        yield return _fadeWait;
        DisplayFadedOut();
    }

    private void DisplayFadedOut()
    {
        StartDisplayRotation();
    }
    

    private void StartChangeCurrentDisplayFadeIn()
    {
        StartCoroutine(CharacterDisplayFadeIn());
    }

    private IEnumerator CharacterDisplayFadeIn()
    {
        _foregroundTransparencyCurve.StartMovingUpOnCurve();
        yield return _fadeWait;
        ChangeDisplay();
        StartChangeCurrentDisplayFadeOut();
    }
    
    private void ChangeDisplay()
    {
        _displaySections[_lastDisplay].SetActive(false);
        _displaySections[_currentDisplay].SetActive(true);
    }
}
