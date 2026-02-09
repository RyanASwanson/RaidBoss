using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTutorialVisuals : MonoBehaviour
{
    [SerializeField] private GeneralScrollPopUp _scrollPopUp;
    
    [Space]
    [SerializeField] private Transform _missionTutorialVisualsParent;

    private List<GameObject> _missionTutorialPages = new List<GameObject>();
    
    GameObject[] _tutorialPages;

    public void SetUpMissionTutorials()
    {
        SubscribeToEvents();

        CreateMissionTutorials();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    
    private void CreateMissionTutorials()
    {
        if (!SelectionManager.Instance.IsPlayingMissionsMode())
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (SelectionManager.Instance.GetSelectedMissionOut(out MissionSO mission))
        {
            _tutorialPages = mission.GetTutorialPages();
            
            for (int i = 0; i < _tutorialPages.Length; i++)
            {
                CreateMissionTutorialPage(i);
            }
        }
        
        _scrollPopUp.ShowScroll();
    }

    private void CreateMissionTutorialPage(int index)
    {
        GameObject page = Instantiate(_tutorialPages[index], _missionTutorialVisualsParent);
        
        _missionTutorialPages.Add(page);
        
        page.SetActive(index == 0);
    }

    public void CloseTutorial()
    {
        _scrollPopUp.HideScroll();
        GameStateManager.Instance.StartProgressToStart();
    }

    private void SubscribeToEvents()
    {
    }

    private void UnsubscribeFromEvents()
    {
    }
}
