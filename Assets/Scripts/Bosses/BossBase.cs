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

    private UnityEvent<float> _bossDamagedEvent = new UnityEvent<float>();
    private UnityEvent<float> _bossStaggerDealtEvent = new UnityEvent<float>();

    protected UnityEvent _bossAbilityUsedEvent = new UnityEvent();

    private UnityEvent _bossDiedEvent = new UnityEvent();
    private UnityEvent _bossStaggeredEvent = new UnityEvent();

    public void Setup(BossSO newSO)
    {
        CreateBossPrefab(newSO);

        SetupChildren();

        SetUpAbilities();

        SetBossSO(newSO);
    }

    /// <summary>
    /// Creates the gameobject for the specific boss and saves needed data
    /// </summary>
    /// <param name="newSO"></param>
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
    private void SetupChildren()
    {
        foreach (BossChildrenFunctionality childFunc in GetComponentsInChildren<BossChildrenFunctionality>())
            childFunc.ChildFuncSetup(this);
    }

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

    public void InvokeBossDiedEvent()
    {
        _bossDiedEvent?.Invoke();
    }
    
    public void InvokeBossStaggeredEvent()
    {
        _bossStaggeredEvent?.Invoke();
    }
    #endregion

    #region Getters
    public BossVisuals GetBossVisuals() => _bossVisuals;
    public BossStats GetBossStats() => _bossStats;

    public BossSO GetBossSO() => _associatedBoss;

    public GameObject GetAssociatedBossObject() => _bossSpecificsGO;
    public SpecificBossFramework GetSpecificBossScript() => _associatedBossScript;



    public UnityEvent<BossSO> GetSOSetEvent() => _bossSOSetEvent;

    public UnityEvent<float> GetBossDamagedEvent() => _bossDamagedEvent;
    public UnityEvent<float> GetBossStaggerDealtEvent() => _bossStaggerDealtEvent;

    public UnityEvent GetBossAbilityUsedEvent() => _bossAbilityUsedEvent;

    public UnityEvent GetBossDiedEvent() => _bossDiedEvent;
    public UnityEvent GetBossStaggeredEvent() => _bossStaggeredEvent;
    #endregion

    #region Setters
    public void SetBossSO(BossSO bossSO)
    {
        _associatedBoss = bossSO;
        InvokeSetBossSO(bossSO);
    }
    #endregion
}
