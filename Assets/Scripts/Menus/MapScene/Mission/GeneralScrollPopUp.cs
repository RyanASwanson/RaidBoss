using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralScrollPopUp : MonoBehaviour
{
    [SerializeField] private float _defaultScrollLength;
    
    [Space]
    [SerializeField] private ScrollUISelection _popUpScrollUI;

    public void ShowScroll()
    {
        _popUpScrollUI.ShowNewScroll(_defaultScrollLength);
    }

    public void HideScroll()
    {
        _popUpScrollUI.HideScroll();
    }
}
