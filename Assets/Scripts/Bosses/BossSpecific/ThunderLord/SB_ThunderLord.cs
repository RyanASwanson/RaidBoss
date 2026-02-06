using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_ThunderLord : SpecificBossFramework
{
    public static SB_ThunderLord Instance;
    
    [Space]
    [SerializeField] private SBA_ImpendingStorm _impendingStorm;

    [Space] 
    [SerializeField] private GameObject _deathVFX;
    
    private GameObject _impendingStormTargetZone;
    
    #region BaseBoss
    
    protected override void CreateSpecificBossInstance()
    {
        Instance = this;
    }

    protected override void StartFight()
    {
        _impendingStormTargetZone = _impendingStorm.BattleStart();
        base.StartFight();
    }

    protected override void BossDied()
    {
        /*BossVisuals.Instance.AddOutlineToBoss();
        BossVisuals.Instance.SetOutLineColor(Color.black);*/
        base.BossDied();
        
        Instantiate(_deathVFX, _bossVisualsBase.transform.position, Quaternion.identity);
    }
    #endregion

    public void ChildGameObjectToImpendingStorm(GameObject childObject)
    {
        childObject.transform.SetParent(_impendingStormTargetZone.transform);
        childObject.transform.localPosition = Vector3.zero;
        childObject.transform.localRotation = Quaternion.identity;
    }
    
    #region Getters
    public SBA_ImpendingStorm GetImpendingStorm() => _impendingStorm;

    #endregion
}
