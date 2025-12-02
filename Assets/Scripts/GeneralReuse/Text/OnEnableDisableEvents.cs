using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnEnableDisableEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent _onEnable;
    [SerializeField] private UnityEvent _onDisable;


    private void OnEnable()
    {
        InvokeOnEnable();
    }

    private void OnDisable()
    {
        InvokeOnDisable();
    }

    #region Events

    private void InvokeOnEnable()
    {
        _onEnable.Invoke();
    }

    private void InvokeOnDisable()
    {
        _onDisable.Invoke();
    }
    #endregion
}
