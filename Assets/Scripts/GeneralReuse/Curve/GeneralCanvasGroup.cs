using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCanvasGroup : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    
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
        _canvasGroup.alpha = progress;
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
