using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtrasUIFunctionality : MonoBehaviour
{
    [SerializeField] private CurveProgression _squishCurve;
    
    private int _currentPageIndex = 0;

    public void ChangeToPage(int newPageIndex)
    {
        if (_currentPageIndex != newPageIndex)
        {
            _currentPageIndex = newPageIndex;
            _squishCurve.StartMovingUpOnCurve();
        }
        else
        {
            
        }
    }
}
