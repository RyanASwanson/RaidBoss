using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScrollMissionSelectionContent : ScrollUIContents
{
    [Space]
    [SerializeField] private TextWithBackground _titleText;
    private Vector3 _titleScale;

    [Space] 
    [SerializeField] private SelectBossLevelButton _bossIcon;
    [SerializeField] private DifficultyButton _difficultyIcon;

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
    [Header("Extended Sections")]
    [SerializeField] private CurveProgression _extendedBossSection;
    [SerializeField] private TextWithBackground _extendedBossNameText;
    
    [SerializeField] private CharacterAbilityDetails[] _bossAbilityDetails;

    [Space] 
    [SerializeField] private CurveProgression _extendedHeroSection;
    [SerializeField] private TextWithBackground _extendedHeroNameText;
    
    [SerializeField] private CharacterAbilityDetails[] _heroAbilityDetails;

    [Space] 
    [SerializeField] private CurveProgression _extendedDifficultySection;
    [SerializeField] private TextWithBackground _extendedDifficultyNameText;
    [SerializeField] private TextWithBackground _difficultyBossMaxHealthText;
    [SerializeField] private TextWithBackground _difficultyBossMaxStaggerText;
    [SerializeField] private TextWithBackground _difficultyBossDamageText;
    [SerializeField] private TextWithBackground _difficultyBossSpeedText;
    [SerializeField] private TextWithBackground _difficultyMaxHeroesText;
    
    [Space]
    [SerializeField] private CurveProgression _extendedMissionModifiersSection;
    [SerializeField] private TextWithBackground _extendedMissionModifierNameText;
    
    [SerializeField] private CharacterAbilityDetails _missionModifierDetails;

    private CurveProgression _currentDisplayingExtendedSection;
    private CurveProgression _currentHoveredOverExtendedSection;

    private BossSO _currentHoveredOverBoss;
    private HeroSO _currentHoveredOverHero;
    private EGameDifficulty _currentHoveredOverDifficulty;
    private MissionModifierSO _currentHoveredOverMissionModifier;
    
    [Space]
    [SerializeField] private SelectionPlayButton _playButton;
    
    private MissionSO _selectedMission;

    private void OnEnable()
    {
        HideAllExtendedSections();
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    public override int UpdateContentsAndCountLines()
    {
        _selectedMission = MapController.Instance.GetSelectedMission().GetAssociatedMission();
        
        _titleText.UpdateText(_selectedMission.GetMissionName());
        _titleScale.Set(_selectedMission.GetMissionTitleScale(),_selectedMission.GetMissionTitleScale(),_selectedMission.GetMissionTitleScale());
        _titleText.transform.localScale = _titleScale;
        
        _bossIcon.SetAssociatedLevel(_selectedMission.GetAssociatedLevel());
        
        _difficultyIcon.UpdateDifficulty(SelectionManager.Instance.GetSelectedDifficulty());

        List<HeroSO> selectedHeroes = SelectionManager.Instance.GetAllSelectedHeroes();

        int heroIteration;
        for (heroIteration = 0; heroIteration < selectedHeroes.Count; heroIteration++)
        {
            _heroIcons[heroIteration].SetAssociatedHero(selectedHeroes[heroIteration]);
            _heroIcons[heroIteration].SetButtonInteractability(true);
        }

        for (; heroIteration < _heroIcons.Length; heroIteration++)
        {
            _heroIcons[heroIteration].ClearAssociatedHero();
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
    
    #region Extended Sections

    #region Update Contents
    public void UpdateExtendedSectionContents()
    {
        if (!_currentDisplayingExtendedSection.IsUnityNull())
        {
            return;
        }
        
        if (_currentHoveredOverExtendedSection.IsUnityNull())
        {
            return;
        }

        if (!MapController.Instance.IsUnityNull())
        {
            if (MapController.Instance.GetIsClickAndDraggingCamera())
            {
                return;
            }
        }
        
        if (!_currentHoveredOverBoss.IsUnityNull())
        {
            UpdateExtendedBossSectionContents();
        }

        if (!_currentHoveredOverHero.IsUnityNull())
        {
            UpdateExtendedHeroSectionContents();
        }

        if (_currentHoveredOverDifficulty != EGameDifficulty.Empty)
        {
            UpdateExtendedDifficultySectionContents();
        }

        if (!_currentHoveredOverMissionModifier.IsUnityNull())
        {
            UpdateExtendedMissionModifierSectionContents();
        }

        _currentHoveredOverExtendedSection.StartMovingUpOnCurve();
        
        _currentDisplayingExtendedSection = _currentHoveredOverExtendedSection;
    }

    private void UpdateExtendedBossSectionContents()
    {
        _extendedBossNameText.UpdateText(_currentHoveredOverBoss.GetBossName());
        _extendedBossNameText.UpdateTextColor(_currentHoveredOverBoss.GetBossHighlightedColor());
        
        for (int i = 0; i < _bossAbilityDetails.Length; i++)
        {
            _bossAbilityDetails[i].UpdateBossAbilityDetails(_currentHoveredOverBoss,i);
        }
    }
    
    private void UpdateExtendedHeroSectionContents()
    {
        _extendedHeroNameText.UpdateText(_currentHoveredOverHero.GetHeroName());
        _extendedHeroNameText.UpdateTextColor(_currentHoveredOverHero.GetHeroHighlightedColor());

        for (int i = 0; i < _heroAbilityDetails.Length; i++)
        {
            _heroAbilityDetails[i].UpdateHeroAbilityDetails(_currentHoveredOverHero,i);
        }
    }
    
    private void UpdateExtendedDifficultySectionContents()
    {
        _extendedDifficultyNameText.UpdateText(SelectionManager.Instance.GetDifficultyNameFromDifficulty(_currentHoveredOverDifficulty));
        _extendedDifficultyNameText.UpdateTextColor(SelectionManager.Instance.GetDifficultyColorFromDifficulty(_currentHoveredOverDifficulty));
        
        _difficultyBossMaxHealthText.UpdateText(SelectionManager.Instance.GetHealthMultiplierFromDifficulty(_currentHoveredOverDifficulty).ToString() + "X");
        _difficultyBossMaxStaggerText.UpdateText(SelectionManager.Instance.GetStaggerMultiplierFromDifficulty(_currentHoveredOverDifficulty).ToString() + "X");
        _difficultyBossDamageText.UpdateText(SelectionManager.Instance.GetDamageMultiplierFromDifficulty(_currentHoveredOverDifficulty).ToString() + "X");
        _difficultyBossSpeedText.UpdateText(SelectionManager.Instance.GetSpeedMultiplierFromDifficulty(_currentHoveredOverDifficulty).ToString() + "X");
        _difficultyMaxHeroesText.UpdateText(SelectionManager.Instance.GetHeroLimitFromDifficulty(_currentHoveredOverDifficulty).ToString());
    }
    
    private void UpdateExtendedMissionModifierSectionContents()
    {
        _extendedMissionModifierNameText.UpdateText(_currentHoveredOverMissionModifier.GetModifierName());
        _extendedMissionModifierNameText.UpdateTextColor(_currentHoveredOverMissionModifier.GetModifierHighlightedColor());
        
        _missionModifierDetails.UpdateMissionModifierDetails(_currentHoveredOverMissionModifier);
    }

    private void HideAllExtendedSections()
    {
        return;
    }
    #endregion

    #region Hovered Over
    public void GeneralSectionHoveredOver()
    {
        UpdateExtendedSectionContents();
    }

    public void BossSectionHoveredOver(BossSO bossSO)
    {
        _currentHoveredOverBoss = bossSO;
        _currentHoveredOverExtendedSection = _extendedBossSection;
        GeneralSectionHoveredOver();
    }

    public void HeroSectionHoveredOver(HeroSO heroSO)
    {
        _currentHoveredOverHero = heroSO;
        _currentHoveredOverExtendedSection = _extendedHeroSection;
        GeneralSectionHoveredOver();
    }

    public void DifficultySectionHoveredOver(EGameDifficulty gameDifficulty)
    {
        _currentHoveredOverDifficulty = gameDifficulty;
        _currentHoveredOverExtendedSection = _extendedDifficultySection;
        GeneralSectionHoveredOver();
    }

    public void MissionModifierSectionHoveredOver(MissionModifierSO missionModifierSO)
    {
        _currentHoveredOverMissionModifier = missionModifierSO;
        _currentHoveredOverExtendedSection = _extendedMissionModifiersSection;
        GeneralSectionHoveredOver();
    }
    
    #endregion 

    #region Not Hovered Over
    public void GeneralSectionNotHoveredOver()
    {
        _currentHoveredOverExtendedSection = null;
        
        if (!_currentDisplayingExtendedSection.IsUnityNull())
        {
            _currentDisplayingExtendedSection.StartMovingDownOnCurve();
        }
    }

    public void SectionFullyHidden()
    {
        _currentDisplayingExtendedSection = null;
        UpdateExtendedSectionContents();
    }

    public void BossSectionNotHoveredOver(BossSO bossSO)
    {
        _currentHoveredOverBoss = null;
        GeneralSectionNotHoveredOver();
    }

    public void HeroSectionNotHoveredOver(HeroSO heroSO)
    {
        _currentHoveredOverHero = null;
        GeneralSectionNotHoveredOver();
    }

    public void DifficultySectionNotHoveredOver(EGameDifficulty gameDifficulty)
    {
        _currentHoveredOverDifficulty = EGameDifficulty.Empty;
        GeneralSectionNotHoveredOver();
    }

    public void MissionModifierSectionNotHoveredOver(MissionModifierSO missionModifierSO)
    {
        _currentHoveredOverMissionModifier = null;
        GeneralSectionNotHoveredOver();
    }
    #endregion
    
    #endregion

    private void SubscribeToEvents()
    {
        SelectionManager.Instance.GetBossHoveredOverEvent().AddListener(BossSectionHoveredOver);
        SelectionManager.Instance.GetHeroHoveredOverEvent().AddListener(HeroSectionHoveredOver);
        SelectionManager.Instance.GetDifficultyHoveredOverEvent().AddListener(DifficultySectionHoveredOver);
        SelectionManager.Instance.GetMissionModifierHoveredOverEvent().AddListener(MissionModifierSectionHoveredOver);
        
        SelectionManager.Instance.GetBossNotHoveredOverEvent().AddListener(BossSectionNotHoveredOver);
        SelectionManager.Instance.GetHeroNotHoveredOverEvent().AddListener(HeroSectionNotHoveredOver);
        SelectionManager.Instance.GetDifficultyNotHoveredOverEvent().AddListener(DifficultySectionNotHoveredOver);
        SelectionManager.Instance.GetMissionModifierNotHoveredOverEvent().AddListener(MissionModifierSectionNotHoveredOver);
        
        
    }

    private void UnsubscribeFromEvents()
    {
        SelectionManager.Instance.GetBossHoveredOverEvent().RemoveListener(BossSectionHoveredOver);
        SelectionManager.Instance.GetHeroHoveredOverEvent().RemoveListener(HeroSectionHoveredOver);
        SelectionManager.Instance.GetDifficultyHoveredOverEvent().RemoveListener(DifficultySectionHoveredOver);
        SelectionManager.Instance.GetMissionModifierHoveredOverEvent().RemoveListener(MissionModifierSectionHoveredOver);
        
        SelectionManager.Instance.GetBossNotHoveredOverEvent().RemoveListener(BossSectionNotHoveredOver);
        SelectionManager.Instance.GetHeroNotHoveredOverEvent().RemoveListener(HeroSectionNotHoveredOver);
        SelectionManager.Instance.GetDifficultyNotHoveredOverEvent().RemoveListener(DifficultySectionNotHoveredOver);
        SelectionManager.Instance.GetMissionModifierNotHoveredOverEvent().RemoveListener(MissionModifierSectionNotHoveredOver);
    }
}
