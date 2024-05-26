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


    // Start is called before the first frame update
    void Start()
    {
        SubscribeToEvents();
    }

    private void NewHeroAdded(HeroSO heroSO)
    {

    }

    private void SubscribeToEvents()
    {
        //UniversalManagers.Instance.GetSelectionManager().GetBossSelectionEvent().AddListener

        //Univers
    }
}
