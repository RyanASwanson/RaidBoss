using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionController : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private BossPillar _bossPillar;

    [SerializeField] private List<SelectBossLevelButton> _bossLevelSelectionButtons;

    [Space]
    [Header("Center")]
    [Header("Center-Boss")]
    [SerializeField] private GameObject _bossDescription;

    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private Text _bossNameBorder;

    [Space]
    [SerializeField] private List<Image> _bossAbilityImageIcons;

    [Space]
    [SerializeField] private Animator _bossAbilityDescriptionAnimator;

    [Space]
    [SerializeField] private Text _bossAbilityBackgroundNameText;
    [SerializeField] private TMP_Text _bossAbilityNameText;

    [SerializeField] private Text _bossAbilityBackgroundTypeText;
    [SerializeField] private TMP_Text _bossAbilityTypeText;

    [SerializeField] private Text _bossAbilityBackgroundDescriptionText;
    [SerializeField] private TMP_Text _bossAbilityDescriptionText;


    private int _currentBossAbilityID = -1;

    [Header("Center-General")]
    [SerializeField] private bool _requiresMaxCharacters;

    [SerializeField] private Button _fightButton;

    private bool _maxCharactersSelected = false;

    public static bool IsSelectionInformationLocked = false;
    public static CharacterSO SelectionLockedCharacter;

    [Space]
    [Header("Center-Hero")]
    [SerializeField] private GameObject _heroDescription;

    [SerializeField] private TMP_Text _heroNameText;
    [SerializeField] private Text _heroNameBorder;

    [Space]
    [SerializeField] private List<StatCounter> _statCounters;

    [Space]
    [SerializeField] private Image _rangeIcon;
    [SerializeField] private List<Sprite> _rangeIconSprites;

    [Space]
    [SerializeField] private Image _difficultyIcon;
    [SerializeField] private List<Sprite> _difficultyIconSprites;
    

    [Space]
    [SerializeField] private Image _heroBasicIcon;
    [SerializeField] private Image _heroManualIcon;
    [SerializeField] private Image _heroPassiveIcon;

    private BossSO _lastBossHoveredOver;
    private BossSO _bossUIToDisplay;
    private HeroSO _lastHeroHoveredOver;
    private HeroSO _heroUIToDisplay;

    [Space]
    [SerializeField] private Animator _heroAbilityDescriptionAnimator;

    private const string SHOW_ABILITY_DESCRIPTION_ANIM_BOOL = "ShowDescription";
    private const string SWAP_ABILITY_DESCRIPTION_ANIM_TRIGGER = "SwapDescription";

    [Space]
    [SerializeField] private Text _heroAbilityBackgroundNameText;
    [SerializeField] private TMP_Text _heroAbilityNameText;

    [SerializeField] private Text _heroAbilityBackgroundTypeText;
    [SerializeField] private TMP_Text _heroAbilityTypeText;

    [SerializeField] private Text _heroAbilityBackgroundDescriptionText;
    [SerializeField] private TMP_Text _heroAbilityDescriptionText;

    private int _currentHeroAbilityID = -1;

    [Space]
    [Header("Hero")]
    [SerializeField] private List<HeroPillar> _heroPillars = new List<HeroPillar>();

    [SerializeField] private List<SelectHeroButton> _heroSelectionButtons = new List<SelectHeroButton>();

    private int _previousMaxHeroes;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        BossSideStart();
        CenterStart();
        HeroSideStart();
        
        PlaySelectionSceneMusic();
    }

    #region Boss Side
    private void BossSideStart()
    {

    }

    private void NewBossAdded(BossSO bossSO)
    {
        _bossPillar.ShowBossOnPillar(bossSO, false);

        CheckMaxCharactersSelected();

    }

    private void BossRemoved(BossSO bossSO)
    {
        _bossPillar.RemoveBossOnPillar();

        CheckMaxCharactersNoLongerSelected();
    }

    private void SwapBoss(BossSO previousBoss)
    {
        //Removes the previous boss
        foreach (SelectBossLevelButton bossButton in _bossLevelSelectionButtons)
        {
            if(bossButton.GetAssociatedBoss() == previousBoss)
                bossButton.SelectBossLevelLeftClicked();

        }
    }
    #endregion

    #region Center

    private void CenterStart()
    {
        FightButtonStartingInteractability();
    }

    #region Center - Boss

    private void BossHoveredOver(BossSO bossSO)
    {
        //if (bossSO == _lastBossHoveredOver ||_selectionManager.AtMaxBossSelected()) return;
        if (bossSO == _lastBossHoveredOver)
        {
            return;
        }

        _lastHeroHoveredOver = null;
        _lastBossHoveredOver = bossSO;

        NewBossHoveredOver(bossSO);
    }

    private void NewBossHoveredOver(BossSO bossSO)
    {
        if (!IsSelectionInformationLocked)
        {
            DisplayBossInformation(bossSO);
        }

        UpdateHeroButtonDifficultyBeaten();
        
        _bossPillar.ShowBossOnPillar(bossSO, true);
    }

    /// <summary>
    /// Called when a boss is no longer hovered over
    /// </summary>
    /// <param name="bossSO"> The scriptable object of the boss no longer hovered over</param>
    private void BossNotHoveredOver(BossSO bossSO)
    {
        //Check if the boss is not the selected boss
        if (bossSO != SelectionManager.Instance.GetSelectedBoss())
        {
            //Start the animation of removing the boss
            _bossPillar.AnimateOutBossOnPillar();
            //Hover over the selected boss
            if (SelectionManager.Instance.AtMaxBossSelected())
                BossHoveredOver(SelectionManager.Instance.GetSelectedBoss());
            {
                return;
            }
        }

        //_lastBossHoveredOver = null;
    }

    public void DisplayBossInformation(BossSO bossSO)
    {
        _bossUIToDisplay = bossSO;
        
        //Show boss description and hide hero description
        _bossDescription.SetActive(true);
        HideFullHeroDescription();

        _bossNameText.text = bossSO.GetBossSelectionScreenName();
        _bossNameBorder.text = bossSO.GetBossSelectionScreenName();

        HideBossAbilityDescription();

        DisplayAbilityIconsForBoss(bossSO);
    }

    private void InformationLockBoss(BossSO bossSO)
    {
        IsSelectionInformationLocked = true;
        SelectionLockedCharacter = bossSO;
        
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
        _bossAbilityDescriptionAnimator.SetBool(SHOW_ABILITY_DESCRIPTION_ANIM_BOOL, true);
    }

    private void UpdateBossAbilityNameText(string newText)
    {
        _bossAbilityBackgroundNameText.text = newText;
        _bossAbilityNameText.text = newText;
    }

    private void UpdateBossAbilityTypeText(string newText)
    {
        _bossAbilityBackgroundTypeText.text = newText;
        _bossAbilityTypeText.text = newText;
    }

    private void UpdateBossAbilityDescriptionText(string newText)
    {
        _bossAbilityBackgroundDescriptionText.text = newText;
        _bossAbilityDescriptionText.text = newText;
    }

    

    public void BossAbilityDescriptionChanged()
    {
        if (_currentBossAbilityID == -1)
        {
            return;
        }
        
        UpdateBossAbilityNameText(_bossUIToDisplay.GetBossAbilityInformation()
            [_currentBossAbilityID]._abilityName);

        UpdateBossAbilityTypeText(_bossUIToDisplay.GetBossAbilityInformation()
            [_currentBossAbilityID]._abilityType.ToString());

        UpdateBossAbilityDescriptionText(_bossUIToDisplay.GetBossAbilityInformation()
            [_currentBossAbilityID]._abilityDescription);
    }

    /// <summary>
    /// Changes what the currently displayed boss description is.
    /// Animates the boss description to change the visuals
    /// </summary>
    /// <param name="abilityID"></param>
    private void SwapBossAbilityDescription(int abilityID)
    {
        _currentBossAbilityID = abilityID;
        _bossAbilityDescriptionAnimator.SetTrigger(SWAP_ABILITY_DESCRIPTION_ANIM_TRIGGER);
    }

    /// <summary>
    /// Animates the boss description to hide it from view.
    /// Removes the currently displayed boss ability id
    /// </summary>
    private void HideBossAbilityDescription()
    {
        _bossAbilityDescriptionAnimator.SetBool(SHOW_ABILITY_DESCRIPTION_ANIM_BOOL, false);
        _currentBossAbilityID = -1;
    }

    private void HideFullBossDescription()
    {
        _bossDescription.SetActive(false);
    }

    #endregion

    #region Center - General
    private void FightButtonStartingInteractability()
    {
        _fightButton.interactable = !_requiresMaxCharacters;
    }

    private void UnlockCharacterInformation()
    {
        IsSelectionInformationLocked = false;
        SelectionLockedCharacter = null;
    }

    /// <summary>
    /// Checks if the player has selected all the heroes and the boss
    /// </summary>
    private void CheckMaxCharactersSelected()
    {
        if (SelectionManager.Instance.AtMaxBossSelected() && SelectionManager.Instance.AtMaxHeroesSelected() && _requiresMaxCharacters)
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
        _fightButton.interactable = true ;
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
        _fightButton.interactable = false;
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
        //Stop if it is the same hero as the previous
        if (heroSO == _lastHeroHoveredOver)
        {
            return;
        }
        //Stop if the hero is selected already
        if (SelectionManager.Instance.GetAllSelectedHeroes().Contains(heroSO) &&
            SelectionManager.Instance.GetHeroAtLastPostion() != heroSO)
        {
            return;
        }

        _lastBossHoveredOver = null;
        _lastHeroHoveredOver = heroSO;
        
        NewHeroHoveredOver(heroSO);
    }

    private void NewHeroHoveredOver(HeroSO heroSO)
    {
        if (!IsSelectionInformationLocked)
        {
            DisplayHeroInformation(heroSO);
        }

        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount();

        if (SelectionManager.Instance.AtMaxHeroesSelected())
        {
            heroPillarNum--;
        }

        _heroPillars[heroPillarNum ].ShowHeroOnPillar(heroSO, true);
    }

    private void HeroNotHoveredOver(HeroSO heroSO)
    {

        /*if (_selectionManager.AtMaxHeroesSelected()) 
        {
            print("first");
            if(_selectionManager.GetHeroAtLastPostion() != heroSO)
            {
                print("RemoveLast");
                int hpn = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

                _heroPillars[hpn-1].AnimateOutHeroOnPillar();

                _lastHeroHoveredOver = null;

                NewHeroHoveredOver(_selectionManager.GetHeroAtLastPostion());
            }
            return;
        }


        if (_selectionManager.GetAllSelectedHeroes().Contains(heroSO)) return;

        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum].AnimateOutHeroOnPillar();

        _lastHeroHoveredOver = null;*/



        if (SelectionManager.Instance.GetAllSelectedHeroes().Contains(heroSO))
        {
            return;
        }

        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount();
        
        if (SelectionManager.Instance.AtMaxHeroesSelected() &&
            SelectionManager.Instance.GetHeroAtLastPostion() != heroSO)
        {
            heroPillarNum--;
        }

        _heroPillars[heroPillarNum].AnimateOutHeroOnPillar();

        _lastHeroHoveredOver = null;

        if (SelectionManager.Instance.AtMaxHeroesSelected() &&
            SelectionManager.Instance.GetHeroAtLastPostion() != heroSO)
        {
            NewHeroHoveredOver(SelectionManager.Instance.GetHeroAtLastPostion());
        }
    }

    private void DisplayHeroInformation(HeroSO heroSO)
    {
        _heroUIToDisplay = heroSO;
        
        //Show hero description and hide boss description
        HideFullBossDescription();
        _heroDescription.SetActive(true);

        //Updates the text to display the heroes name
        _heroNameText.text = heroSO.GetHeroName();
        _heroNameBorder.text = heroSO.GetHeroName();

        HideHeroAbilityDescription();

        //Displays all stats associated for the hero on the counters
        DisplayStatsForHero(heroSO);

        DisplayHeroRangeAndDifficulty(heroSO);

        DisplayAbilityIconsForHero(heroSO);
    }

    private void InformationLockHero(HeroSO heroSO)
    {
        IsSelectionInformationLocked = true;
        SelectionLockedCharacter = heroSO;
        
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
        _difficultyIcon.sprite = _difficultyIconSprites[(int)heroSO.GetHeroDifficulty()];
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
        _heroAbilityDescriptionAnimator.SetBool(SHOW_ABILITY_DESCRIPTION_ANIM_BOOL, true);
    }

    private void UpdateHeroAbilityNameText(string newText)
    {
        _heroAbilityBackgroundNameText.text = newText;
        _heroAbilityNameText.text = newText;
    }

    private void UpdateHeroAbilityTypeText(string newText)
    {
        _heroAbilityBackgroundTypeText.text = newText;
        _heroAbilityTypeText.text = newText;
    }

    private void UpdateHeroAbilityDescriptionText(string newText)
    {
        _heroAbilityBackgroundDescriptionText.text = newText;
        _heroAbilityDescriptionText.text = newText;
    }

    
    /// <summary>
    /// Changes which ability is being displayed
    /// Can update the ability name, type, and description text
    /// Presents the hero ability based on which hero ability ID is currently active
    /// </summary>
    public void HeroAbilityDescriptionChanged()
    {
        switch(_currentHeroAbilityID)
        {
            case (0):
                UpdateHeroAbilityNameText(_heroUIToDisplay.GetHeroBasicAbilityName());
                UpdateHeroAbilityTypeText(EHeroAbilityType.Basic.ToString());
                UpdateHeroAbilityDescriptionText(_heroUIToDisplay.GetHeroBasicAbilityDescription());
                return;
            case (1):
                UpdateHeroAbilityNameText(_heroUIToDisplay.GetHeroManualAbilityName());
                UpdateHeroAbilityTypeText(EHeroAbilityType.Manual.ToString());
                UpdateHeroAbilityDescriptionText(_heroUIToDisplay.GetHeroManualAbilityDescription());
                return;
            case (2):
                UpdateHeroAbilityNameText(_heroUIToDisplay.GetHeroPassiveAbilityName());
                UpdateHeroAbilityTypeText(EHeroAbilityType.Passive.ToString());
                UpdateHeroAbilityDescriptionText(_heroUIToDisplay.GetHeroPassiveAbilityDescription());
                return;
        }
    }

    private void SwapHeroAbilityDescription(int abilityID)
    {
        _currentHeroAbilityID = abilityID;
        _heroAbilityDescriptionAnimator.SetTrigger(SWAP_ABILITY_DESCRIPTION_ANIM_TRIGGER);
    }

    private void HideHeroAbilityDescription()
    {
        _heroAbilityDescriptionAnimator.SetBool(SHOW_ABILITY_DESCRIPTION_ANIM_BOOL, false);
        _currentHeroAbilityID = -1;
    }

    private void HideFullHeroDescription()
    {
        //StopStatDisplayProcess();
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
    }

    private void HeroLimitReduced()
    {
        List<HeroSO> heroesToRemove = new();

        for (int i = SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty(); i != _previousMaxHeroes; i++)
        {
            if (SelectionManager.Instance.GetAllSelectedHeroes().Count > i)
            {
                heroesToRemove.Add(SelectionManager.Instance.GetAllSelectedHeroes()[i]);
                //_heroPillars[i].ShowPreviewPillar(false);
            }
            //_heroPillars[i].MovePillar(false);
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

    private void NewHeroAdded(HeroSO heroSO)
    {
        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount();

        _heroPillars[heroPillarNum - 1].ShowHeroOnPillar(heroSO,false);

        if (heroPillarNum < SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty())
        {
            MoveHeroPillar(heroPillarNum, true);
        }

        CheckMaxCharactersSelected();
    }


    /// <summary>
    /// Removes a hero from the pillar and makes a pillar move down
    /// </summary>
    /// <param name="heroSO"></param>
    private void HeroRemoved(HeroSO heroSO)
    {
        int heroPillarNum = SelectionManager.Instance.GetSelectedHeroesCount() + 1;

        if (heroPillarNum > 0 &&
            heroPillarNum < SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty())
        {
            MoveHeroPillar(heroPillarNum, false);
        }

        //Remove the hero on the pillar that had a hero removed
        _heroPillars[SelectionManager.Instance.GetIndexOfLastHeroRemoved()].RemoveHeroOnPillar();
        RearrangeHeroesOnPillars();

        CheckMaxCharactersNoLongerSelected();
    }


    private void SwapHero(HeroSO hero)
    {
        SelectHeroButton heroButton = GetHeroButtonFromSO(hero);

        heroButton.SelectHeroButtonLeftClicked();
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
        if (pillarNum + 1 >= SelectionManager.Instance.GetMaxHeroesCountWithCurrentDifficulty()) return;

        if(!_heroPillars[pillarNum].HasStoredHero() && _heroPillars[pillarNum+1].HasStoredHero())
        {
            _heroPillars[pillarNum].ShowHeroOnPillar(_heroPillars[pillarNum + 1].GetStoredHero(),true);
            _heroPillars[pillarNum + 1].RemoveHeroOnPillar();
        }
        MoveNextHeroBackToCurrentPillar(pillarNum + 1);
        
    }

    private void MoveHeroPillar(int pillarNum, bool moveUp)
    {
        _heroPillars[pillarNum].MovePillar(moveUp);
    }


    private void UpdateHeroButtonDifficultyBeaten()
    {
        foreach(SelectHeroButton selectHeroButton in _heroSelectionButtons)
        {
            selectHeroButton.SetBestDifficultyBeatenIcon(_lastBossHoveredOver);
        }
    }
    #endregion

    #region General

    private void PlaySelectionSceneMusic()
    {
        AudioManager.Instance.PlayMusic(AudioManager.SELECTION_SCENE_MUSIC_ID, false);
    }
    
    public void BackToMainMenu()
    {
        SceneLoadManager.Instance.LoadMainMenuScene();
    }

    #endregion

    private void SubscribeToEvents()
    {
        SelectionManager.Instance.GetBossSelectionEvent().AddListener(NewBossAdded);
        SelectionManager.Instance.GetBossDeselectionEvent().AddListener(BossRemoved);

        SelectionManager.Instance.GetBossSwapEvent().AddListener(SwapBoss);

        SelectionManager.Instance.GetBossHoveredOverEvent().AddListener(BossHoveredOver);
        SelectionManager.Instance.GetBossNotHoveredOverEvent().AddListener(BossNotHoveredOver);
        
        SelectionManager.Instance.GetBossInformationLockedEvent().AddListener(InformationLockBoss);

        SelectionManager.Instance.GetHeroSelectionEvent().AddListener(NewHeroAdded);
        SelectionManager.Instance.GetHeroDeselectionEvent().AddListener(HeroRemoved);

        SelectionManager.Instance.GetHeroSwapEvent().AddListener(SwapHero);

        SelectionManager.Instance.GetHeroHoveredOverEvent().AddListener(HeroHoveredOver);
        SelectionManager.Instance.GetHeroNotHoveredOverEvent().AddListener(HeroNotHoveredOver);
        
        SelectionManager.Instance.GetHeroInformationLockedEvent().AddListener(InformationLockHero);

        SelectionManager.Instance.GetDifficultySelectionEvent().AddListener(HeroLimitChanged);
        
        SelectionManager.Instance.GetInformationUnlockedEvent().AddListener(UnlockCharacterInformation);
    }
}
