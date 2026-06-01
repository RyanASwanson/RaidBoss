using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCanvasOnPause : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _previousAlpha;

    private void OnEnable()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void HideCanvasGroup()
    {
        _previousAlpha = _canvasGroup.alpha;
        _canvasGroup.alpha = 0;
    }

    private void ShowCanvasGroup()
    {
        _canvasGroup.alpha = _previousAlpha;
    }

    private void SubscribeToEvents()
    {
        TimeManager.Instance.GetGamePausedEvent().AddListener(HideCanvasGroup);
        TimeManager.Instance.GetGameUnpausedEvent().AddListener(ShowCanvasGroup);
    }

    private void UnsubscribeFromEvents()
    {
        TimeManager.Instance.GetGamePausedEvent().RemoveListener(HideCanvasGroup);
        TimeManager.Instance.GetGameUnpausedEvent().AddListener(ShowCanvasGroup);
    }
}
