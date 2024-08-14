using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBUI_TerraLordUI : SpecificBossUIFramework
{
    [SerializeField] private List<Animator> _passiveBars;
    private const string _passiveBarShowAnimTrigger = "ShowBar";
    private const string _passiveBarHideAnimTrigger = "HideBar";

    int _startingPassiveBarValue;
    int _previousPassiveBarValue;

    [Space]
    [SerializeField] private float _minorWarningPercent;
    [SerializeField] private float _majorWarningPercent;

    [Space]
    [SerializeField] private Animator _leftWarningIcon;
    [SerializeField] private Animator _rightWarningIcon;
    private const string _warningIconAnimInt = "WarningLevel";

    SB_TerraLord _terraLordFunctionality;

    private void UpdatePassiveBars(float passivePercent)
    {
        int passiveBarValue = ConvertPercentToValue(passivePercent);

        //Debug.Log(passivePercent);
        IterateThroughPassiveBars(_previousPassiveBarValue, passiveBarValue);

        EvaluateWarningLevels(passivePercent);

        _previousPassiveBarValue = passiveBarValue;
    }

    private void IterateThroughPassiveBars(int start, int end)
    {
        int changeVal = 1;
        bool upOrDown = true;

        if (end < start)
        {
            changeVal *= -1;
            upOrDown = false;
        }
        else if (end == start)
        {
            changeVal = 0;
        }

        start = Mathf.Clamp(start, -1, _passiveBars.Count);
        end = Mathf.Clamp(end, -1, _passiveBars.Count);

        for (int i = start; i != end; i += changeVal)
        {
            AnimateSpecificBar(i, upOrDown);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="barPos"></param>
    /// <param name="changeDirection"></param> false = down, true = up
    private void AnimateSpecificBar(int barPos, bool changeDirection)
    {
        bool sideCondition = barPos > _startingPassiveBarValue;

        if (sideCondition == changeDirection)
        {
            _passiveBars[barPos].ResetTrigger(_passiveBarHideAnimTrigger);
            _passiveBars[barPos].SetTrigger(_passiveBarShowAnimTrigger);
        }
        else
        {
            _passiveBars[barPos].ResetTrigger(_passiveBarShowAnimTrigger);
            _passiveBars[barPos].SetTrigger(_passiveBarHideAnimTrigger);
        }
            
    }

    //Takes the percentage and converts it to a scale from -1 to 1
    private int ConvertPercentToValue(float passivePercent)
    {
        float lerpA = _startingPassiveBarValue;
        float lerpB = _passiveBars.Count;

        return Mathf.RoundToInt(Mathf.LerpUnclamped(lerpA, lerpB, passivePercent));
    }

    private void EvaluateWarningLevels(float passivePercent)
    {
        if (passivePercent >= _majorWarningPercent)
            _rightWarningIcon.SetInteger(_warningIconAnimInt, 2);
        else if (passivePercent >= _minorWarningPercent)
            _rightWarningIcon.SetInteger(_warningIconAnimInt, 1);
        else if (passivePercent <= -_majorWarningPercent)
            _leftWarningIcon.SetInteger(_warningIconAnimInt, 2);
        else if (passivePercent <= -_minorWarningPercent)
            _leftWarningIcon.SetInteger(_warningIconAnimInt, 1);
        else
        {
            _rightWarningIcon.SetInteger(_warningIconAnimInt, 0);
            _leftWarningIcon.SetInteger(_warningIconAnimInt, 0);
        }
            
    }

    protected override void AdditionalSetup()
    {
        _terraLordFunctionality = (SB_TerraLord)_associatedBossScript;

        _startingPassiveBarValue = (_passiveBars.Count - 1)/2;
        _previousPassiveBarValue = _startingPassiveBarValue;
    }

    protected override void SubscribeToEvents()
    {
        _terraLordFunctionality.GetPassivePercentUpdatedEvent().AddListener(UpdatePassiveBars);
    }
}
