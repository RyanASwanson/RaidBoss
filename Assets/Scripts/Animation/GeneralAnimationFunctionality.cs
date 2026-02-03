using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides general functionality for animations to play
/// </summary>
public class GeneralAnimationFunctionality : MonoBehaviour
{
    [SerializeField] private List<GameObject> _vfxList;

    [Space] 
    [SerializeField] private UnityEvent _onDestroySelfEvent;
    [SerializeField] private UnityEvent _onDestroyParentEvent;

    public void CreateVFXAtCurrentLocation(int vfxID)
    {
        Instantiate(_vfxList[vfxID], transform.position, transform.rotation); 
    }

    private void CreateVFXAtChildLocation(int vfxID)
    {
        Instantiate(_vfxList[vfxID], transform.GetChild(0).position, transform.GetChild(0).rotation);
    }

    public void DestroySelf()
    {
        _onDestroySelfEvent.Invoke();
        Destroy(gameObject);
    }

    public void DestroyParent()
    {
        _onDestroyParentEvent.Invoke();
        Destroy(transform.parent.gameObject);
    }

    private void OnDestroy()
    {
        _onDestroySelfEvent.RemoveAllListeners();   
        _onDestroyParentEvent.RemoveAllListeners();
    }
}
