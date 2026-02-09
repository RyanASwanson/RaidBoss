using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralImageFill : MonoBehaviour
{
    [SerializeField] private Image _image;
    
    [SerializeField] private CurveProgression _curveProgression;
    
    
    void OnEnable()
    {
        SubscribeToEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    

    public void UpdateProgress(float progress)
    {
        _image.fillAmount = progress;
    }
    
    private void SubscribeToEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.AddListener(UpdateProgress);
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_curveProgression)
        {
            _curveProgression.OnCurveValueChanged.RemoveListener(UpdateProgress);
        }
    }
}
