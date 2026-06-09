using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetZoneScaleToEdgeOfMap : MonoBehaviour
{
    [SerializeField] private bool _doesScaleOnStart;
    [SerializeField] private bool _doesConstantlyScale;

    [SerializeField] private bool _doesHaveScaleCap;
    [SerializeField] private float _scaleCap;
    
    [Space]
    [SerializeField] private GameObject[] _storedTargetZone;
    
    public void Start()
    {
        if (_doesScaleOnStart)
        {
            ScaleToEdge();
        }
        if (_doesConstantlyScale)
        {
            StartCoroutine(ConstantScaleToEdge());
        }
    }

    private IEnumerator ConstantScaleToEdge()
    {
        while (true)
        {
            ScaleToEdge();
            yield return null;
        }
    }
    
    public void ScaleToEdge()
    {
        foreach (GameObject targetZone in _storedTargetZone)
        {
            ScaleSpecificTargetZone(targetZone);
        }
    }

    private void ScaleSpecificTargetZone(GameObject targetZone)
    {
        Vector3 edgeOfMap =  EnvironmentManager.Instance.GetEdgeOfMapLoc(transform.position,
            (transform.forward).normalized);

        float scaleDistance = Vector3.Distance(transform.position, edgeOfMap) / 2;

        if (_doesHaveScaleCap)
        {
            scaleDistance = Mathf.Clamp(scaleDistance,0, _scaleCap);
        }
        
        targetZone.transform.localScale = new(targetZone.transform.localScale.x,
            targetZone.transform.localScale.y, scaleDistance);
    }
    
    #region Setters
    public void SetScaleCap(float scaleCap) => _scaleCap = scaleCap;
    public void MultiplyScaleCap(float multiplier) => _scaleCap *= multiplier;
    #endregion
    
}
