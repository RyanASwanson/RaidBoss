using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollUISelection : MonoBehaviour
{
    [SerializeField] private AnimationCurve _unscrollCurve;
    
    [SerializeField] private float _middleScrollScaleMultiplier;
    private Vector3 _middleScrollStartingScale;
    
    [Space]
    [SerializeField] private RectTransform _scrollTop;
    [SerializeField] private RectTransform _scrollUpper;
    [SerializeField] private RectTransform _scrollMiddle;
    [SerializeField] private RectTransform _scrollLower;
    [SerializeField] private RectTransform _scrollBottom;

    private Coroutine _scrollOpenCloseCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        _middleScrollStartingScale = _scrollMiddle.localScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            OpenScroll();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            CloseScroll();
        }

        SetMiddleTransform();
    }

    public void OpenScroll()
    {
        StopCurrentScrollProgress();

        _scrollOpenCloseCoroutine = StartCoroutine(ScrollOpenProcess());
    }
    
    public void CloseScroll()
    {
        StopCurrentScrollProgress();
        
        _scrollOpenCloseCoroutine = StartCoroutine(ScrollCloseProcess());
    }
    
    public void StopCurrentScrollProgress()
    {
        if (!_scrollOpenCloseCoroutine.IsUnityNull())
        {
            StopCoroutine(_scrollOpenCloseCoroutine);
        }
    }
    
    private IEnumerator ScrollOpenProcess()
    {
        yield return null;
    }
    
    private IEnumerator ScrollCloseProcess()
    {
        yield return null;
    }

    private void SetMiddleTransform()
    {
        float middleY = ( _scrollLower.localPosition.y + _scrollUpper.localPosition.y) / 2;
        
        _scrollMiddle.localPosition  = new Vector3(_scrollMiddle.localPosition.x, middleY ,_scrollMiddle.localPosition.z);

        float middleScale = (_scrollUpper.localPosition.y - _scrollLower.localPosition.y) * _middleScrollScaleMultiplier;
        //Debug.Log(_scrollUpper.localPosition.y + "    " + _scrollLower.localPosition.y + "    " + middleScale);
        _scrollMiddle.localScale = new Vector3(_middleScrollStartingScale.x, middleScale, _middleScrollStartingScale.z);
    }
    
}
