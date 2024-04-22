using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeroBase : MonoBehaviour
{
    [SerializeField] private HeroPathfinding _heroPathfinding;

    private UnityEvent<HeroSO> _heroSOSetEvent = new UnityEvent<HeroSO>();
    private UnityEvent _heroControlledStartEvent = new UnityEvent();
    private UnityEvent _heroControlledEndEvent = new UnityEvent();

    [Header("TEST")]
    [SerializeField] private HeroSO _testSO;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
        SetHeroSO(_testSO);
    }

    void Setup()
    {
        foreach (HeroChildrenFunctionality childFunc in GetComponentsInChildren<HeroChildrenFunctionality>())
            childFunc.ChildFuncSetup(this);
    }

    #region Events
    private void InvokeSetHeroSO(HeroSO heroSO)
    {
        _heroSOSetEvent?.Invoke(heroSO);
    }

    public void InvokeHeroControlledBegin()
    {
        _heroControlledStartEvent?.Invoke();
    }

    public void InvokeHeroControlledEnd()
    {
        _heroControlledEndEvent?.Invoke();
    }
    #endregion

    #region Getters
    public HeroPathfinding GetPathfinding() => _heroPathfinding;
    public UnityEvent<HeroSO> GetSOSetEvent() => _heroSOSetEvent;
    public UnityEvent GetHeroControlledBeginEvent() => _heroControlledStartEvent;
    public UnityEvent GetHeroControlledEndEvent() => _heroControlledEndEvent;
    #endregion

    #region Setters
    private void SetHeroSO(HeroSO heroSO)
    {
        InvokeSetHeroSO(heroSO);
    }
    #endregion
}
