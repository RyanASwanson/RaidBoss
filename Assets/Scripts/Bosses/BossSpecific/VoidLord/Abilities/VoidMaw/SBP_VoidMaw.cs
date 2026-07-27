using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_VoidMaw : BossProjectileFramework
{
    [SerializeField] private SBP_VoidMawProjectile[] _voidMaws;

    private SBA_VoidMaw _voidMawAbility;
    private HeroBase _targetHero;
    private float _heroDistance;
    private Vector3 _heroCorner;
    
    private Vector3 _startLocation = new();
    private Vector3 _endLocation = new();

    private int _projectilesCompleted = 0;

    public void AdditionalSetUp(SBA_VoidMaw voidMawAbility, Vector3 heroCorner, float heroDistance)
    {
        _voidMawAbility = voidMawAbility;
        _heroCorner = heroCorner;
        _heroDistance = heroDistance;
        
        StartMovingVoidMaws();
    }

    private void StartMovingVoidMaws()
    {
        for(int i = 0; i < _voidMaws.Length; i++)
        {
            _voidMaws[i].SetUpProjectile(_myBossBase,_abilityID);
            _voidMaws[i].AdditionalSetUp(this, i,-_heroCorner, _heroDistance);
        }
    }

    public void MawReachedEnd()
    {
        _projectilesCompleted++;
        
        if (_projectilesCompleted >= _voidMaws.Length)
        {
            _voidMawAbility.MawsReachedEnd();
            Destroy(gameObject);
        }
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        
    }
    #endregion
}
