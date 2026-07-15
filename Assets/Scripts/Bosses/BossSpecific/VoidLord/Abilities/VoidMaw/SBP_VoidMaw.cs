using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_VoidMaw : BossProjectileFramework
{
    [SerializeField] private SBP_VoidMawProjectile[] _voidMaws;

    private HeroBase _targetHero;
    private float _heroDistance;
    
    private Vector3 _startLocation = new();
    private Vector3 _endLocation = new();

    private int _projectilesCompleted = 0;

    public void AdditionalSetUp(HeroBase targetHero, float heroDistance)
    {
        _targetHero = targetHero;
        _heroDistance = heroDistance;
        
        DetermineTargetLocations();
        StartMovingVoidMaws();
    }
    
    private void DetermineTargetLocations()
    {
        _startLocation.Set(-Mathf.Round(_targetHero.transform.position.x / _heroDistance) * _heroDistance, 0, 
            -Mathf.Round(_targetHero.transform.position.z / _heroDistance) * _heroDistance);

        if (Mathf.Abs(_startLocation.x) > Mathf.Abs(_startLocation.z))
        {
            _startLocation.Set(_startLocation.x, 0, 0);
        }
        else
        {
            _startLocation.Set(0, 0, _startLocation.z);
        }
        Debug.Log("Target Location " + _startLocation + " from " + _targetHero.transform.position + " and hero distance of " + _heroDistance);
    }

    private void StartMovingVoidMaws()
    {
        for(int i = 0; i < _voidMaws.Length; i++)
        {
            _voidMaws[i].SetUpProjectile(_myBossBase,_abilityID);
            _voidMaws[i].AdditionalSetUp(this, i,_startLocation, _heroDistance);
        }
    }

    public void MawReachedEnd()
    {
        _projectilesCompleted++;
        
        if (_projectilesCompleted >= _voidMaws.Length)
        {
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
