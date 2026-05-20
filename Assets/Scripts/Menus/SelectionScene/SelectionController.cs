using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SelectionController : MonoBehaviour
{
    public static SelectionController Instance;
    
    [Header("Boss")]
    [SerializeField] private BossPillar _bossPillar;
    
    [SerializeField] private BossBackgroundChanger _bossBackgroundChanger;

    [SerializeField] private List<SelectBossLevelButton> _bossLevelSelectionButtons;

    [SerializeField] private Camera _bossCamera;

    [Space]
    [Header("Center")]
    [Header("Center-Boss")]
    [SerializeField] private GameObject _bossDescription;
    [SerializeField] private CurveProgression _bossDescriptionCurve;

    [Space]
    [SerializeField] private TextWithBackground _bossNameText;

    [Space]
    [SerializeField] private List<Image> _bossAbilityImageIcons;

    [Space]
    [SerializeField] private ScrollUISelection _bossScrollUI;
    
    private int _currentBossAbilityID = -1;

    [Header("Center-MissionModifiers")] 
    [SerializeField] private Button _missionModifierTabButton;

    [SerializeField] private float _missionModifierSFXPitchIncrease;

    [SerializeField] private CurveProgression _activeModifiersCurve;

    [SerializeField] private CurveProgression _selectableModifiersCurve;
    
    [SerializeField] private List<ActiveMissionModifierSelectionButton> _activeMissionModifierSelectionButtons;
    [SerializeField] private List<MissionModifierSelectionButton> _missionModifierSelectionButtons;
    
    private bool _areModifiersActive = false;

    [Header("Center-General")]
    [SerializeField] private SelectionPlayButton _fightButton;
    private bool _maxCharactersSelected = false;

    [SerializeField] private GameObject _characterInformationLockVisuals;
    
    public static bool IsSelectionInformationLocked = false;
    public static CharacterSO SelectionLockedCharacter;

    [Space]
    [Header("Center-Hero")]
    [SerializeField] private GameObject _heroDescription;
    [SerializeField] private CurveProgression _heroDescriptionCurve;

    [Space]
    [SerializeField] private TextWithBackground _heroNameText;

    [Space]
    [SerializeField] private List<StatCounter> _statCounters;

    [Space]
    [SerializeField] private Image _rangeIcon;
    [SerializeField] private List<Sprite> _rangeIconSprites;
    [SerializeField] private Color[] _rangeTextColors;
    [SerializeField] private TextWithBackground _rangeText;

    [Space] 
    [SerializeField] private Image _difficultyIcon;
    [SerializeField] private List<Sprite> _difficultyIconSprites;
    [SerializeField] private Color[] _difficultyTextColors;
    [SerializeField] private TextWithBackground _difficultyText;

    [Space]
    [SerializeField] private Image _heroBasicIcon;
    [SerializeField] private Image _heroManualIcon;
    [SerializeField] private Image _heroPassiveIcon;

    private BossSO _lastBossHoveredOver;
    private BossSO _bossUIToDisplay;
    private HeroSO _lastHeroHoveredOver;
    private HeroSO _heroUIToDisplay;
    private MissionModifierSO _lastMissionModifierHoveredOver;
    private MissionModifierSO _missionModifierUIToDisplay;

    [Space] 
    [SerializeField] private ScrollUISelection _heroScrollUI;

    private int _currentHeroAbilityID = -1;
    private int _currentMissionModifierID = -1;

    [Space]
    [Header("Hero")]
    [SerializeField] private List<HeroPillar> _heroPillars = new List<HeroPillar>();

    [SerializeField] private float _heroProgressBackgroundMoveTime;
    [SerializeField] private float _heroProgressBackgroundMaxSize;
    [SerializeField] private AnimationCurve _heroProgressBackgroundMoveCurve;
    
    [SerializeField] private AnimationCurve _heroProgressBackgroundCurve;
    [SerializeField] private GameObject _heroProgressBackground;
    
    [SerializeField] private GeneralVFXFunctionality _heroProgressBackgroundParticles;

    private float _heroProgressBackgroundStartPosition;
    private float _heroProgressBackgroundEndPosition;

    private float _heroProgressBackgroundMoveProgress;

    private Coroutine _heroProgressBackgroundMoveProcessCoroutine;
    
    [SerializeField] private List<MeshRenderer> _heroBackgrounds;
    
    [SerializeField] private List<SelectHeroButton> _heroSelectionButtons = new List<SelectHeroButton>();

    private int _previousMaxHeroes;
    
    [SerializeField] private Camera _heroCamera;

    // Start is called before the first frame update
    void OnEnable()
    {
        Instance = this;
        SubscribeToEvents();

        IsSelectionInformationLocked = false;
        
        SelectionManager.Instance.SetSelectedGameMode(EGameMode.Free);

        BossSideStart();
        CenterStart();
        HeroSideStart();
    }

    #region Boss Side
    private void BossSideStart()
    {
        SetUpBossButtons();
    }
    
    private void SetUpBossButtons()
    {
        for (int i = 0; i < _bossLevelSelectionButtons.Count; i++)
        {
            _bossLevelSelectionButtons[i].SetAssociatedLevel(SaveManager.Instance.GetLevelsInGame()[i]);
        }
    }

    private void NewBossAddedSelection(BossSO bossSO)
    {
        _bossPillar.ShowBossOnPillar(bossSO, false);
        
        PlayBossSelectedAudio(bossSO);

        CheckMaxCharactersSelected();
        
        ShowBossBackground();
    }

    private void BossRemovedSelection(BossSO bossSO)
    {
        _bossPillar.BossOnPillarDeselected();

        PlayBossDeselectedAudio(bossSO);

        CheckMaxCharactersNoLongerSelected();

        HideBossBackground();
    }

    private void SwapBoss(BossSO previousBoss)
    {
        //Removes the previous boss
        foreach (SelectBossLevelButton bossButton in _bossLevelSelectionButtons)
        {
            if (bossButton.GetAssociatedBoss() == previousBoss)
            {
                //bossButton.SelectBossLevelLeftClicked();
                bossButton.BossLevelSwapped();
            }

        }
    }
    
    public void ForceBossButtonPressFromID(int id)
    {
        ForceBossButtonPress(_bossLevelSelectionButtons[id]);
    }
    
    public void ForceBossButtonPress(SelectBossLevelButton selectButton)
    {
        selectButton.SelectBossLevelLeftClicked();
    }
    #endregion

    #region Center

    private void CenterStart()
    {
        MissionModifierStart();
        FightButtonStartingInteractability();
    }

    #region Center - Boss

    private void BossHoveredOver(BossSO bossSO)
    {
        _lastHeroHoveredOver = null;
        _lastBossHoveredOver = bossSO;
        
        if (SelectionManager.Instance.GetSelectedBoss() == bossSO)
        {
            OldBossHoveredOver(bossSO);
        }
        else
        {
            NewBossHoveredOver(bossSO);
        }
    }

    private void NewBossHoveredOver(BossSO bossSO)
    {
        GeneralBossHoveredOver(bossSO);
        
        _bossPillar.ShowBossOnPillar(bossSO, true);
        
    }

    private void OldBossHoveredOver(BossSO bossSO)
    {
        GeneralBossHoveredOver(bossSO);
        
        _bossPillar.PlayBossHoverAnimation();
    }

    private void GeneralBossHoveredOver(BossSO bossSO)
    {
        AttemptDisplayBossInformation(bossSO);

        UpdateHeroButtonDifficultyBeaten(bossSO);
    }

    /// <summary>
    /// Called when a boss is no longer hovered over
    /// </summary>
    /// <param name="bossSO"> The scriptable object of the boss no longer hovered over</param>
    private void BossNotHoveredOver(BossSO bossSO)
    {
        if (SelectionManager.Instance.AtMaxBossSelected())
        {
            if (bossSO != SelectionManager.Instance.GetSelectedBoss())
            {
                NewBossHoveredOver(SelectionManager.Instance.GetSelectedBoss());
            }
        }
        else
        {
            _bossPillar.AnimateOutBossOnPillar();
        }
    }

    public void AttemptDisplayBossInformation(BossSO bossSO)
    {
        if (!IsSelectionInformationLocked)
        {
            DisplayBossInformation(bossSO);
        }
    }

    public void DisplayBossInformation(BossSO bossSO)
    {
        _bossUIToDisplay = bossSO;
        
        //Show boss description and hide hero description
        _bossDescription.SetActive(true);
        HideFullHeroDescription();

        _bossDescriptionCurve.StartMovingUpOnCurve();
        
        _bossNameText.UpdateText(bossSO.GetBossSelectionScreenName());
        _bossNameText.UpdateTextColor(bossSO.GetBossHighlightedColor());

        HideBossAbilityDescription();

        DisplayAbilityIconsForBoss(bossSO);
    }

    private void InformationLockBoss(BossSO bossSO)
    {
        LockCharacterInformation(bossSO);
        
        DisplayBossInformation(bossSO);
    }


    public void BossAbilityIconPressed(int abilityID)
    {
        if (_currentBossAbilityID == -1)
        {
            ShowBossAbilityDescription(abilityID);
        }
        else if (abilityID != _currentBossAbilityID)
        {
            SwapBossAbilityDescription(abilityID);
        }
        else
        {
            HideBossAbilityDescription();
        }
    }

    public void BossAbilityHoverBegin(int abilityID)
    {
        ShowBossAbilityDescription(abilityID);
    }
    public void BossAbilityHoverEnd()
    {
        HideBossAbilityDescription();
    }

    private void DisplayAbilityIconsForBoss(BossSO bossSO)
    {
        List<BossAbilityInformation> bossAbilityInfo = bossSO.GetBossAbilityInformation();
        for(int i = 0; i < bossAbilityInfo.Count; i++)
        {
            _bossAbilityImageIcons[i].sprite = bossAbilityInfo[i]._abilityImage;
        }
    }

    public void ShowBossAbilityDescription(int abilityID)
    {
        _currentBossAbilityID = abilityID;
        _bossScrollUI.ShowNewScroll(90);
    }

    /// <summary>
    /// Changes what the currently displayed boss description is.
    /// Animates the boss description to change the visuals
    /// </summary>
    /// <param name="abilityID"></param>
    private void SwapBossAbilityDescription(int abilityID)
    {
        _currentBossAbilityID = abilityID;
        _bossScrollUI.ShowNewScroll(90);
    }

    /// <summary>
    /// Animates the boss description to hide it from view.
    /// Removes the currently displayed boss ability id
    /// </summary>
    private void HideBossAbilityDescription()
    {
        _currentBossAbilityID = -1;
        
        _bossScrollUI.HideScroll();
    }

    private void HideFullBossDescription()
    {
        _bossDescription.SetActive(false);
    }

    private void ShowBossBackground()
    {
        _bossBackgroundChanger.UpdateBackground(SelectionManager.Instance.GetSelectedLevel());
    }

    private void HideBossBackground()
    {
        _bossBackgroundChanger.HideCurrentBackground();
    }

    #endregion
    
    #region Center - Mission Modifiers

    private void MissionModifierStart()
    {
        PressStartingSelectedMissionModifiers();
    }

    private void PressStartingSelectedMissionModifiers()
    {
        foreach (MissionModifierSelectionButton modifierSelectionButton in _missionModifierSelectionButtons)
        {
            modifierSelectionButton.SetUpMissionModifierSelectionButton();
        }
        
        int i;
        for (i = 0; i < SelectionManager.Instance.GetCurrentMissionModifiers().Count; i++)
        {
            _missionModifierSelectionButtons[SelectionManager.Instance.GetCurrentMissionModifiers()[i].GetModifierID()].SetStartingMissionModifierStatusToPressed();
            _activeMissionModifierSelectionButtons[i].SetStartingMissionModifierStatus(true);
        }

        for (; i < _activeMissionModifierSelectionButtons.Count; i++)
        {
            _activeMissionModifierSelectionButtons[i].SetStartingMissionModifierStatus(false);
        }
    }

    public void MissionModifierHoveredOver(MissionModifierSO missionModifier)
    {
        _lastMissionModifierHoveredOver = missionModifier;
        
        if (SelectionManager.Instance.GetCurrentMissionModifiers().Contains(missionModifier))
        {
            OldMissionModifierHoveredOver(missionModifier);
        }
        else
        {
            NewMissionModifierHoveredOver(missionModifier);
        }
    }

    public void NewMissionModifierHoveredOver(MissionModifierSO missionModifier)
    {
        GeneralMissionModifierHoveredOver(missionModifier);
        
        GetLastActiveMissionModifier().SetCurrentHoveredMissionModifierID(missionModifier);
    }

    public void OldMissionModifierHoveredOver(MissionModifierSO missionModifier)
    {
        GeneralMissionModifierHoveredOver(missionModifier);
    }

    public void GeneralMissionModifierHoveredOver(MissionModifierSO missionModifier)
    {
        ShowMissionModifierDescription(missionModifier);
    }

    public void MissionModifierNotHoveredOver(MissionModifierSO missionModifier)
    {
        GetLastActiveMissionModifier().ResetCurrentHoveredMissionModifier();
        HideMissionModifierDescription();
    }
    
    public void ShowMissionModifierDescription(MissionModifierSO missionModifier)
    {
        _missionModifierUIToDisplay = missionModifier;
        _currentMissionModifierID = missionModifier.GetModifierID();
        
        _heroScrollUI.ShowNewScroll(90);
    }
    
    public void SwapMissionModifierDescription(MissionModifierSO missionModifier)
    {
        _missionModifierUIToDisplay = missionModifier;
        _currentMissionModifierID = missionModifier.GetModifierID();
        
        _heroScrollUI.HideScroll();
        _heroScrollUI.ShowNewScroll(90);
    }

    public void HideMissionModifierDescription()
    {
        _currentMissionModifierID = -1;
        
        _heroScrollUI.HideScroll();
    }

    public void ShowMissionModifierTab()
    {
        _selectableModifiersCurve.StartMovingUpOnCurve();
    }

    public void HideMissionModifierTab()
    {
        _selectableModifiersCurve.StartMovingDownOnCurve();
    }

    public void MissionModifierTabHoverBegin()
    {
        ShowMissionModifierTab();
    }

    public void MissionModifierTabHoverEnd()
    {
        HideMissionModifierTab();
    }

    private ActiveMissionModifierSelectionButton GetLastActiveMissionModifier()
    {
        return _activeMissionModifierSelectionButtons[
            Mathf.Clamp(SelectionManager.Instance.GetCurrentMissionModifiers().Count,0,SelectionManager.MAX_MISSION_MODIFIERS-1)];
    }

    private void MissionModifierSelected(MissionModifierSO missionModifier)
    {
        int modifierNum = SelectionManager.Instance.GetMissionModifierCount() - 1;
        
        _activeMissionModifierSelectionButtons[modifierNum].UpdateModifierImage(true);

        PlayMissionModifierSelectedAudio(missionModifier);
    }

    private void MissionModifierDeselected(MissionModifierSO missionModifier)
    {
        for (int i = SelectionManager.MAX_MISSION_MODIFIERS-1; i >= SelectionManager.Instance.GetIndexOfLastMissionModifierRemoved(); i--)
        {
            _activeMissionModifierSelectionButtons[i].UpdateModifierImage(false);
        }
        PlayMissionModifierDeselectedAudio(missionModifier);
    }

    private void MissionModifierSwap(MissionModifierSO missionModifier)
    {
        ForceMissionModifierButtonPress(GetModifierButtonFromSO(missionModifier));
    }
    
    public void ForceMissionModifierButtonPress(MissionModifierSO missionModifier)
    {
        ForceMissionModifierButtonPress(_missionModifierSelectionButtons[missionModifier.GetModifierID()]);
    }

    public void ForceMissionModifierButtonPress(int modifierID)
    {
        ForceMissionModifierButtonPress(_missionModifierSelectionButtons[modifierID]);
    }
    

    public void ForceMissionModifierButtonPress(MissionModifierSelectionButton missionModifierSelectionButton)
    {
        missionModifierSelectionButton.ButtonPressed();
    }
    
    private MissionModifierSelectionButton GetModifierButtonFromSO(MissionModifierSO missionModifier)
    {
        foreach (MissionModifierSelectionButton modifierButton in _missionModifierSelectionButtons)
        {
            if (modifierButton.GetMissionModifier() == missionModifier)
            {
                return modifierButton;
            }
        }

        return null;
    }
    #endregion

    #region Center - General
    private void FightButtonStartingInteractability()
    {
        _fightButton.SetUpPlayButton();
    }

    private void LockCharacterInformation(CharacterSO lockCharacter)
    {
        IsSelectionInformationLocked = true;
        SelectionLockedCharacter = lockCharacter;
        
        _characterInformationLockVisuals.SetActive(true);
        PlaySelectionInformationLockAudio();
    }

    private void LockMissionInformation()
    {
        IsSelectionInformationLocked = true;
    }

    private void UnlockCharacterInformation()
    {
        if (SelectionLockedCharacter.IsUnityNull())
        {
            return;
        }
        
        IsSelectionInformationLocked = false;
        SelectionLockedCharacter = null;
        
        _characterInformationLockVisuals.SetActive(false);
        PlaySelectionInformationUnlockAudio();
    }

    private void CheckMaxCharactersSelectionStatus()
    {
        CheckMaxCharactersNoLongerSelected();
        CheckMaxCharactersSelected();
    }

    /// <summary>
    /// Checks if the player has selected all the heroes and the boss
    /// </summary>
    private void CheckMaxCharactersSelected()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!DebugScript.Instance.IsUnityNull() && !DebugScript.Instance.RequiresMaxCharactersSelected)
        {
            MaxCharactersSelected();
            return;
        }
#endif
        if (SelectionManager.Instance.AtMaxBossSelected() && SelectionManager.Instance.AtMaxHeroesSelected())
        {
            MaxCharactersSelected();
        }
    }

    /// <summary>
    /// Called when all characters, both heroes and bosses, and selected.
    /// </summary>
    private void MaxCharactersSelected()
    {
        _maxCharactersSelected = true;
        _fightButton.MaxCharactersSelected(true);
    }
    
    private void CheckMaxCharactersNoLongerSelected()
    {
        if (_maxCharactersSelected)
        {
            NoLongerMaxHeroesSelected();
        }
    }

    private void NoLongerMaxHeroesSelected()
    {
        _maxCharactersSelected = false;
        _fightButton.MaxCharactersSelected(false);
    }

    private void HideFullBossHeroDescriptions()
    {
        HideFullHeroDescription();
        HideFullBossDescription();
    }

    private void DifficultySelected(EGameDifficulty difficulty)
    {
        PlayDifficultySelectedAudio(difficulty);
        HeroLimitChanged(difficulty);
    }
    
    /// <summary>
    /// Causes the game to proceed to the currently selected level
    /// Called by play button press
    /// </summary>
    public void PlayLevel()
    {
        SceneLoadManager.Instance.LoadCurrentlySelectedLevelSO();
    }
    #endregion

    #region Center - Hero
    private void HeroHoveredOver(HeroSO heroSO)
    {
        _lastBossHoveredOver = null;
        _lastHeroHoveredOver = heroSO;
        
        if (SelectionManager.Instance.GetAllSelectedHeroes().Contains(heroSO))
        {
            OldHeroHoveredOver(heroSO);
        }
        else
        {
            NewHeroHoveredOver(heroSO);
        }
    }

    private void NewHeroHoveredOver(HeroSO heroSO)
    {
        GeneralHeroHoveredOver(heroSO);

        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount();

        if (SelectionManager.Instance.AtMaxHeroesSelected())
        {
            heroPillarNum--;
        }

        _heroPillars[heroPillarNum ].ShowHeroOnPillar(heroSO, true);
    }

    private void OldHeroHoveredOver(HeroSO heroSO)
    {
        GeneralHeroHoveredOver(heroSO);

        HeroPillar heroPillar = FindHeroPillarWithHero(heroSO);
        if (!heroPillar.IsUnityNull())
        {
            heroPillar.PlayHeroHoverAnimation();
        }
    }

    private void GeneralHeroHoveredOver(HeroSO heroSO)
    {
        AttemptDisplayHeroInformation(heroSO);
    }

    public void HeroNotHoveredOver(HeroSO heroSO)
    {
        if (SelectionManager.Instance.GetAllSelectedHeroes().Contains(heroSO))
        {
            return;
        }

        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount();
        
        if (SelectionManager.Instance.AtMaxHeroesSelected() &&
            SelectionManager.Instance.GetHeroAtLastPosition() != heroSO)
        {
            heroPillarNum--;
        }

        _heroPillars[heroPillarNum].AnimateOutHeroOnPillar();

        _lastHeroHoveredOver = null;

        if (SelectionManager.Instance.AtMaxHeroesSelected() &&
            SelectionManager.Instance.GetHeroAtLastPosition() != heroSO)
        {
            NewHeroHoveredOver(SelectionManager.Instance.GetHeroAtLastPosition());
        }
    }

    private void AttemptDisplayHeroInformation(HeroSO heroSO)
    {
        if (!IsSelectionInformationLocked)
        {
            DisplayHeroInformation(heroSO);
        }
    }

    private void DisplayHeroInformation(HeroSO heroSO)
    {
        _heroUIToDisplay = heroSO;
        
        //Show hero description and hide boss description
        HideFullBossDescription();
        _heroDescription.SetActive(true);
        
        _heroDescriptionCurve.StartMovingUpOnCurve();

        //Updates the text to display the heroes name
        _heroNameText.UpdateText(heroSO.GetHeroName());
        _heroNameText.UpdateTextColor(heroSO.GetHeroHighlightedColor());
        
        float textScale = heroSO.GetSelectionDisplayScaleMultiplier();
        _heroNameText.transform.localScale = new Vector3(textScale,textScale,textScale);

        HideHeroAbilityDescription();

        //Displays all stats associated for the hero on the counters
        DisplayStatsForHero(heroSO);

        DisplayHeroRangeAndDifficulty(heroSO);

        DisplayAbilityIconsForHero(heroSO);
    }

    private void InformationLockHero(HeroSO heroSO)
    {
        LockCharacterInformation(heroSO);
        
        DisplayHeroInformation(heroSO);
    }

    private void DisplayStatsForHero(HeroSO heroSO)
    {
        _statCounters[0].ShowStatNodes(heroSO.GetSurvivalStat());
        _statCounters[1].ShowStatNodes(heroSO.GetDamageStat());
        _statCounters[2].ShowStatNodes(heroSO.GetStaggerStat());
        _statCounters[3].ShowStatNodes(heroSO.GetSpeedStat());
        _statCounters[4].ShowStatNodes(heroSO.GetUtilityStat());
    }

    private void StopStatDisplayProcess()
    {
        foreach (StatCounter statCounter in _statCounters)
        {
            statCounter.StopShowNodeProcess();
        }
    }

    private void DisplayHeroRangeAndDifficulty(HeroSO heroSO)
    {
        _rangeIcon.sprite = _rangeIconSprites[(int)heroSO.GetHeroRange()];
        _rangeText.UpdateTextColor(_rangeTextColors[(int)heroSO.GetHeroRange()]);
        
        _difficultyIcon.sprite = _difficultyIconSprites[(int)heroSO.GetHeroDifficulty()];
        _difficultyText.UpdateTextColor(_difficultyTextColors[(int)heroSO.GetHeroDifficulty()]);
    }

    private void DisplayAbilityIconsForHero(HeroSO heroSO)
    {
        _heroBasicIcon.sprite = heroSO.GetHeroBasicAbilityIcon();
        _heroManualIcon.sprite = heroSO.GetHeroManualAbilityIcon();
        _heroPassiveIcon.sprite = heroSO.GetHeroPassiveAbilityIcon();
    }

    public void HeroAbilityIconPressed(int abilityID)
    {
        if(_currentHeroAbilityID == -1)
        {
            ShowHeroAbilityDescription(abilityID);
        }
        else if(abilityID != _currentHeroAbilityID)
        {
            SwapHeroAbilityDescription(abilityID);
        }
        else
        {
            HideHeroAbilityDescription();
        }
    }

    public void HeroAbilityHoverBegin(int abilityID)
    {
        ShowHeroAbilityDescription(abilityID);
    }
    public void HeroAbilityHoverEnd()
    {
        HideHeroAbilityDescription();
    }
    
    public void ShowHeroAbilityDescription(int abilityID)
    {
        _currentHeroAbilityID = abilityID;
        
        _heroScrollUI.ShowNewScroll(90);
    }
    
    private void SwapHeroAbilityDescription(int abilityID)
    {
        _currentHeroAbilityID = abilityID;
        
        _heroScrollUI.ShowNewScroll(90);
    }

    private void HideHeroAbilityDescription()
    {
        _currentHeroAbilityID = -1;
        
        _heroScrollUI.HideScroll();
    }

    private void HideFullHeroDescription()
    {
        _heroDescription.SetActive(false);
    }

    #endregion
    
    #endregion

    #region Hero Side
    private void HeroSideStart()
    {
        _previousMaxHeroes = SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty();
        MoveHeroPillar(0, true);
        ShowHeroPreviewPillars();
        SetUpHeroButtons();
    }

    private void SetUpHeroButtons()
    {
        for (int i = 0; i < _heroSelectionButtons.Count; i++)
        {
            _heroSelectionButtons[i].SetAssociatedHero(SaveManager.Instance.GetHeroesInGame()[i]);
        }
    }

    private void ShowHeroPreviewPillars()
    {
        int maxHeroes = SelectionManager.Instance.GetDefaultMaxHeroesCount();
        int maxHeroesOnDifficulty = SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty();
        int currentHeroes = SelectionManager.Instance.GetSelectedHeroesCount();

        // Iterate from the second pillar to the max amount of pillars
        for (int i = 1; i < maxHeroes; i++)
        {
            // If the current pillar is less than max amount of heroes on this difficulty
            // And if the current pillar is not equal to the current amount of selected heroes. We do this to prevent
            //  showing the preview of a pillar that is currently going up.
            //  This could happen if we were on normal (or heroic) with max heroes and then increased the difficulty by 1.
            if (i < maxHeroesOnDifficulty && i != currentHeroes)
            {
                _heroPillars[i].ShowPreviewPillar(true);
            }
            else
            {
                _heroPillars[i].ShowPreviewPillar(false);
            }
            
        }
    }

    private void HeroLimitChanged(EGameDifficulty difficulty)
    {
        //Determine if the hero limit went up or down

        //Hero limit went down
        if (SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty() < _previousMaxHeroes)
        {
            HeroLimitReduced();
        }
        //Hero limit went up
        else if (SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty() > _previousMaxHeroes)
        {
            HeroLimitIncreased();
        }
        ShowHeroPreviewPillars();
            
        _previousMaxHeroes = SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty();
        
        StartUpdateHeroProgressBackgroundProcess();
        CheckMaxCharactersSelectionStatus();
    }

    private void HeroLimitReduced()
    {
        List<HeroSO> heroesToRemove = new();

        for (int i = SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty(); i != _previousMaxHeroes; i++)
        {
            if (SelectionManager.Instance.GetAllSelectedHeroes().Count > i)
            {
                heroesToRemove.Add(SelectionManager.Instance.GetAllSelectedHeroes()[i]);
            }
            
            MoveHeroPillar(i, false);

        }

        foreach (SelectHeroButton heroButton in _heroSelectionButtons)
        {
            HeroSO associatedHero = heroButton.GetAssociatedHero();
            if (heroesToRemove.Contains(associatedHero))
            {
                heroButton.SelectHeroButtonLeftClicked();
                
                heroesToRemove.Remove(associatedHero);
            }
        }
    }

    private void HeroLimitIncreased()
    {
        if(SelectionManager.Instance.GetSelectedHeroesCount() == _previousMaxHeroes)
        {
            //_heroPillars[_previousMaxHeroes].MovePillar(true);
            MoveHeroPillar(_previousMaxHeroes, true);
        }
    }

    private void NewHeroAddedSelection(HeroSO heroSO)
    {
        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount();

        _heroPillars[heroPillarNum - 1].ShowHeroOnPillar(heroSO,false);

        if (heroPillarNum < SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty())
        {
            MoveHeroPillar(heroPillarNum, true);
        }
        
        PlayHeroSelectedAudio(heroSO);

        StartUpdateHeroProgressBackgroundProcess();
        CheckMaxCharactersSelected();
    }


    /// <summary>
    /// Removes a hero from the pillar and makes a pillar move down
    /// </summary>
    /// <param name="heroSO"></param>
    private void HeroRemovedSelection(HeroSO heroSO)
    {
        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount() + 1;

        if (heroPillarNum > 0 &&
            heroPillarNum < SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty())
        {
            // Shows the pillar preview just to be certain that it is properly displayed
            // Prevents situations of the pillar moving down and not showing the preview
            _heroPillars[heroPillarNum].ShowPreviewPillar(true);
            MoveHeroPillar(heroPillarNum, false);
        }

        PlayHeroDeselectedAudio(heroSO);
        
        //Remove the hero on the pillar that had a hero removed
        //_heroPillars[SelectionManager.Instance.GetIndexOfLastHeroRemoved()].RemoveHeroOnPillar();
        // Deselects the hero on the pillar
        _heroPillars[SelectionManager.Instance.GetIndexOfLastHeroRemoved()].HeroOnPillarDeselected();
        RearrangeHeroesOnPillars();

        StartUpdateHeroProgressBackgroundProcess();
        CheckMaxCharactersNoLongerSelected();
    }


    private void SwapHero(HeroSO hero)
    {
        ForceHeroButtonPress(GetHeroButtonFromSO(hero));
    }
    
    public void ForceHeroButtonPressFromID(int id)
    {
        ForceHeroButtonPress(_heroSelectionButtons[id]);
    }
    
    public void ForceHeroButtonPress(SelectHeroButton selectButton)
    {
        selectButton.SelectHeroButtonLeftClicked();
    }

    private SelectHeroButton GetHeroButtonFromSO(HeroSO hero)
    {
        foreach (SelectHeroButton heroButton in _heroSelectionButtons)
        {
            if (heroButton.GetAssociatedHero() == hero)
            {
                return heroButton;
            }
        }

        return null;
    }

    private HeroPillar FindHeroPillarWithHero(HeroSO searchHero)
    {
        foreach (HeroPillar heroPillar in _heroPillars)
        {
            if (heroPillar.GetStoredHero() == searchHero)
            {
                return heroPillar;
            }
        }
        return null;
    }

    private void RearrangeHeroesOnPillars()
    {
        MoveNextHeroBackToCurrentPillar(0);
    }

    private void MoveNextHeroBackToCurrentPillar(int pillarNum)
    {
        if (pillarNum + 1 >= SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty())
        {
            return;
        }
        
        if(!_heroPillars[pillarNum].HasHeroSelectedOnPillar() && _heroPillars[pillarNum+1].HasHeroSelectedOnPillar())
        {
            _heroPillars[pillarNum].ShowHeroOnPillar(_heroPillars[pillarNum + 1].GetStoredHero(),false, true,false);
            _heroPillars[pillarNum].PlayHeroHoverAnimation();
            
            _heroPillars[pillarNum + 1].RemoveHeroOnPillar();
            _heroPillars[pillarNum + 1].DestroyHeroSelectedOnPillar();
        }
        MoveNextHeroBackToCurrentPillar(pillarNum + 1);
        
    }

    private void MoveHeroPillar(int pillarNum, bool moveUp)
    {
        _heroPillars[pillarNum].MovePillar(moveUp);
    }

    #region HeroBackgrounds

    private void StartUpdateHeroProgressBackgroundProcess()
    {
        StopUpdateHeroProgressBackgroundProcess();

        _heroProgressBackgroundStartPosition = _heroProgressBackground.transform.localScale.x;
        
        _heroProgressBackgroundEndPosition = Mathf.Lerp(0.1f, _heroProgressBackgroundMaxSize,
            _heroProgressBackgroundCurve.Evaluate(SelectionManager.Instance.GetHeroSelectionProgress()));
        
        _heroProgressBackgroundParticles.PlayAllParticleSystems();
        _heroProgressBackgroundParticles.SetEmissionRateMultiplier(_heroProgressBackgroundEndPosition);

        _heroProgressBackgroundMoveProcessCoroutine = StartCoroutine(UpdateHeroProgressBackgroundProcess());
    }

    private void StopUpdateHeroProgressBackgroundProcess()
    {
        if (!_heroProgressBackgroundMoveProcessCoroutine.IsUnityNull())
        {
            StopCoroutine(_heroProgressBackgroundMoveProcessCoroutine);
        }
    }

    private IEnumerator UpdateHeroProgressBackgroundProcess()
    {
        float moveTimer = 0;
        
        while (moveTimer < 1)
        {
            moveTimer += Time.deltaTime / _heroProgressBackgroundMoveTime;
            _heroProgressBackgroundMoveProgress = moveTimer;
            UpdateHeroProgressBackground(_heroProgressBackgroundMoveProgress);
            yield return null;
        }
    }

    private void UpdateHeroProgressBackground(float moveProgress)
    {
        float heroProgress = Mathf.Lerp(_heroProgressBackgroundStartPosition, _heroProgressBackgroundEndPosition, 
            _heroProgressBackgroundMoveCurve.Evaluate(moveProgress));
        
        _heroProgressBackground.transform.localScale = new Vector3(heroProgress,
            _heroProgressBackground.transform.localScale.y, _heroProgressBackground.transform.localScale.z);
    }
    
    
    /*private void UpdateAllHeroBackgrounds()
    {
        for (int i = 0; i < SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty(); i++)
        {
            UpdateHeroBackground(i);
        }
    }

    private void UpdateHeroBackground(int backgroundID)
    {
        if (SelectionManager.Instance.GetAllSelectedHeroes().Count > backgroundID)
        {
            _heroBackgrounds[backgroundID].enabled = true;
            _heroBackgrounds[backgroundID].material.color = SelectionManager.Instance.GetAllSelectedHeroes()[backgroundID]
                .GetHeroHighlightedColor();

            _heroBackgrounds[backgroundID].material = SelectionManager.Instance.GetAllSelectedHeroes()[backgroundID]
                .GetHeroBackgroundMaterial();
        }
        else
        {
            _heroBackgrounds[backgroundID].enabled = false;
        }
    }*/
    #endregion

    private void UpdateHeroButtonDifficultyBeaten(BossSO bossSO)
    {
        foreach(SelectHeroButton selectHeroButton in _heroSelectionButtons)
        {
            selectHeroButton.SetBestDifficultyBeatenIcon(bossSO);
        }
    }
    
    #endregion
    
    #region Audio

    private void PlayBossSelectedAudio(BossSO selectedBoss)
    {
        /*AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.BossSelected);*/
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[selectedBoss.GetBossID()].SelectionSelectedAudio);
    }

    private void PlayBossDeselectedAudio(BossSO selectedBoss)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.BossDeselected);
    }
    
    private void PlayHeroSelectedAudio(HeroSO selectedHero)
    {
        /*AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.HeroSelected);*/
        
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificHeroAudio[selectedHero.GetHeroID()].SelectionSelectedAudio);
    }

    private void PlayHeroDeselectedAudio(HeroSO selectedHero)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.HeroDeselected);
    }

    private void PlaySelectionInformationLockAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.SelectionInformationLocked);
    }

    private void PlaySelectionInformationUnlockAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.SelectionInformationUnlocked);
    }

    private void PlayDifficultySelectedAudio(EGameDifficulty difficulty)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.DifficultySelected[(int)difficulty-1]);
    }

    private void PlayMissionModifierSelectedAudio(MissionModifierSO selectedMissionModifier)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.GeneralMissionModifierSelected,
            out EventInstance eventInstance);
        
        eventInstance.getPitch(out float pitch);
        eventInstance.setPitch(pitch + (SelectionManager.Instance.GetCurrentMissionModifiers().Count*_missionModifierSFXPitchIncrease));
    }

    private void PlayMissionModifierDeselectedAudio(MissionModifierSO selectedMissionModifier)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.UserInterfaceAudio.SelectionSceneUserInterfaceAudio.GeneralMissionModifierDeselected,
            out EventInstance eventInstance);
        
        eventInstance.getPitch(out float pitch);
        eventInstance.setPitch(pitch + (SelectionManager.Instance.GetCurrentMissionModifiers().Count*_missionModifierSFXPitchIncrease));
    }
    #endregion

    private void SubscribeToEvents()
    {
        SelectionManager.Instance.GetBossSelectionEvent().AddListener(NewBossAddedSelection);
        SelectionManager.Instance.GetBossDeselectionEvent().AddListener(BossRemovedSelection);

        SelectionManager.Instance.GetBossSwapEvent().AddListener(SwapBoss);

        SelectionManager.Instance.GetBossHoveredOverEvent().AddListener(BossHoveredOver);
        SelectionManager.Instance.GetBossNotHoveredOverEvent().AddListener(BossNotHoveredOver);
        
        SelectionManager.Instance.GetBossInformationLockedEvent().AddListener(InformationLockBoss);
        
        SelectionManager.Instance.GetMissionModifierSelectionEvent().AddListener(MissionModifierSelected);
        SelectionManager.Instance.GetMissionModifierDeselectionEvent().AddListener(MissionModifierDeselected);
        
        SelectionManager.Instance.GetMissionModifierSwapEvent().AddListener(MissionModifierSwap);
        
        SelectionManager.Instance.GetMissionModifierHoveredOverEvent().AddListener(MissionModifierHoveredOver);
        SelectionManager.Instance.GetMissionModifierNotHoveredOverEvent().AddListener(MissionModifierNotHoveredOver);

        SelectionManager.Instance.GetHeroSelectionEvent().AddListener(NewHeroAddedSelection);
        SelectionManager.Instance.GetHeroDeselectionEvent().AddListener(HeroRemovedSelection);

        SelectionManager.Instance.GetHeroSwapEvent().AddListener(SwapHero);
        

        SelectionManager.Instance.GetHeroHoveredOverEvent().AddListener(HeroHoveredOver);
        SelectionManager.Instance.GetHeroNotHoveredOverEvent().AddListener(HeroNotHoveredOver);
        
        SelectionManager.Instance.GetHeroInformationLockedEvent().AddListener(InformationLockHero);

        SelectionManager.Instance.GetDifficultySelectionEvent().AddListener(DifficultySelected);
        
        SelectionManager.Instance.GetInformationUnlockedEvent().AddListener(UnlockCharacterInformation);
    }
    
    #region Getters

    public int GetCurrentBossAbilityID() => _currentBossAbilityID;
    public int GetCurrentHeroAbilityID() => _currentHeroAbilityID;
    public int GetCurrentMissionModifierID() => _currentMissionModifierID;
    
    public BossSO GetBossUIToDisplay() => _bossUIToDisplay;
    public HeroSO GetHeroUIToDisplay() => _heroUIToDisplay;
    public MissionModifierSO GetMissionModifierUIToDisplay() => _missionModifierUIToDisplay;
    
    public Camera GetBossCamera() => _bossCamera;
    public Camera GetHeroCamera() => _heroCamera;

    #endregion
}
