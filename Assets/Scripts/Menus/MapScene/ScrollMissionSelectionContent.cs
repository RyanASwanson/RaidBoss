using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMissionSelectionContent : ScrollUIContents
{
    [Space]
    [SerializeField] private TextWithBackground _titleText;
    private Vector3 _titleScale;

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
    [SerializeField] private RectTransform[] _highlightIcons;

    [SerializeField] private RectTransform _bossHighlightHolder;
    [SerializeField] private Vector2 _bossHighlightOffset;
    [SerializeField] private RectTransform _difficultyHighlightHolder;
    [SerializeField] private Vector2 _difficultyHighlightOffset;
    [SerializeField] private RectTransform[] _heroHighlightHolders;
    [SerializeField] private Vector2 _heroHighlightOffset;
    [SerializeField] private RectTransform[] _missionModifiersHighlightHolders;
    [SerializeField] private Vector2 _missionModifierHighlightOffset;
    
    [Space]
    [SerializeField] private SelectionPlayButton _playButton;
    
    private MissionSO _selectedMission;
    
    public override int UpdateContentsAndCountLines()
    {
        _selectedMission = MapController.Instance.GetSelectedMission().GetAssociatedMission();
        
        _titleText.UpdateText(_selectedMission.GetMissionName());
        _titleScale.Set(_selectedMission.GetMissionTitleScale(),_selectedMission.GetMissionTitleScale(),_selectedMission.GetMissionTitleScale());
        _titleText.transform.localScale = _titleScale;

        _bossIcon.sprite = _selectedMission.GetAssociatedLevel().GetLevelBoss().GetBossIcon();

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

        DisplayMissionModifierContents();

        DisplayHighlightContents();
        
        _playButton.UpdateBossAndHeroSelectionIcons();
        
        return 10;
    }

    private void DisplayMissionModifierContents()
    {
        if (_selectedMission.GetMissionModifiers().Length > 0)
        {
            _noModifiersText.SetActive(false);
            _modifierIconsHolder.gameObject.SetActive(true);
            
            Vector2 modifierPosition = new();
            
            for (int i = 0; i < _missionModifierIcons.Length; i++)
            {
                if (i >= _selectedMission.GetMissionModifiers().Length)
                {
                    _missionModifierIcons[i].gameObject.SetActive(false);
                }
                else
                {
                    _missionModifierIcons[i].gameObject.SetActive(true);
                    _missionModifierIcons[i].SetAssociatedModifier(_selectedMission.GetMissionModifiers()[i]);

                    modifierPosition.Set((-_distanceBetweenMissionModifierIcons / 2) *
                        (_selectedMission.GetMissionModifiers().Length - 1) + (_distanceBetweenMissionModifierIcons*i), 0);
                    
                    _missionModifierIcons[i].SetAnchoredPosition(modifierPosition);

                }
            }
        }
        else
        {
            _noModifiersText.SetActive(true);
            _modifierIconsHolder.gameObject.SetActive(false);
        }
    }

    private void DisplayHighlightContents()
    {
        int highLightID = 0;
        Vector2 highLightLocation = new Vector3();
        MissionDisplayHighlight highLight;
        
        for (; highLightID < _selectedMission.GetMissionDisplayHighlights().Length; highLightID++)
        {
            highLight = _selectedMission.GetMissionDisplayHighlights()[highLightID];
            switch (highLight.HighlightType)
            {
                case EMissionDisplayHighlightType.Boss:
                {
                    _highlightIcons[highLightID].SetParent(_bossHighlightHolder);
                    highLightLocation = _bossHighlightOffset;
                    break;
                }
                case EMissionDisplayHighlightType.Difficulty:
                {
                    _highlightIcons[highLightID].SetParent(_difficultyHighlightHolder);
                    highLightLocation = _difficultyHighlightOffset;
                    break;
                }
                case EMissionDisplayHighlightType.Hero:
                {
                    _highlightIcons[highLightID].SetParent(_heroHighlightHolders[highLight.HightlightID]);
                    highLightLocation =  _heroHighlightOffset;
                    break;
                }
                case EMissionDisplayHighlightType.MissionModifier:
                {
                    _highlightIcons[highLightID].SetParent(_missionModifiersHighlightHolders[highLight.HightlightID]);
                    highLightLocation = _missionModifierHighlightOffset;
                    break;
                }
            }
            
            _highlightIcons[highLightID].localScale = Vector3.one;
            _highlightIcons[highLightID].anchoredPosition = highLightLocation;
            //_highlightIcons[highLightID].anchoredPosition = highLightLocation;
            _highlightIcons[highLightID].gameObject.SetActive(true);
        }

        for (; highLightID < _highlightIcons.Length; highLightID++)
        {
            _highlightIcons[highLightID].gameObject.SetActive(false);
        }
    }
}
