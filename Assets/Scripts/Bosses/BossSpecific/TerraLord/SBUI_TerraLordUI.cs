using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the boss UI for the Terra Lord
/// </summary>
public class SBUI_TerraLordUI : SpecificBossUIFramework
{
    [SerializeField] private List<Animator> _passiveBars;

    private const string PASSIVE_BAR_SHOW_ANIM_TRIGGER = "ShowBar";
    private const string PASSIVE_BAR_HIDE_ANIM_TRIGGER = "HideBar";

    int _startingPassiveBarValue;
    int _previousPassiveBarValue;

    [Space]
    [SerializeField] private float _minorWarningPercent;
    [SerializeField] private float _majorWarningPercent;

    [Space]
    [SerializeField] private Animator _leftWarningIcon;
    [SerializeField] private Animator _rightWarningIcon;

    private const string WARNING_ICON_ANIM_INT = "WarningLevel";

    SB_TerraLord _terraLordFunctionality;

    /// <summary>
    /// Adjusts the ui whenever the Terra Lords passive has updated its value
    /// </summary>
    /// <param name="passivePercent"></param>
    private void UpdatePassiveBars(float passivePercent)
    {
        int passiveBarValue = ConvertPercentToValue(passivePercent);

        IterateThroughPassiveBars(_previousPassiveBarValue, passiveBarValue);

        //Determines if the value is far enough on either side to show a warning
        EvaluateWarningLevels(passivePercent);

        _previousPassiveBarValue = passiveBarValue;
    }

    /// <summary>
    /// Moves from the previous passive bar value to the new value
    /// First it determines the direction that it is moving in, as well as 
    ///     if we should be hiding or showing the nodes we are moving through
    /// Iterates through the bars and animates them accordingly
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
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
    /// Animates a single bar on the passive ui
    /// </summary>
    /// <param name="barPos"></param>
    /// <param name="changeDirection"></param> false = down, true = up
    private void AnimateSpecificBar(int barPos, bool changeDirection)
    {
        bool sideCondition = barPos > _startingPassiveBarValue;

        if (sideCondition == changeDirection)
        {
            _passiveBars[barPos].ResetTrigger(PASSIVE_BAR_HIDE_ANIM_TRIGGER);
            _passiveBars[barPos].SetTrigger(PASSIVE_BAR_SHOW_ANIM_TRIGGER);
        }
        else
        {
            _passiveBars[barPos].ResetTrigger(PASSIVE_BAR_SHOW_ANIM_TRIGGER);
            _passiveBars[barPos].SetTrigger(PASSIVE_BAR_HIDE_ANIM_TRIGGER);
        }
            
    }

    /// <summary>
    /// Takes the percentage and converts it to a scale from -1 to 1
    /// </summary>
    /// <param name="passivePercent"></param>
    /// <returns></returns>
    private int ConvertPercentToValue(float passivePercent)
    {
        float lerpA = _startingPassiveBarValue;
        float lerpB = _passiveBars.Count;

        return Mathf.RoundToInt(Mathf.LerpUnclamped(lerpA, lerpB, passivePercent));
    }

    /// <summary>
    /// Determines if the passive percent is far enough to either side
    ///     to show a warning UI element. The warning is animated differently
    ///     depending on the level of intensity
    /// </summary>
    /// <param name="passivePercent"></param>
    private void EvaluateWarningLevels(float passivePercent)
    {
        if (passivePercent >= _majorWarningPercent)
            _rightWarningIcon.SetInteger(WARNING_ICON_ANIM_INT, 2);
        else if (passivePercent >= _minorWarningPercent)
            _rightWarningIcon.SetInteger(WARNING_ICON_ANIM_INT, 1);
        else if (passivePercent <= -_majorWarningPercent)
            _leftWarningIcon.SetInteger(WARNING_ICON_ANIM_INT, 2);
        else if (passivePercent <= -_minorWarningPercent)
            _leftWarningIcon.SetInteger(WARNING_ICON_ANIM_INT, 1);
        else
        {
            _rightWarningIcon.SetInteger(WARNING_ICON_ANIM_INT, 0);
            _leftWarningIcon.SetInteger(WARNING_ICON_ANIM_INT, 0);
        }
            
    }



    #region Base Boss UI
    public override void SetupBossSpecificUIFunctionality(BossBase bossBase, SpecificBossFramework specificBoss)
    {
        _terraLordFunctionality = (SB_TerraLord)specificBoss;

        //Sets the starting value at the middle
        _startingPassiveBarValue = (_passiveBars.Count - 1) / 2;
        _previousPassiveBarValue = _startingPassiveBarValue;

        base.SetupBossSpecificUIFunctionality(bossBase, specificBoss);
    }


    protected override void SubscribeToEvents()
    {
        _terraLordFunctionality.GetPassivePercentUpdatedEvent().AddListener(UpdatePassiveBars);
    }
    #endregion
}
