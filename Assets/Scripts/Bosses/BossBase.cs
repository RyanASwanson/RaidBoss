using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossBase : MonoBehaviour
{
    private BossSO _associatedBoss;

    private UnityEvent<BossSO> _bossSOSetEvent = new UnityEvent<BossSO>();

    public void Setup(BossSO bossSO)
    {

    }

    #region Events

    #endregion

    #region Getters

    #endregion

    #region Setters

    #endregion
}
