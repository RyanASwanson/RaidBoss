using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_VoidMawTargetZone : BossProjectileFramework
{
    [SerializeField] private float _minTargetZoneDistance;
    [SerializeField] private float _maxTargetZoneDistance;

    [Space] 
    [SerializeField] private float _distanceMultiplicativeSizeScalar;
    [SerializeField] private float _distanceAdditiveSizeScalar;
    
    [Space]
    [SerializeField] private GameObject[] _targetZones;
    private Vector3[] _targetZoneDirections;
    private float _targetZoneHeight;
    private float _heroDistance;
    private Coroutine _targetZoneTrackingProcess;
    
    private HeroBase _storedHeroTarget;

    public void TargetZoneSetUp(HeroBase heroTarget)
    {
        _storedHeroTarget = heroTarget;

        SetUpDirections();
        
        _targetZoneTrackingProcess = StartCoroutine(UpdateTargetZones());
    }

    private void SetUpDirections()
    {
        _targetZoneDirections = new Vector3[_targetZones.Length];
        for (int i = 0; i < _targetZones.Length; i++)
        {
            _targetZoneDirections[i] = _targetZones[i].transform.position;
        }

        _targetZoneHeight = _targetZoneDirections[0].y;
    }

    private IEnumerator UpdateTargetZones()
    {
        Vector3 heroPosition = new();
        Vector3 targetZoneLocation = new();
        
        while (true)
        {
            heroPosition = Quaternion.AngleAxis(-45,Vector3.up) * _storedHeroTarget.transform.position;
            
            _heroDistance = Mathf.Max(Mathf.Abs(heroPosition.x),Mathf.Abs(heroPosition.z));
            
            _heroDistance = Mathf.Clamp(_heroDistance, _minTargetZoneDistance, _maxTargetZoneDistance);
            
            for (int i = 0; i < _targetZones.Length; i++)
            {
                targetZoneLocation = _targetZoneDirections[i] * _heroDistance;
                targetZoneLocation.Set(targetZoneLocation.x, _targetZoneHeight, targetZoneLocation.z);
                _targetZones[i].transform.position = targetZoneLocation;
                
                targetZoneLocation.Set(1,1,GetTargetSizeSizeScalar() ) ;
                _targetZones[i].transform.localScale = targetZoneLocation;
            }
            yield return null;
        }
        
    }

    public float StopVoidMawTargetTracking()
    {
        StopCoroutine(_targetZoneTrackingProcess);
        
        return GetCornerDistance();
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        //StartCoroutine(AbilityProcess());
    }
    #endregion
    
    #region Getters
    public float GetHeroDistance() => _heroDistance;

    public float GetCornerDistance()
    {
        Vector3 tempPos = new Vector3(GetHeroDistance(),0,GetHeroDistance());
        tempPos = Quaternion.AngleAxis(45, Vector3.up) * tempPos;

        return Mathf.Max(Mathf.Abs(tempPos.x), Mathf.Abs(tempPos.z));
    }
    
    public float GetTargetSizeSizeScalar() => (_heroDistance * _distanceMultiplicativeSizeScalar) + _distanceAdditiveSizeScalar;

    #endregion
}
