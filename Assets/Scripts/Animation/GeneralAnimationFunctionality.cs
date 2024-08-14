using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAnimationFunctionality : MonoBehaviour
{
    [SerializeField] private List<GameObject> _vfxList;

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
        Destroy(gameObject);
    }

    public void DestroyParent()
    {
        Destroy(transform.parent.gameObject);
    }
}
