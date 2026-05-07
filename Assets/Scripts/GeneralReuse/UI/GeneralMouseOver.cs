using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GeneralMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UnityEvent _onMouseEnter;
    [SerializeField] private UnityEvent _onMouseExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        InvokeOnMouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InvokeOnMouseExit();
    }

    private void OnDestroy()
    {
        _onMouseEnter.RemoveAllListeners();
        _onMouseExit.RemoveAllListeners();
    }
    
    #region Events

    public void InvokeOnMouseEnter()
    {
        _onMouseEnter?.Invoke();
    }

    public void InvokeOnMouseExit()
    {
        _onMouseExit?.Invoke();
    }
    #endregion

    #region Getters

    public UnityEvent GetOnMouseEnter() => _onMouseEnter;
    public UnityEvent GetOnMouseExit() => _onMouseExit;

    #endregion
}
