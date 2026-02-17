using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrollMissionTutorialContent : ScrollUIContents
{
    [Header("Title")]
    [SerializeField] private TextWithBackground _titleText;
    [SerializeField] private RectTransform _titleBackground;
    
    [Space]
    [SerializeField] private GameObject _missionPageHolder;
    
    [Space]
    [SerializeField] private GameObject _defaultSpecificTutorialPage;
    
    [Space]
    [SerializeField] private MissionTutorialVisuals _missionTutorialVisuals;
    
    private SpecificTutorialPage[] _tutorialPages;

    private MissionSO _currentMission;

    public void CreatePages()
    {
        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission))
        {
            _currentMission = mission;
            _tutorialPages = new SpecificTutorialPage[mission.GetTutorialPages().Length];

            for (int i = 0; i < _tutorialPages.Length; i++)
            {
                CreateMissionTutorialPage(i);
            }
        }
    }
    
    private void CreateMissionTutorialPage(int index)
    {
        SpecificTutorialPage newPage;
        newPage = Instantiate(_defaultSpecificTutorialPage, _missionPageHolder.transform).GetComponent<SpecificTutorialPage>();
        
        /*if (!_currentMission.GetTutorialPages()[index].TutorialPageObject.IsUnityNull())
        {
            newPage= Instantiate(_currentMission.GetTutorialPages()[index].TutorialPageObject, _missionPageHolder.transform)
                    .GetComponent<SpecificTutorialPage>();
        }*/
        
        newPage.SetUpPage(_currentMission.GetTutorialPages()[index]);
        
        _tutorialPages[index] = newPage;
        
        newPage.gameObject.SetActive(index == 0);
    }

    public override int UpdateContentsAndCountLines()
    {
        _tutorialPages[_missionTutorialVisuals.GetPreviousPageID()].gameObject.SetActive(false);
        _tutorialPages[_missionTutorialVisuals.GetCurrentPageID()].gameObject.SetActive(true);
        
        _titleText.UpdateText(_currentMission.GetTutorialPages()[_missionTutorialVisuals.GetCurrentPageID()].TutorialPageTitle);
        
        _titleBackground.sizeDelta = new Vector2(
            _currentMission.GetTutorialPages()[_missionTutorialVisuals.GetCurrentPageID()].TutorialPageTitleWidth, 
            _titleBackground.sizeDelta.y);
        
        _missionTutorialVisuals.NewPageStartDisplay();
        
        return 10;
    }


}
