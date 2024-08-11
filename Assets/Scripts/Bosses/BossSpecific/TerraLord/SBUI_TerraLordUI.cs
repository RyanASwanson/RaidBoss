using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBUI_TerraLordUI : SpecificBossUIFramework
{
    [SerializeField] private List<Animator> _passivePassiveBars;

    SB_TerraLord _terraLordFunctionality;

    private void UpdatePassiveBars(float passiveVal)
    {
        Debug.Log(passiveVal);
    }

    protected override void AdditionalSetup()
    {
        _terraLordFunctionality = (SB_TerraLord)_associatedBossScript;
    }

    protected override void SubscribeToEvents()
    {
        _terraLordFunctionality.GetPassivePercentUpdatedEvent().AddListener(UpdatePassiveBars);
    }
}
