using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCanvasDecal : MonoBehaviour
{
    [SerializeField] private bool _isVisibleOnEnable;
    
    [Space]
    [SerializeField] private Image _associatedImage;

    [Space] 
    [SerializeField] private bool _hasImageTransparencyCurve;
    [SerializeField] private CurveProgression _imageTransparencyCurve;

    private void OnEnable()
    {
        _associatedImage.enabled = _isVisibleOnEnable;
    }

    public void ShowCustomCanvasDecal(Sprite showSprite)
    {
        _associatedImage.sprite = showSprite;

        ActivateCustomDecal();
    }

    public void ActivateCustomDecal()
    {
        if (_hasImageTransparencyCurve)
        {
            _associatedImage.enabled = true;
            _imageTransparencyCurve.StartMovingUpOnCurve();
        }
    }
}
