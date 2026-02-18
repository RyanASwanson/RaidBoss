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
    [SerializeField] private GameObject _noModifiersText;
    [SerializeField] private RectTransform _modifierIconsHolder;

    [Space] 
    [SerializeField] private float _distanceBetweenMissionModifierIcons;
    [SerializeField] private SelectMissionModifierButton[] _missionModifierIcons;
    
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

        if (missionSO.GetMissionModifiers().Length > 0)
        {
            _noModifiersText.SetActive(false);
            _modifierIconsHolder.gameObject.SetActive(true);
            
            _modifierIconsHolder.anchoredPosition = new Vector2(
                (-_distanceBetweenMissionModifierIcons/2) * (missionSO.GetMissionModifiers().Length-1),0);
            
            for (int i = 0; i < _missionModifierIcons.Length; i++)
            {
                if (i >= missionSO.GetMissionModifiers().Length)
                {
                    _missionModifierIcons[i].gameObject.SetActive(false);
                }
                else
                {
                    _missionModifierIcons[i].gameObject.SetActive(true);
                    _missionModifierIcons[i].SetAssociatedModifier(missionSO.GetMissionModifiers()[i]);
          
                }
            }
        }
        else
        {
            _noModifiersText.SetActive(true);
            _modifierIconsHolder.gameObject.SetActive(false);
        }
        
        _playButton.UpdateBossAndHeroSelectionIcons();
        
        return 10;
    }
}
