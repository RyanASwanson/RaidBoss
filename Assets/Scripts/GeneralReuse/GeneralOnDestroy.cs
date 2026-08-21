using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralOnDestroy : MonoBehaviour
{
    private UnityEvent<GameObject> _onDestroy = new();

    private void OnDestroy()
    {
        _onDestroy?.Invoke(gameObject);
        _onDestroy?.RemoveAllListeners();
    }
    
    public UnityEvent<GameObject> GetOnDestroy() => _onDestroy;
}
