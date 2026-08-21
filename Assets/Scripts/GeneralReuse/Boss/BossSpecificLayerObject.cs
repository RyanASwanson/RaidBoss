using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossSpecificLayerObject : MonoBehaviour
{
    [SerializeField] private UnityEvent _onHit;

    public void InvokeOnHit()
    {
        _onHit?.Invoke();
    }
    
    public UnityEvent GetOnHit() => _onHit;
}
