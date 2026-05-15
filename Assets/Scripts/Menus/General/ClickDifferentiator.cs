using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickDifferentiator : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent _onLeftClickEvent;
    [SerializeField] private UnityEvent _onMiddleClickEvent;
    [SerializeField] private UnityEvent _onRightClickEvent;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                _onLeftClickEvent?.Invoke();
                return;
            case PointerEventData.InputButton.Middle:
                _onMiddleClickEvent?.Invoke();
                return;
            case PointerEventData.InputButton.Right:
                _onRightClickEvent?.Invoke();
                return;
        }
    }
}
