using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_DreadSpears : SpecificBossAbilityFramework
{
    [Space] 
    [SerializeField] private bool _doesSpawnRay;
    private SBP_RayOfHopeTargetZone _associatedRay;
    
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _dreadSpears;
    
    private GameObject _storedTargetZone;
    private Vector3 _attackDirection;
    private Vector3 _rayPosition;
    private Coroutine _targetZoneCoroutine;
    
    private IEnumerator UpdateTargetZone()
    {
        Vector3 lastCheckedDirection = Vector3.zero;
        
        while(!_storedTargetZone.IsUnityNull() && !_storedTarget.IsUnityNull())
        {
            _storedTargetLocation = _storedTarget.transform.position;
            _attackDirection = _storedTargetLocation - Vector3.zero;

            if (lastCheckedDirection == _attackDirection)
            {
                yield return null;
                continue;
            }
            lastCheckedDirection = _attackDirection;

            _storedTargetZone.transform.LookAt(_storedTarget.transform);
            _storedTargetZone.transform.eulerAngles = new Vector3(0, _storedTargetZone.transform.eulerAngles.y, 0);

            if (!_associatedRay.IsUnityNull())
            {
                _rayPosition = _storedTarget.transform.position;
                _rayPosition.Set(-_rayPosition.x,_rayPosition.y,-_rayPosition.z);
                _associatedRay.SetRayPosition(_rayPosition);
            }

            yield return null;
            
        }
    }
    
    #region Base Ability

    protected override void StartShowTargetZone()
    {
        _storedTargetZone = Instantiate(_targetZone, transform.position, Quaternion.identity);
        
        _storedTargetZone.transform.position = new Vector3(_storedTargetZone.transform.position.x,
            _specificAreaTarget.y, _storedTargetZone.transform.position.z);

        //Adds the newly spawn target zone into the list of target zones currently active
        _currentTargetZones.Add(_storedTargetZone.GetComponent<BossTargetZoneParent>());

        _associatedRay = SB_VoidLord.Instance.SpawnRayOfHopeTargetZone(_storedTarget.transform.position);

        _targetZoneCoroutine = StartCoroutine(UpdateTargetZone());
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        if (!_targetZoneCoroutine.IsUnityNull())
        {
            StopCoroutine(_targetZoneCoroutine);
        }
        
        //Sets up the projectile
        SBP_DreadSpears dreadSpears = Instantiate(_dreadSpears, _storedTargetZone.transform.position, Quaternion.identity)
            .GetComponent<SBP_DreadSpears>();
        
        dreadSpears.transform.LookAt(_storedTarget.transform.position);
        dreadSpears.transform.eulerAngles = new Vector3(0, dreadSpears.transform.eulerAngles.y, 0);
        
        dreadSpears.SetUpProjectile(_myBossBase, _abilityID, _wasBossEnragedOnAbilityActivation);

        if (!_associatedRay.IsUnityNull())
        {
            _associatedRay.SpawnRayOfHope();
        }
        
        base.AbilityStart();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
        _associatedRay.RemoveRay();
    }
    #endregion
}
