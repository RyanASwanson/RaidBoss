using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollMissionTutorialContent : ScrollUIContents
{
    [Header("Title")]
    [SerializeField] private TextWithBackground _titleText;
    [SerializeField] private RectTransform _titleBackground;
    
    [Space]
    [SerializeField] private GameObject _missionPageHolder;
    
    [Space]
    [SerializeField] private MissionTutorialVisuals _missionTutorialVisuals;
    
    private GameObject[] _tutorialPages;

    private MissionSO _currentMission;

    public void CreatePages()
    {
        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission))
        {
            _currentMission = mission;
            _tutorialPages = new GameObject[mission.GetTutorialPages().Length];

            for (int i = 0; i < _tutorialPages.Length; i++)
            {
                CreateMissionTutorialPage(i);
            }
        }
    }
    
    private void CreateMissionTutorialPage(int index)
    {
        GameObject newPage = Instantiate(_currentMission.GetTutorialPages()[index].TutorialPageObject, _missionPageHolder.transform);
        _tutorialPages[index] = newPage;
        
        newPage.SetActive(index == 0);
    }

    public override int UpdateContentsAndCountLines()
    {
        _tutorialPages[_missionTutorialVisuals.GetPreviousPageID()].SetActive(false);
        _tutorialPages[_missionTutorialVisuals.GetCurrentPageID()].SetActive(true);
        
        _titleText.UpdateText(_currentMission.GetTutorialPages()[_missionTutorialVisuals.GetCurrentPageID()].TutorialPageTitle);
        
        _titleBackground.sizeDelta = new Vector2(
            _currentMission.GetTutorialPages()[_missionTutorialVisuals.GetCurrentPageID()].TutorialPageTitleWidth, 
            _titleBackground.sizeDelta.y);
        
        _missionTutorialVisuals.NewPageStartDisplay();
        
        return 10;
    }


}
