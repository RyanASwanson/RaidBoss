using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeroBase : MonoBehaviour
{
    [SerializeField] private HeroPathfinding _heroPathfinding;

    private UnityEvent<HeroSO> _heroSOSetEvent;

    [Header("TEST")]
    [SerializeField] private HeroSO _testSO;

    // Start is called before the first frame update
    void Start()
    {
        SetHeroSO(_testSO);
    }

    #region Events
    private void InvokeSetHeroSO(HeroSO heroSO)
    {
        _heroSOSetEvent?.Invoke(heroSO);
    }
    #endregion

    #region Getters
    public HeroPathfinding GetPathfinding() => _heroPathfinding;
    public UnityEvent<HeroSO> GetSOSetEvent() => _heroSOSetEvent;
    #endregion

    #region Setters
    private void SetHeroSO(HeroSO heroSO)
    {
        InvokeSetHeroSO(heroSO);
    }
    #endregion
}
