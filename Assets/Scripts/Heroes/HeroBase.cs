using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Holds the base functionality for the hero
/// Holds the children functionality scripts
/// Holds the specific hero framework
/// Holds all events relating to the hero
/// </summary>
public class HeroBase : MonoBehaviour
{
    [Header("ChildFunctionality")]
    [SerializeField] private HeroPathfinding _heroPathfinding;
    [SerializeField] private HeroVisuals _heroVisuals;
    [SerializeField] private HeroStats _heroStats;
    [Space]

    [Header("Child GameObjects")]
    [SerializeField] private GameObject _heroSpecificsGO;

    [Header("Colliders")]
    [SerializeField] private Collider _damageCollider;

    private HeroSO _associatedSO;
    private GameObject _associatedHeroGameObject;
    private SpecificHeroFramework _associatedHeroScript;

    private UnityEvent<HeroSO> _heroSOSetEvent = new UnityEvent<HeroSO>();

    private UnityEvent _heroControlledStartEvent = new UnityEvent();
    private UnityEvent _heroControlledEndEvent = new UnityEvent();

    private UnityEvent _heroManualAbilityCharging = new UnityEvent();
    private UnityEvent _heroManualAbilityFullyCharged = new UnityEvent();
    private UnityEvent<Vector3> _heroManualAbilityAttempt = new UnityEvent<Vector3>();

    private UnityEvent _heroStartedMovingOnMeshEvent = new UnityEvent();
    private UnityEvent _heroStoppedMovingOnMeshEvent = new UnityEvent();

    //Event called when hero takes damage. Damage taken stored as float
    private UnityEvent<float> _heroDamagedEvent = new UnityEvent<float>();
    private UnityEvent<float> _heroDamagedOverrideEvent = new UnityEvent<float>();
    private UnityEvent<float> _heroHealedEvent = new UnityEvent<float>();

    //Event called when hero health changes. Current health stored as float
    private UnityEvent<float> _heroHealthChanged = new UnityEvent<float>();

    private UnityEvent _heroDamagedUnderHalfEvent = new UnityEvent();
    private UnityEvent _heroDamagedUnderQuarterEvent = new UnityEvent();

    private UnityEvent _heroHealedAboveHalfEvent = new UnityEvent();
    private UnityEvent _heroHealedAboveQuarterEvent = new UnityEvent();

    private UnityEvent _heroDiedEvent = new UnityEvent();

    public void Setup(HeroSO newSO)
    {
        CreateHeroPrefab(newSO);

        SetupChildren();

        SetHeroSO(newSO);

        AssignSelfToUI();
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

    private void AssignSelfToUI()
    {
        GameplayManagers.Instance.GetGameUIManager().SetAssociatedHeroUIManager(this);
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

    public void InvokeHeroManualAbilityChargingEvent()
    {
        _heroManualAbilityCharging?.Invoke();
    }
    public void InvokeHeroManualAbilityFullyChargedEvent()
    {
        _heroManualAbilityFullyCharged?.Invoke();
    }
    public void InvokeHeroManualAbilityAttemptEvent(Vector3 activateLocation)
    {
        _heroManualAbilityAttempt?.Invoke(activateLocation);
    }
    public void InvokeHeroStartedMovingEvent()
    {
        _heroStartedMovingOnMeshEvent?.Invoke();
    }
    public void InvokeHeroStoppedMovingEvent()
    {
        _heroStoppedMovingOnMeshEvent?.Invoke();
    }
    public void InvokeHeroDamagedEvent(float damageAmount)
    {
        _heroDamagedEvent?.Invoke(damageAmount);

        InvokeHeroHealthChangedEvent();
    }
    public void InvokeHeroDamageOverrideEvent(float damageAmount)
    {
        _heroDamagedOverrideEvent?.Invoke(damageAmount);
    }
    public void InvokeHeroHealedEvent(float healAmount)
    {
        _heroHealedEvent?.Invoke(healAmount);

        InvokeHeroHealthChangedEvent();
    }

    public void InvokeHeroHealthChangedEvent()
    {
        _heroHealthChanged?.Invoke(GetHeroStats().GetCurrentHealth());
    }

    public void InvokeHeroDamagedUnderHalfEvent()
    {
        _heroDamagedUnderHalfEvent?.Invoke();
    }

    public void InvokeHeroDamagedUnderQuarterEvent()
    {
        _heroDamagedUnderQuarterEvent?.Invoke();
    }

    public void InvokeHeroHealedAboveHalfEvent()
    {
        _heroHealedAboveHalfEvent?.Invoke();
    }

    public void InvokeHeroHealedAboveQuarterEvent()
    {
        _heroDamagedUnderQuarterEvent?.Invoke();
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

    public Collider GetHeroDamageCollider() => _damageCollider;

    public HeroSO GetHeroSO() => _associatedSO;

    public GameObject GetAssociatedHeroObject() => _associatedHeroGameObject;
    public SpecificHeroFramework GetSpecificHeroScript() => _associatedHeroScript;

    public UnityEvent<HeroSO> GetSOSetEvent() => _heroSOSetEvent;
    public UnityEvent GetHeroControlledBeginEvent() => _heroControlledStartEvent;
    public UnityEvent GetHeroControlledEndEvent() => _heroControlledEndEvent;

    public UnityEvent GetHeroManualAbilityChargingEvent() => _heroManualAbilityCharging;
    public UnityEvent GetHeroManualAbilityFullyChargedEvent() => _heroManualAbilityFullyCharged;
    public UnityEvent<Vector3> GetHeroManualAbilityAttemptEvent() => _heroManualAbilityAttempt;

    public UnityEvent GetHeroStartedMovingEvent() => _heroStartedMovingOnMeshEvent;
    public UnityEvent GetHeroStoppedMovingEvent() => _heroStoppedMovingOnMeshEvent;

    public UnityEvent<float> GetHeroDamagedEvent() => _heroDamagedEvent;
    public UnityEvent<float> GetHeroDamagedOverrideEvent() => _heroDamagedOverrideEvent;
    public UnityEvent<float> GetHeroHealedEvent() => _heroHealedEvent;

    public UnityEvent<float> GetHeroHealthChangedEvent() => _heroHealthChanged;

    public UnityEvent GetHeroDamagedUnderHalfEvent() => _heroDamagedUnderHalfEvent;
    public UnityEvent GetHeroDamagedUnderQuarterEvent() => _heroDamagedUnderQuarterEvent;
    public UnityEvent GetHeroHealedAboveHalfEvent() => _heroHealedAboveHalfEvent;
    public UnityEvent GetHeroHealedAboveQuarterEvent() => _heroHealedAboveQuarterEvent;

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
