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
    [SerializeField] private Text _bossAbilityBackgroundDescriptionText;
    [SerializeField] private TMP_Text _bossAbilityDescriptionText;

    [SerializeField] private Text _bossAbilityBackgroundNameText;
    [SerializeField] private TMP_Text _bossAbilityNameText;
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
    [SerializeField] private Image _heroBasicIcon;
    [SerializeField] private Image _heroManualIcon;
    [SerializeField] private Image _heroPassiveIcon;

    private BossSO _lastBossHoveredOver;
    private HeroSO _lastHeroHoveredOver;

    [Space]
    [SerializeField] private Animator _heroAbilityDescriptionAnimator;
    private const string _showAbilityDescriptionBool = "ShowDescription";
    private const string _swapAbilityDescriptionTrigger = "SwapDescription";

    [Space]
    [SerializeField] private Text _heroAbilityBackgroundDescriptionText;
    [SerializeField] private TMP_Text _heroAbilityDescriptionText;

    [SerializeField] private Text _heroAbilityBackgroundNameText;
    [SerializeField] private TMP_Text _heroAbilityNameText;
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

        _selectionManager = FindObjectOfType<SelectionManager>();
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
        _bossAbilityDescriptionAnimator.SetBool(_showAbilityDescriptionBool, true);
    }

    private void UpdateBossAbilityDescriptionText(string newText)
    {
        _bossAbilityBackgroundDescriptionText.text = newText;
        _bossAbilityDescriptionText.text = newText;
    }

    private void UpdateBossAbilityNameText(string newText)
    {
        _bossAbilityBackgroundNameText.text = newText;
        _bossAbilityNameText.text = newText;
    }

    public void BossAbilityDescriptionChanged()
    {
        UpdateBossAbilityDescriptionText(_lastBossHoveredOver.GetBossAbilityInformation()
            [_currentBossAbilityID]._abilityDescription);

        UpdateBossAbilityNameText(_lastBossHoveredOver.GetBossAbilityInformation()
            [_currentBossAbilityID]._abilityName);

    }

    /// <summary>
    /// Changes what the currently displayed boss description is.
    /// Animates the boss description to change the visuals
    /// </summary>
    /// <param name="abilityID"></param>
    private void SwapBossAbilityDescription(int abilityID)
    {
        _currentBossAbilityID = abilityID;
        _bossAbilityDescriptionAnimator.SetTrigger(_swapAbilityDescriptionTrigger);
    }

    /// <summary>
    /// Animates the boss description to hide it from view.
    /// Removes the currently displayed boss ability id
    /// </summary>
    private void HideBossAbilityDescription()
    {
        _bossAbilityDescriptionAnimator.SetBool(_showAbilityDescriptionBool, false);
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

        DisplayAbilityIconsForHero(heroSO);

        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum ].ShowHeroOnPillar(heroSO, true);
    }

    private void DisplayStatsForHero(HeroSO heroSO)
    {
        _statCounters[0].ShowStatNodes(heroSO.GetSurvivalStat());
        _statCounters[1].ShowStatNodes(heroSO.GetDamageStat());
        _statCounters[2].ShowStatNodes(heroSO.GetStaggerStat());
        _statCounters[3].ShowStatNodes(heroSO.GetSpeedStat());
        _statCounters[4].ShowStatNodes(heroSO.GetUtilityStat());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Y))
        {
            print(_lastHeroHoveredOver.GetSurvivalStat());
            print(_lastHeroHoveredOver.GetDamageStat());
            print(_lastHeroHoveredOver.GetStaggerStat());
            print(_lastHeroHoveredOver.GetSpeedStat());
            print(_lastHeroHoveredOver.GetUtilityStat());

            DisplayStatsForHero(_lastHeroHoveredOver);
        }
    }

    private void StopStatDisplayProcess()
    {
        foreach (StatCounter statCounter in _statCounters)
            statCounter.StopShowNodeProcess();
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


    public void ShowHeroAbilityDescription(int abilityID)
    {
        _currentHeroAbilityID = abilityID;
        _heroAbilityDescriptionAnimator.SetBool(_showAbilityDescriptionBool, true);
    }


    private void UpdateHeroAbilityDescriptionText(string newText)
    {
        _heroAbilityBackgroundDescriptionText.text = newText;
        _heroAbilityDescriptionText.text = newText;
    }

    private void UpdateHeroAbilityNameText(string newText)
    {
        _heroAbilityBackgroundNameText.text = newText;
        _heroAbilityNameText.text = newText;
    }

    public void HeroAbilityDescriptionChanged()
    {
        switch(_currentHeroAbilityID)
        {
            case (0):
                UpdateHeroAbilityDescriptionText(_lastHeroHoveredOver.GetHeroBasicAbilityDescription());
                UpdateHeroAbilityNameText(_lastHeroHoveredOver.GetHeroBasicAbilityName());
                return;
            case (1):
                UpdateHeroAbilityDescriptionText(_lastHeroHoveredOver.GetHeroManualAbilityDescription());
                UpdateHeroAbilityNameText(_lastHeroHoveredOver.GetHeroManualAbilityName());
                return;
            case (2):
                UpdateHeroAbilityDescriptionText(_lastHeroHoveredOver.GetHeroPassiveAbilityDescription());
                UpdateHeroAbilityNameText(_lastHeroHoveredOver.GetHeroPassiveAbilityName());
                return;

        }
            
    }

    private void SwapHeroAbilityDescription(int abilityID)
    {
        _currentHeroAbilityID = abilityID;
        _heroAbilityDescriptionAnimator.SetTrigger(_swapAbilityDescriptionTrigger);
    }

    private void HideHeroAbilityDescription()
    {
        _heroAbilityDescriptionAnimator.SetBool(_showAbilityDescriptionBool, false);
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

        UniversalManagers.Instance.GetSelectionManager().GetHeroSelectionEvent().AddListener(NewHeroAdded);
        UniversalManagers.Instance.GetSelectionManager().GetHeroDeselectionEvent().AddListener(HeroRemoved);

        UniversalManagers.Instance.GetSelectionManager().GetHeroHoveredOverEvent().AddListener(HeroHoveredOver);
    }
}
