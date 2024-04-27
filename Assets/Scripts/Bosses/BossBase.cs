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

    public void Setup(BossSO newSO)
    {
        CreateBossPrefab(newSO);

        SetupChildren();

        SetBossSO(newSO);
    }

    private void CreateBossPrefab(BossSO newSO)
    {
        _associatedBossGameObject = Instantiate(newSO.GetBossPrefab(), _bossSpecificsGO.transform);
        _associatedBossScript = _associatedBossGameObject.GetComponent<SpecificBossFramework>();

        _associatedBossScript.SubscribeToEvents();
    }

    private void SetupChildren()
    {
        foreach (BossChildrenFunctionality childFunc in GetComponentsInChildren<BossChildrenFunctionality>())
            childFunc.ChildFuncSetup(this);
    }

    #region Events
    public void InvokeSetBossSO(BossSO bossSO)
    {
        _bossSOSetEvent?.Invoke(bossSO);
    }
    #endregion

    #region Getters
    public BossVisuals GetBossVisuals() => _bossVisuals;
    public BossStats GetBossStats() => _bossStats;
    #endregion

    #region Setters
    public void SetBossSO(BossSO bossSO)
    {
        _associatedBoss = bossSO;
    }
    #endregion
}
