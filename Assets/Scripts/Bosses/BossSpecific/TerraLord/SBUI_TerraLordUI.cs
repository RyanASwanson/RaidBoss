using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBUI_TerraLordUI : SpecificBossUIFramework
{
    [SerializeField] private List<Animator> _passiveBars;
    private const string _passiveBarShow = "ShowBar";
    private const string _passiveBarHide = "ShowBar";

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
        if (end < start)
            changeVal *= -1;

        for (int i = start; i != end; i += changeVal)
        {
            Debug.Log(i);
        }
    }

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
