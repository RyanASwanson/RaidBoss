using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossBase : MonoBehaviour
{
    [SerializeField] private BossStats _bossStats;

    private BossSO _associatedBoss;

    private UnityEvent<BossSO> _bossSOSetEvent = new UnityEvent<BossSO>();

    public void Setup(BossSO bossSO)
    {
        SetupChildren();

        SetBossSO(bossSO);
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
    public BossStats GetBossStats() => _bossStats;
    #endregion

    #region Setters
    public void SetBossSO(BossSO bossSO)
    {
        _associatedBoss = bossSO;
    }
    #endregion
}
