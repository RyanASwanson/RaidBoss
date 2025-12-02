using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GeneralButtonHeld : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private UnityEvent _onHold;
    [SerializeField] private UnityEvent _onRelease;

    public void OnPointerDown(PointerEventData eventData)
    {
        InvokeOnHold();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InvokeOnRelease();
    }
    
    #region Events

    private void InvokeOnHold()
    {
        _onHold.Invoke();
    }

    private void InvokeOnRelease()
    {
        _onRelease.Invoke();
    }
    #endregion
}
