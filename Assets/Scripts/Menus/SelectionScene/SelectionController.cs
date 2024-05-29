using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject _bossPillar;

    [Header("Center")]

    [Header("Hero")]
    [SerializeField] private List<HeroPillar> _heroPillars = new List<HeroPillar>();

    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();

        HeroSideStart();
    }

    #region Boss Side
    private void BossSideStart()
    {

    }
    #endregion

    #region Center

    #endregion

    #region Hero Side
    private void HeroSideStart()
    {
        MoveHeroPillar(0, true);
    }

    private void NewHeroAdded(HeroSO heroSO)
    {
        int heroPillarNum = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();

        _heroPillars[heroPillarNum - 1].ShowHeroOnPillar(heroSO);

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
            _heroPillars[pillarNum].ShowHeroOnPillar(_heroPillars[pillarNum + 1].GetStoredHero());
            _heroPillars[pillarNum + 1].RemoveHeroOnPillar();
        }
        MoveNextHeroBackToCurrentPillar(pillarNum + 1);
        
    }

    private void MoveHeroPillar(int pillarNum, bool moveUp)
    {
        _heroPillars[pillarNum].MovePillar(moveUp);
    }

    #endregion

    private void SubscribeToEvents()
    {
        //UniversalManagers.Instance.GetSelectionManager().GetBossSelectionEvent().AddListener

        UniversalManagers.Instance.GetSelectionManager().GetHeroSelectionEvent().AddListener(NewHeroAdded);
        UniversalManagers.Instance.GetSelectionManager().GetHeroDeselectionEvent().AddListener(HeroRemoved);
    }
}
