using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTutorialIndividualStepUISection : MonoBehaviour
{
    [SerializeField] private CurveProgression _canvasGroupCurve;
    
    [SerializeField] private CurveProgression _scaleCurve;

    private CurveProgression _customUICurve;

    [Space]
    [SerializeField] private Transform _lowerHolder;

    private GameObject _childIndividualTutorialUI;
    private RectTransform _childIndividualTutorialUIRectTransform;
    
    private CustomIndividualTutorialUI _customIndividualTutorialUI;

    public void SetUpCustomTutorialIndividualStepUISection(CustomIndividualTutorialUI customIndividualTutorialUI)
    {
        _customIndividualTutorialUI = customIndividualTutorialUI;

        if (_customIndividualTutorialUI.DoesUseCanvasGroupCurve)
        {
            _canvasGroupCurve.ForceSetCurveProgress(0);
        }

        if (_customIndividualTutorialUI.DoesUseScaleCurve)
        {
            _scaleCurve.ForceSetCurveProgress(0);
        }
    }

    public void SetUpChildIndividualTutorialUI(GameObject childIndividualTutorialUI)
    {
        _childIndividualTutorialUI = childIndividualTutorialUI;

        if (!_customIndividualTutorialUI.DoesUseCustomUIObjectCanvas)
        {
            _childIndividualTutorialUI.transform.parent = _lowerHolder;

            _childIndividualTutorialUIRectTransform = _childIndividualTutorialUI.GetComponent<RectTransform>();
            _childIndividualTutorialUIRectTransform.localPosition = Vector3.zero;
            _childIndividualTutorialUIRectTransform.localScale = Vector3.one;
        }
        else
        {
            _childIndividualTutorialUI.transform.SetParent(null);
            _customUICurve = _childIndividualTutorialUI.GetComponent<CurveProgression>();
            _customUICurve.ForceSetCurveProgress(0);
        }
    }

    public void ShowIndividualStepUI()
    {
        if (_customIndividualTutorialUI.DoesUseCanvasGroupCurve)
        {
            _canvasGroupCurve.StartMovingUpOnCurve();
        }
        
        if (_customIndividualTutorialUI.DoesUseScaleCurve)
        {
            _scaleCurve.StartMovingUpOnCurve();
        }

        if (_customIndividualTutorialUI.DoesUseCustomUIObjectCanvas)
        {
            _customUICurve.StartMovingUpOnCurve();
        }
    }

    public void HideIndividualStepUI()
    {
        if (_customIndividualTutorialUI.DoesUseCanvasGroupCurve)
        {
            _canvasGroupCurve.StartMovingDownOnCurve();
        }
        
        if (_customIndividualTutorialUI.DoesUseScaleCurve)
        {
            _scaleCurve.StartMovingDownOnCurve();
        }
        
        if (_customIndividualTutorialUI.DoesUseCustomUIObjectCanvas)
        {
            _customUICurve.StartMovingDownOnCurve();
        }
    }
}
