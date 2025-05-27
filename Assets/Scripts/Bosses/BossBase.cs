using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossBase : MonoBehaviour
{
    [Header("Child Functionality")]
    [SerializeField] private BossVisuals _bossVisuals;
    [SerializeField] private BossStats _bossStats;

    [Header("Child GameObjects")]
    [SerializeField] private GameObject _bossSpecificsGO;

    private BossSO _associatedBoss;
    private GameObject _associatedBossGameObject;
    private SpecificBossFramework _associatedBossScript;

    private UnityEvent<BossSO> _bossSOSetEvent = new UnityEvent<BossSO>();
    private UnityEvent _bossTargetsAssigned = new UnityEvent();

    private UnityEvent<float> _bossDamagedEvent = new UnityEvent<float>();
    private UnityEvent<float> _bossStaggerDealtEvent = new UnityEvent<float>();

    protected UnityEvent _bossAbilityUsedEvent = new UnityEvent();

    private UnityEvent _bossStaggeredEvent = new UnityEvent();
    private UnityEvent _bossNoLongerStaggeredEvent = new UnityEvent();

    protected UnityEvent _bossReachedHalfHealthEvent = new UnityEvent();
    protected UnityEvent _bossReachedQuarterHealthEvent = new UnityEvent();
    protected UnityEvent _bossReachedTenthHealthEvent = new UnityEvent();

    protected UnityEvent _bossEnragedEvent = new UnityEvent();

    public void SetUp(BossSO newSO)
    {
        CreateBossPrefab(newSO);

        SetUpChildren();

        SetUpAbilities();

        SetBossSO(newSO);
    }

    /// <summary>
    /// Creates the game object for the specific boss and saves needed data
    /// </summary>
    /// <param name="newSO"> The scriptable object of the boss </param>
    private void CreateBossPrefab(BossSO newSO)
    {
        _associatedBossGameObject = Instantiate(newSO.GetBossPrefab(), _bossSpecificsGO.transform);
        _associatedBossScript = _associatedBossGameObject.GetComponent<SpecificBossFramework>();

        //Tells the specific boss script to set up
        _associatedBossScript.SetupSpecificBoss(this);
    }

    /// <summary>
    /// Sets up all scripts that inherit from BossChildrenFunctionality
    /// </summary>
    private void SetUpChildren()
    {
        foreach (BossChildrenFunctionality childFunc in GetComponentsInChildren<BossChildrenFunctionality>())
        {
            childFunc.ChildFuncSetup(this);
        }
    }

    /// <summary>
    /// Sets up all abilities the boss has
    /// </summary>
    private void SetUpAbilities()
    {
        foreach(SpecificBossAbilityFramework bossAbility in GetComponentsInChildren<SpecificBossAbilityFramework>())
        {
            bossAbility.AbilitySetup(this);
        }
    }

    #region Events
    public void InvokeSetBossSO(BossSO bossSO)
    {
        _bossSOSetEvent?.Invoke(bossSO);
    }
    public void InvokeBossTargetsAssignedEvent()
    {
        _bossTargetsAssigned?.Invoke();
    }

    public void InvokeBossDamagedEvent(float damage)
    {
        _bossDamagedEvent?.Invoke(damage);
    }
    public void InvokeBossStaggerDealt(float stagger)
    {
        _bossStaggerDealtEvent?.Invoke(stagger);
    }

    public void InvokeBossAbilityUsedEvent()
    {
        _bossAbilityUsedEvent?.Invoke();
    }
    
    public void InvokeBossStaggeredEvent()
    {
        _bossStaggeredEvent?.Invoke();
    }
    public void InvokeBossNoLongerStaggeredEvent()
    {
        _bossNoLongerStaggeredEvent.Invoke();
    }

    public void InvokeBossHalfHealthEvent()
    {
        _bossReachedHalfHealthEvent?.Invoke();
    }
    public void InvokeBossQuarterHealthEvent()
    {
        _bossReachedQuarterHealthEvent?.Invoke();
    }
    public void InvokeBossTenthHealthEvent()
    {
        _bossReachedTenthHealthEvent?.Invoke();
    }

    public void InvokeBossEnragedEvent()
    {
        _bossEnragedEvent?.Invoke();
    }
    #endregion

    #region Getters
    public BossSO GetBossSO() => _associatedBoss;

    public GameObject GetAssociatedBossObject() => _bossSpecificsGO;
    public SpecificBossFramework GetSpecificBossScript() => _associatedBossScript;
    
    public UnityEvent<BossSO> GetSOSetEvent() => _bossSOSetEvent;
    public UnityEvent GetBossTargetsAssignedEvent() => _bossTargetsAssigned;

    public UnityEvent<float> GetBossDamagedEvent() => _bossDamagedEvent;
    public UnityEvent<float> GetBossStaggerDealtEvent() => _bossStaggerDealtEvent;

    public UnityEvent GetBossAbilityUsedEvent() => _bossAbilityUsedEvent;

    public UnityEvent GetBossStaggeredEvent() => _bossStaggeredEvent;
    public UnityEvent GetBossNoLongerStaggeredEvent() => _bossNoLongerStaggeredEvent;

    public UnityEvent GetBossHalfHealthEvent() => _bossReachedHalfHealthEvent;
    public UnityEvent GetBossQuarterHealthEvent() => _bossReachedQuarterHealthEvent;
    public UnityEvent GetBossTenthHealthEvent() => _bossReachedTenthHealthEvent;

    public UnityEvent GetBossEnragedEvent() => _bossEnragedEvent;
    #endregion

    #region Setters
    public void SetBossSO(BossSO bossSO)
    {
        _associatedBoss = bossSO;
        InvokeSetBossSO(bossSO);
    }
    #endregion
}
