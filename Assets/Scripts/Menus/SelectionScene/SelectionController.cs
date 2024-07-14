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
    [SerializeField] private GameObject _bossDescription;

    [SerializeField] private TMP_Text _bossNameText;
    [SerializeField] private Text _bossNameBorder;


    [Space]
    [SerializeField] private GameObject _heroDescription;

    [SerializeField] private TMP_Text _heroNameText;
    [SerializeField] private Text _heroNameBorder;

    [SerializeField] private List<Image> _survivalCounters;
    [SerializeField] private List<Image> _damageCounters;
    [SerializeField] private List<Image> _staggerCounters;
    [SerializeField] private List<Image> _speedCounters;
    [SerializeField] private List<Image> _utilityCounters;

    [SerializeField] private Image _heroBasicIcon;
    [SerializeField] private Image _heroManualIcon;
    [SerializeField] private Image _heroPassiveIcon;

    private BossSO _lastBossHoveredOver;
    private HeroSO _lastHeroHoveredOver;

    [Space]
    [SerializeField] private Animator _heroAbilityDescriptionAnimator;
    private const string _showAbilityDescriptionBool = "ShowDescription";
    private const string _swapAbilityDescriptionTrigger = "SwapDescription";

    [SerializeField] private Text _heroAbilityBackgroundDescriptionText;
    [SerializeField] private TMP_Text _heroAbilityDescriptionText;
    private float _currentAbilityID = -1;

    [Space]
    [SerializeField] private Animator _bossAbilityDescriptionAnimator;

    [Space]
    [Header("Hero")]
    [SerializeField] private List<HeroPillar> _heroPillars = new List<HeroPillar>();

    [SerializeField] private List<SelectHeroButton> _heroSelectionButtons = new List<SelectHeroButton>();

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        BossSideStart();
        CenterStart();
        HeroSideStart();
    }

    #region Boss Side
    private void BossSideStart()
    {

    }

    private void NewBossAdded(BossSO bossSO)
    {
        _bossPillar.ShowBossOnPillar(bossSO, false);
    }

    private void BossRemoved(BossSO bossSO)
    {
        _bossPillar.RemoveBossOnPillar();
    }
    #endregion

    #region Center

    private void CenterStart()
    {

    }

    private void BossHoveredOver(BossSO bossSO)
    {
        if (bossSO == _lastBossHoveredOver) return;

        _lastHeroHoveredOver = null;
        _lastBossHoveredOver = bossSO;

        NewBossHoveredOver(bossSO);
    }

    private void NewBossHoveredOver(BossSO bossSO)
    {
        //Show boss description and hide hero description
        _bossDescription.SetActive(true);
        _heroDescription.SetActive(false);

        _bossNameText.text = bossSO.GetBossName();
        _bossNameBorder.text = bossSO.GetBossName();

        UpdateHeroButtonDifficultyBeaten();

        _bossPillar.ShowBossOnPillar(bossSO, true);
    }

    private void HeroHoveredOver(HeroSO heroSO)
    {
        //Stop if it is the same hero as the previous
        if (heroSO == _lastHeroHoveredOver) return;
        //Stop if the hero is selected already
        if (UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes().Contains(heroSO)) return;
        //bool a= (UniversalManagers.Instance.GetSelectionManager().GetAllSelectedHeroes().Contains(heroSO))

        _lastBossHoveredOver = null;
        _lastHeroHoveredOver = heroSO;
        NewHeroHoveredOver(heroSO);
    }

    private void NewHeroHoveredOver(HeroSO heroSO)
    {
        
        //Show hero description and hide boss description
        _bossDescription.SetActive(false);
        _heroDescription.SetActive(true);

        //Updates the text to display the heroes name
        _heroNameText.text = heroSO.GetHeroName();
        _heroNameBorder.text = heroSO.GetHeroName();

        HideAbilityDescription();

        //Displays all stats associated for the hero on the counters
        DisplayStatsForHero(heroSO);

        DisplayAbilityIconsForHero(heroSO);

        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum ].ShowHeroOnPillar(heroSO, true);
    }

    private void DisplayStatsForHero(HeroSO heroSO)
    {
        ShowStatCounter(_survivalCounters, heroSO.GetSurvivalStat());
        ShowStatCounter(_damageCounters, heroSO.GetDamageStat());
        ShowStatCounter(_staggerCounters, heroSO.GetStaggerStat());
        ShowStatCounter(_speedCounters, heroSO.GetSpeedStat());
        ShowStatCounter(_utilityCounters, heroSO.GetUtilityStat());
    }

    private void DisplayAbilityIconsForHero(HeroSO heroSO)
    {
        _heroBasicIcon.sprite = heroSO.GetHeroBasicAbilityIcon();
        _heroManualIcon.sprite = heroSO.GetHeroManualAbilityIcon();
        _heroPassiveIcon.sprite = heroSO.GetHeroPassiveAbilityIcon();
    }
    
    private void ShowStatCounter(List<Image> statCounters, int statNumber)
    {
        for(int i = 0; i < statCounters.Count; i++)
        {
            statCounters[i].enabled = (i<statNumber);
        }
    }

    public void HeroAbilityIconPressed(float abilityID)
    {
        if(_currentAbilityID == -1)
        {
            ShowHeroAbilityDescription(abilityID);
        }
        else if(abilityID != _currentAbilityID)
        {
            SwapAbilityDescription(abilityID);
        }
        else
        {
            HideAbilityDescription();
        }
    }


    public void ShowHeroAbilityDescription(float abilityID)
    {
        _currentAbilityID = abilityID;
        _heroAbilityDescriptionAnimator.SetBool(_showAbilityDescriptionBool, true);
    }

    public void UpdateHeroDescriptionText(string newText)
    {
        _heroAbilityBackgroundDescriptionText.text = newText;
        _heroAbilityDescriptionText.text = newText;
    }

    public void HeroAbilityDescriptionChanged()
    {
        switch(_currentAbilityID)
        {
            case (0):
                UpdateHeroDescriptionText(_lastHeroHoveredOver.GetHeroBasicAbilityDescription());
                return;
            case (1):
                UpdateHeroDescriptionText(_lastHeroHoveredOver.GetHeroManualAbilityDescription());
                return;
            case (2):
                UpdateHeroDescriptionText(_lastHeroHoveredOver.GetHeroPassiveAbilityDescription());
                return;

        }
            
    }

    public void SwapAbilityDescription(float abilityID)
    {
        _currentAbilityID = abilityID;
        _heroAbilityDescriptionAnimator.SetTrigger(_swapAbilityDescriptionTrigger);
    }

    public void HideAbilityDescription()
    {
        _heroAbilityDescriptionAnimator.SetBool(_showAbilityDescriptionBool, false);
        _currentAbilityID = -1;
    }


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
