using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GeneralInputPress : MonoBehaviour
{
    [SerializeField] protected UnityEvent _onPressEvent;
    [SerializeField] protected UnityEvent _onReleaseEvent;
    
    protected UniversalPlayerInputActions _universalPlayerInputActions;

    protected virtual void OnEnable()
    {
        _universalPlayerInputActions = new UniversalPlayerInputActions();
        _universalPlayerInputActions.Enable();
    }

    protected virtual void OnDisable()
    {
        _universalPlayerInputActions.Enable();
    }

    protected virtual void ButtonPressStart(InputAction.CallbackContext context)
    {
        _onPressEvent?.Invoke();
    }

    protected virtual void ButtonPressEnd(InputAction.CallbackContext context)
    {
        _onReleaseEvent?.Invoke();
    }
    
}
