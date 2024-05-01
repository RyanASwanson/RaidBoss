using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeroBase : MonoBehaviour
{
    [Header("ChildFunctionality")]
    [SerializeField] private HeroPathfinding _heroPathfinding;
    [SerializeField] private HeroVisuals _heroVisuals;
    [SerializeField] private HeroStats _heroStats;
    [Space]

    [Header("Child GameObjects")]
    [SerializeField] private GameObject _heroSpecificsGO;

    private HeroSO _associatedSO;
    private GameObject _associatedHeroGameObject;
    private SpecificHeroFramework _associatedHeroScript;

    private UnityEvent<HeroSO> _heroSOSetEvent = new UnityEvent<HeroSO>();

    private UnityEvent _heroControlledStartEvent = new UnityEvent();
    private UnityEvent _heroControlledEndEvent = new UnityEvent();

    private UnityEvent<Vector3> _heroManualAbilityAttempt = new UnityEvent<Vector3>();

    private UnityEvent _heroStartedMovingOnMeshEvent = new UnityEvent();
    private UnityEvent _heroStoppedMovingOnMeshEvent = new UnityEvent();

    private UnityEvent<float> _heroDamagedEvent = new UnityEvent<float>();
    private UnityEvent<float> _heroHealedEvent = new UnityEvent<float>();

    private UnityEvent _heroDiedEvent = new UnityEvent();

    public void Setup(HeroSO newSO)
    {
        CreateHeroPrefab(newSO);

        SetupChildren();

        SetHeroSO(newSO);
    }

    /// <summary>
    /// Creates the gameobject for the specific hero and saves needed data
    /// </summary>
    /// <param name="newSO"></param>
    private void CreateHeroPrefab(HeroSO newSO)
    {
        _associatedHeroGameObject = Instantiate(newSO.GetHeroPrefab(), _heroSpecificsGO.transform);
        _associatedHeroScript = _associatedHeroGameObject.GetComponentInChildren<SpecificHeroFramework>();

        _associatedHeroScript.SetupSpecificHero(this, newSO);
    }

    /// <summary>
    /// Sets up all scripts that inherit from HeroChildrenFunctionality
    /// </summary>
    private void SetupChildren()
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

    public void InvokeHeroManualAbilityAttempt(Vector3 activateLocation)
    {
        _heroManualAbilityAttempt?.Invoke(activateLocation);
    }
    public void InvokeHeroStartedMoving()
    {
        _heroStartedMovingOnMeshEvent?.Invoke();
    }
    public void InvokeHeroStoppedMoving()
    {
        _heroStoppedMovingOnMeshEvent?.Invoke();
    }
    public void InvokeHeroDamagedEvent(float damageAmount)
    {
        _heroDamagedEvent?.Invoke(damageAmount);
    }
    public void InvokeHeroHealedEvent(float healAmount)
    {
        _heroHealedEvent?.Invoke(healAmount);
    }
    public void InvokeHeroDiedEvent()
    {
        _heroDiedEvent?.Invoke();
    }
    #endregion

    #region Getters
    public HeroPathfinding GetPathfinding() => _heroPathfinding;
    public HeroVisuals GetHeroVisuals() => _heroVisuals;
    public HeroStats GetHeroStats() => _heroStats;

    public HeroSO GetHeroSO() => _associatedSO;

    public GameObject GetAssociatedHeroObject() => _associatedHeroGameObject;
    public SpecificHeroFramework GetSpecificHeroScript() => _associatedHeroScript;

    public UnityEvent<HeroSO> GetSOSetEvent() => _heroSOSetEvent;
    public UnityEvent GetHeroControlledBeginEvent() => _heroControlledStartEvent;
    public UnityEvent GetHeroControlledEndEvent() => _heroControlledEndEvent;

    public UnityEvent<Vector3> GetHeroManualAbilityAttemptEvent() => _heroManualAbilityAttempt;

    public UnityEvent GetHeroStartedMovingEvent() => _heroStartedMovingOnMeshEvent;
    public UnityEvent GetHeroStoppedMovingEvent() => _heroStoppedMovingOnMeshEvent;

    public UnityEvent<float> GetHeroDamagedEvent() => _heroDamagedEvent;
    public UnityEvent<float> GetHeroHealedEvent() => _heroHealedEvent;

    public UnityEvent GetHeroDiedEvent() => _heroDiedEvent;
    #endregion

    #region Setters
    public void SetHeroSO(HeroSO heroSO)
    {
        _associatedSO = heroSO;
        InvokeSetHeroSO(heroSO);
    }
    #endregion
}
