using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionController : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private BossPillar _bossPillar;

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


    private SelectionManager _selectionManager;

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        BossSideStart();
        CenterStart();
        HeroSideStart();

        _selectionManager = UniversalManagers.Instance.GetSelectionManager();
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
    #endregion

    #region Center

    private void CenterStart()
    {
        FightButtonStartingInteractablity();
    }

    #region Center - Boss

    private void BossHoveredOver(BossSO bossSO)
    {
        if (bossSO == _lastBossHoveredOver ||_selectionManager.AtMaxBossSelected()) return;

        _lastHeroHoveredOver = null;
        _lastBossHoveredOver = bossSO;
        _bossUIToDisplay = bossSO;

        NewBossHoveredOver(bossSO);
    }

    private void NewBossHoveredOver(BossSO bossSO)
    {
        //Show boss description and hide hero description
        _bossDescription.SetActive(true);
        HideFullHeroDescription();

        _bossNameText.text = bossSO.GetBossName();
        _bossNameBorder.text = bossSO.GetBossName();

        HideBossAbilityDescription();

        DisplayAbilityIconsForBoss(bossSO);

        UpdateHeroButtonDifficultyBeaten();

        _bossPillar.ShowBossOnPillar(bossSO, true);
    }

    private void BossNotHoveredOver(BossSO bossSO)
    {
        if (_selectionManager.AtMaxBossSelected()) return;

        _bossPillar.AnimateOutBossOnPillar();

        _lastBossHoveredOver = null;
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
        if (_currentBossAbilityID == -1) return;


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
    private void FightButtonStartingInteractablity()
    {
        _fightButton.interactable = !_requiresMaxCharacters;

    }

    private void CheckMaxCharactersSelected()
    {

        if (_selectionManager.AtMaxBossSelected() && _selectionManager.AtMaxHeroesSelected() && _requiresMaxCharacters)
        {
            MaxCharactersSelected();
        }
    }

    private void MaxCharactersSelected()
    {
        _maxCharactersSelected = true;
        _fightButton.interactable = true ;
    }
    
    private void CheckMaxCharactersNoLongerSelected()
    {
        if (_maxCharactersSelected)
            NoLongerMaxHeroesSelected();
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
        UniversalManagers.Instance.GetSceneLoadManager().LoadCurrentlySelectedLevelSO();
    }
    #endregion

    #region Center - Hero
    private void HeroHoveredOver(HeroSO heroSO)
    {
        //Stops if it is already at max heroes
        if (_selectionManager.AtMaxHeroesSelected()) return;

        //Stop if it is the same hero as the previous
        if (heroSO == _lastHeroHoveredOver) return;
        //Stop if the hero is selected already
        if (_selectionManager.GetAllSelectedHeroes().Contains(heroSO)) return;

        //bool a= (UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes().Contains(heroSO))

        _lastBossHoveredOver = null;
        _lastHeroHoveredOver = heroSO;
        _heroUIToDisplay = heroSO;
        NewHeroHoveredOver(heroSO);
    }

    private void NewHeroHoveredOver(HeroSO heroSO)
    {
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

        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum ].ShowHeroOnPillar(heroSO, true);
    }

    private void HeroNotHoveredOver(HeroSO heroSO)
    {
        if (_selectionManager.AtMaxHeroesSelected()) return;
        if (_selectionManager.GetAllSelectedHeroes().Contains(heroSO)) return;

        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum].AnimateOutHeroOnPillar();

        _lastHeroHoveredOver = null;
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
            statCounter.StopShowNodeProcess();
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
                UpdateHeroAbilityTypeText(HeroAbilityType.Basic.ToString());
                UpdateHeroAbilityDescriptionText(_heroUIToDisplay.GetHeroBasicAbilityDescription());
                return;
            case (1):
                UpdateHeroAbilityNameText(_heroUIToDisplay.GetHeroManualAbilityName());
                UpdateHeroAbilityTypeText(HeroAbilityType.Manual.ToString());
                UpdateHeroAbilityDescriptionText(_heroUIToDisplay.GetHeroManualAbilityDescription());
                return;
            case (2):
                UpdateHeroAbilityNameText(_heroUIToDisplay.GetHeroPassiveAbilityName());
                UpdateHeroAbilityTypeText(HeroAbilityType.Passive.ToString());
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
        MoveHeroPillar(0, true);
    }

    private void NewHeroAdded(HeroSO heroSO)
    {
        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum - 1].ShowHeroOnPillar(heroSO,false);

        if (heroPillarNum < UniversalManagers.Instance.GetSelectionManager().GetMaxHeroesCount())
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
        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount() + 1;

        if (heroPillarNum > 0 &&
            heroPillarNum < UniversalManagers.Instance.GetSelectionManager().GetMaxHeroesCount())
        {
            MoveHeroPillar(heroPillarNum, false);
        }

        FindHeroPillarWithHero(heroSO).RemoveHeroOnPillar();
        RearrangeHeroesOnPillars();

        CheckMaxCharactersNoLongerSelected();
    }

    private HeroPillar FindHeroPillarWithHero(HeroSO searchHero)
    {
        foreach (HeroPillar heroPillar in _heroPillars)
        {
            if (heroPillar.GetStoredHero() == searchHero)
                return heroPillar;
        }
        return null;
    }

    private void RearrangeHeroesOnPillars()
    {
        MoveNextHeroBackToCurrentPillar(0);
    }

    private void MoveNextHeroBackToCurrentPillar(int pillarNum)
    {
        if (pillarNum + 1 >= UniversalManagers.Instance.GetSelectionManager().GetMaxHeroesCount()) return;

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

    public void BackToMainMenu()
    {
        UniversalManagers.Instance.GetSceneLoadManager().LoadMainMenuScene();
    }

    #endregion

    private void SubscribeToEvents()
    {
        UniversalManagers.Instance.GetSelectionManager().GetBossSelectionEvent().AddListener(NewBossAdded);
        UniversalManagers.Instance.GetSelectionManager().GetBossDeselectionEvent().AddListener(BossRemoved);

        UniversalManagers.Instance.GetSelectionManager().GetBossHoveredOverEvent().AddListener(BossHoveredOver);
        UniversalManagers.Instance.GetSelectionManager().GetBossNotHoveredOverEvent().AddListener(BossNotHoveredOver);

        UniversalManagers.Instance.GetSelectionManager().GetHeroSelectionEvent().AddListener(NewHeroAdded);
        UniversalManagers.Instance.GetSelectionManager().GetHeroDeselectionEvent().AddListener(HeroRemoved);

        UniversalManagers.Instance.GetSelectionManager().GetHeroHoveredOverEvent().AddListener(HeroHoveredOver);
        UniversalManagers.Instance.GetSelectionManager().GetHeroNotHoveredOverEvent().AddListener(HeroNotHoveredOver);
    }
}
