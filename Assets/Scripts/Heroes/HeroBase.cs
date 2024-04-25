using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeroBase : MonoBehaviour
{
    [SerializeField] private HeroPathfinding _heroPathfinding;
    [SerializeField] private HeroVisuals _heroVisuals;
    [SerializeField] private HeroStats _heroStats;

    private HeroSO _associatedSO;

    private UnityEvent<HeroSO> _heroSOSetEvent = new UnityEvent<HeroSO>();
    private UnityEvent _heroControlledStartEvent = new UnityEvent();
    private UnityEvent _heroControlledEndEvent = new UnityEvent();

    private UnityEvent<float> _heroDamaged = new UnityEvent<float>();
    private UnityEvent<float> _heroHealed = new UnityEvent<float>();

    public void Setup(HeroSO newSO)
    {
        foreach (HeroChildrenFunctionality childFunc in GetComponentsInChildren<HeroChildrenFunctionality>())
            childFunc.ChildFuncSetup(this);

        SetHeroSO(newSO);
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
    public HeroVisuals GetHeroVisuals() => _heroVisuals;
    public HeroStats GetHeroStats() => _heroStats;

    public UnityEvent<HeroSO> GetSOSetEvent() => _heroSOSetEvent;
    public UnityEvent GetHeroControlledBeginEvent() => _heroControlledStartEvent;
    public UnityEvent GetHeroControlledEndEvent() => _heroControlledEndEvent;
    #endregion

    #region Setters
    public void SetHeroSO(HeroSO heroSO)
    {
        _associatedSO = heroSO;
        InvokeSetHeroSO(heroSO);
    }
    #endregion
}
