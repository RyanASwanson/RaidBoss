using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBUI_TerraLordUI : SpecificBossUIFramework
{
    [SerializeField] private List<Animator> _passiveBars;
    private const string _passiveBarShow = "ShowBar";
    private const string _passiveBarHide = "HideBar";

    int _startingPassiveBarValue;
    int _previousPassiveBarValue;

    SB_TerraLord _terraLordFunctionality;

    private void UpdatePassiveBars(float passivePercent)
    {
        int passiveBarValue = ConvertPercentToValue(passivePercent);

        //Debug.Log(passivePercent);
        IterateThroughPassiveBars(_previousPassiveBarValue, passiveBarValue);

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

        start = Mathf.Clamp(start, -1, _passiveBars.Count+1);
        end = Mathf.Clamp(end, -1, _passiveBars.Count+1);
            

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
            _passiveBars[barPos].SetTrigger(_passiveBarShow);
        }

        else
            _passiveBars[barPos].SetTrigger(_passiveBarHide);
    }

    //Takes the percentage and converts it to a scale from -1 to 1
    private int ConvertPercentToValue(float passivePercent)
    {
        float lerpA = _startingPassiveBarValue;
        float lerpB = _passiveBars.Count;

        return Mathf.RoundToInt(Mathf.LerpUnclamped(lerpA, lerpB, passivePercent));
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
