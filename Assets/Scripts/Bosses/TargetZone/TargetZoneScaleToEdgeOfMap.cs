using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetZoneScaleToEdgeOfMap : MonoBehaviour
{
    [SerializeField] private bool _doesScaleOnStart;
    [SerializeField] private bool _doesConstantlyScale;
    
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
        /*Vector3 edgeOfMap =  EnvironmentManager.Instance.GetEdgeOfMapLoc(transform.position,
            (transform.forward).normalized);
        
        _storedTargetZone[0].transform.localScale = new(_storedTargetZone[0].transform.localScale.x,
            _storedTargetZone[0].transform.localScale.y, Vector3.Distance(transform.position, edgeOfMap) / 2);*/

        foreach (GameObject targetZone in _storedTargetZone)
        {
            ScaleSpecificTargetZone(targetZone);
        }
    }

    private void ScaleSpecificTargetZone(GameObject targetZone)
    {
        Vector3 edgeOfMap =  EnvironmentManager.Instance.GetEdgeOfMapLoc(transform.position,
            (transform.forward).normalized);
        
        targetZone.transform.localScale = new(targetZone.transform.localScale.x,
            targetZone.transform.localScale.y, Vector3.Distance(transform.position, edgeOfMap) / 2);
    }
    
}
