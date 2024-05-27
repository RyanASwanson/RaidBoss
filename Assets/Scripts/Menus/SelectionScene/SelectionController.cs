using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject _bossPillar;

    [Header("Center")]

    [Header("Hero")]
    [SerializeField] private List<GameObject> _heroPillars = new List<GameObject>();

    private const string _heroPillarMoveAnimBool = "PillarUp";

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
        int heroPillarToMove = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount();
        if (heroPillarToMove < UniversalManagers.Instance.GetSelectionManager().GetMaxHeroesCount())
            MoveHeroPillar(heroPillarToMove, true);
    }

    private void HeroRemoved(HeroSO heroSO)
    {
        int heroPillarToMove = UniversalManagers.Instance.GetSelectionManager().GetSelectedHeroesCount() +1;
        if (heroPillarToMove > 0 && 
            heroPillarToMove < UniversalManagers.Instance.GetSelectionManager().GetMaxHeroesCount())
            MoveHeroPillar(heroPillarToMove, false);
    }

    private void MoveHeroPillar(int pillarNum, bool moveUp)
    {
        _heroPillars[pillarNum].GetComponent<Animator>().SetBool(_heroPillarMoveAnimBool, moveUp);
    }

    #endregion

    private void SubscribeToEvents()
    {
        //UniversalManagers.Instance.GetSelectionManager().GetBossSelectionEvent().AddListener

        UniversalManagers.Instance.GetSelectionManager().GetHeroSelectionEvent().AddListener(NewHeroAdded);
        UniversalManagers.Instance.GetSelectionManager().GetHeroDeselectionEvent().AddListener(HeroRemoved);
    }
}
