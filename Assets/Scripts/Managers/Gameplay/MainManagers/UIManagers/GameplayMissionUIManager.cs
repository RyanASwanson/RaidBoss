using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayMissionUIManager : GameUIChildrenFunctionality
{
    [SerializeField] private GameObject _missionTutorialUIObjectParent;
    
    [Space]
    [SerializeField] private GameObject _missionTutorialVisuals;
    
    public override void ChildFuncSetUp()
    {
        base.ChildFuncSetUp();
        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission) &&
            mission.GetTutorialPages().Length > 0)
        {
            MissionTutorialVisuals missionTutorialVisuals = 
                Instantiate(_missionTutorialVisuals, _missionTutorialUIObjectParent.transform).GetComponent<MissionTutorialVisuals>();
            
            missionTutorialVisuals.SetUpMissionTutorials();
        }


    }
    
    
}
