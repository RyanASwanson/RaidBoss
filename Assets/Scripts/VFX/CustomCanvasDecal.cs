using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCanvasDecal : MonoBehaviour
{
    [SerializeField] private bool _activateOnEnable;
    
    [Space]
    [SerializeField] private Image _associatedImage;

    [Space] 
    [SerializeField] private bool _hasImageTransparencyCurve;
    [SerializeField] private CurveProgression _imageTransparencyCurve;

    private void OnEnable()
    {
        if (_activateOnEnable)
        {
            ActivateCustomDecal();
        }
    }

    public void ShowCustomCanvasDecal(Sprite showSprite)
    {
        _associatedImage.sprite = showSprite;

        ActivateCustomDecal();
    }

    private void ActivateCustomDecal()
    {
        if (_hasImageTransparencyCurve)
        {
            _imageTransparencyCurve.StartMovingUpOnCurve();
        }
    }
}
