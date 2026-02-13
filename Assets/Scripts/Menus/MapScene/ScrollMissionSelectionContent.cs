using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMissionSelectionContent : ScrollUIContents
{
    [Space]
    [SerializeField] private TextWithBackground _titleText;

    [Space] 
    [SerializeField] private Image _bossIcon;
    [SerializeField] private Image _difficultyIcon;

    [Space] 
    [SerializeField] private SelectHeroButton[] _heroIcons;
    
    [Space]
    [SerializeField] private SelectionPlayButton _playButton;
    
    public override int UpdateContentsAndCountLines()
    {
        MissionSO missionSO = MapController.Instance.GetSelectedMission().GetAssociatedMission();
        
        _titleText.UpdateText(missionSO.GetMissionName());

        _bossIcon.sprite = missionSO.GetAssociatedLevel().GetLevelBoss().GetBossIcon();

        _difficultyIcon.sprite = SelectionManager.Instance.GetDifficultyIconOfCurrentDifficulty();

        List<HeroSO> selectedHeroes = SelectionManager.Instance.GetAllSelectedHeroes();

        int heroIteration;
        for (heroIteration = 0; heroIteration < selectedHeroes.Count; heroIteration++)
        {
            _heroIcons[heroIteration].SetAssociatedHero(selectedHeroes[heroIteration]);
            _heroIcons[heroIteration].SetButtonInteractability(true);
        }

        for (; heroIteration < _heroIcons.Length; heroIteration++)
        {
            _heroIcons[heroIteration].ClearButtonHeroIconVisuals();
            _heroIcons[heroIteration].SetButtonInteractability(false);
        }
        
        _playButton.UpdateBossAndHeroSelectionIcons();
        
        return 10;
    }
}
