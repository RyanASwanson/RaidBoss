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
    [SerializeField] private bool _isVFXChilded;
    [SerializeField] private Transform _vfxTargetLocation;
    
    [Space] 
    [SerializeField] private UnityEvent _onDestroySelfEvent;
    [SerializeField] private UnityEvent _onDestroyParentEvent;

    public void CreateVFXAtCurrentLocation(int vfxID)
    {
        CreateVFX(vfxID, transform);
    }

    private void CreateVFXAtChildLocation(int vfxID)
    {
        CreateVFX(vfxID, transform.GetChild(0).transform);
    }
    
    private void CreateVFXAtTargetLocation(int vfxID)
    {
        CreateVFX(vfxID, _vfxTargetLocation.transform);

    }

    private void CreateVFX(int vfxID, Transform location)
    {
        GameObject createdVfx;
        if (_isVFXChilded)
        {
            createdVfx =Instantiate(_vfxList[vfxID], location);
        }
        else
        {
            createdVfx =Instantiate(_vfxList[vfxID], location.position, location.rotation);
            createdVfx.transform.localScale = Vector3.one;
        }

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
