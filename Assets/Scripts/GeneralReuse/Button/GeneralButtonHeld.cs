using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GeneralButtonHeld : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool _requiresEnabledButton;
    [SerializeField] private Button _associatedButton;
    
    [Space]
    [SerializeField] private UnityEvent _onHold;
    [SerializeField] private UnityEvent _onRelease;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!CanBePressed())
        {
            return;
        }
        
        InvokeOnHold();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!CanBePressed())
        {
            return;
        }
        
        InvokeOnRelease();
    }

    private bool CanBePressed()
    {
        if (_requiresEnabledButton && (_associatedButton.IsUnityNull() || !_associatedButton.interactable))
        {
            return false;
        }

        return true;
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
