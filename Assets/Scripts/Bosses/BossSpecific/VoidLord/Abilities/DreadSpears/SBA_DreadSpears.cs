using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_DreadSpears : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _dreadSpears;
    
    private GameObject _storedTargetZone;
    private Vector3 _attackDirection;
    
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

            yield return null;
            
        }
    }
    
    #region Base Ability

    protected override void StartShowTargetZone()
    {
        _storedTargetZone = Instantiate(_targetZone, transform.position, Quaternion.identity);
        
        //Vector3.set does not work here
        _storedTargetZone.transform.position = new Vector3(_storedTargetZone.transform.position.x,
            _specificAreaTarget.y, _storedTargetZone.transform.position.z);

        //Adds the newly spawn target zone into the list of target zones currently active
        _currentTargetZones.Add(_storedTargetZone.GetComponent<BossTargetZoneParent>());

        StartCoroutine(UpdateTargetZone());
        
        base.StartShowTargetZone();
    }


    protected override void AbilityStart()
    {
        //Sets up the projectile
        SBP_DreadSpears dreadSpears = Instantiate(_dreadSpears, _storedTargetZone.transform.position, Quaternion.identity)
            .GetComponent<SBP_DreadSpears>();
        
        dreadSpears.transform.LookAt(_storedTarget.transform.position);
        dreadSpears.transform.eulerAngles = new Vector3(0, dreadSpears.transform.eulerAngles.y, 0);
        
        dreadSpears.SetUpProjectile(_myBossBase, _abilityID, _wasBossEnragedOnAbilityActivation);
        //avalanche.AdditionalSetUp(_storedTargetLocation, this);
        
        base.AbilityStart();
    }
    #endregion
}
