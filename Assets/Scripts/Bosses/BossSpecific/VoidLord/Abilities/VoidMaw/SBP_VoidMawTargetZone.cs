using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBP_VoidMawTargetZone : BossProjectileFramework
{
    [SerializeField] private bool _doesTargetZoneTrack;
    
    [Space]
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
    private float _cornerDistance;
    
    private Vector3 _tempHeroPosition;
    private Vector3 _tempTargetZoneLocation;
    
    private Vector3 _closestHeroCorner = new();
    private Vector3 _heroCornerDistanceVector;
    
    private Coroutine _targetZoneTrackingProcess;
    
    private HeroBase _storedHeroTarget;

    public void TargetZoneSetUp(HeroBase heroTarget, SBP_RayOfHopeTargetZone associatedRay)
    {
        _storedHeroTarget = heroTarget;

        SetUpDirections();

        if (_doesTargetZoneTrack)
        {
            _targetZoneTrackingProcess = StartCoroutine(UpdateTargetZonesProcess(associatedRay));
        }
        else
        {
            UpdateTargetZones(associatedRay);
        }
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

    private IEnumerator UpdateTargetZonesProcess(SBP_RayOfHopeTargetZone associatedRay)
    {
        while (!_storedHeroTarget.IsUnityNull())
        {
            UpdateTargetZones(associatedRay);
            
            yield return null;
        }
        
    }

    private void UpdateTargetZones(SBP_RayOfHopeTargetZone associatedRay)
    {
        _tempHeroPosition = Quaternion.AngleAxis(-45,Vector3.up) * _storedHeroTarget.transform.position;
            
        _heroDistance = Mathf.Max(Mathf.Abs(_tempHeroPosition.x),Mathf.Abs(_tempHeroPosition.z));
            
        _heroDistance = Mathf.Clamp(_heroDistance, _minTargetZoneDistance, _maxTargetZoneDistance);
            
        for (int i = 0; i < _targetZones.Length; i++)
        {
            _tempTargetZoneLocation = _targetZoneDirections[i] * _heroDistance;
            _tempTargetZoneLocation.Set(_tempTargetZoneLocation.x, _targetZoneHeight, _tempTargetZoneLocation.z);
            _targetZones[i].transform.position = _tempTargetZoneLocation;
                
            _tempTargetZoneLocation.Set(1,1,GetTargetSizeSizeScalar() ) ;
            _targetZones[i].transform.localScale = _tempTargetZoneLocation;
        }
            
        CalculateClosestHeroCorner();
            
        if (!associatedRay.IsUnityNull())
        {
            associatedRay.SetRayPosition(_closestHeroCorner);
        }
    }
    
    public void CalculateClosestHeroCorner()
    {
        CalculateCornerDistance();
        
        _closestHeroCorner = Vector3.zero;
        if (Mathf.Abs(_storedHeroTarget.transform.position.x) >
            Mathf.Abs(_storedHeroTarget.transform.position.z))
        {
            _closestHeroCorner.x = _storedHeroTarget.transform.position.x > 0 ? _cornerDistance : -_cornerDistance;
        }
        else
        {
            _closestHeroCorner.z = _storedHeroTarget.transform.position.z > 0 ? _cornerDistance : -_cornerDistance;
        }
    }
    
    public void CalculateCornerDistance()
    {
        _heroCornerDistanceVector = new Vector3(GetHeroDistance(),0,GetHeroDistance());
        _heroCornerDistanceVector = Quaternion.AngleAxis(45, Vector3.up) * _heroCornerDistanceVector;
        _cornerDistance =  Mathf.Max(Mathf.Abs(_heroCornerDistanceVector.x), Mathf.Abs(_heroCornerDistanceVector.z));
    }

    public void StopVoidMawTargetTracking()
    {
        if (!_targetZoneTrackingProcess.IsUnityNull())
        {
            StopCoroutine(_targetZoneTrackingProcess);
        }
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

    public float GetCornerDistance() => _cornerDistance;

    public Vector3 GetClosestCorner() => _closestHeroCorner;
    
    public float GetTargetSizeSizeScalar() => (_heroDistance * _distanceMultiplicativeSizeScalar) + _distanceAdditiveSizeScalar;

    #endregion
}
