using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_VoidMawProjectile : BossProjectileFramework
{
    [SerializeField] private float _baseMoveTime;
    [SerializeField] private AnimationCurve _moveCurve;
    private float _moveTime;
    
    private Vector3 _startLocation;
    private Vector3 _midPointLocation = new();
    private Vector3 _endLocation;
    
    private int _movementID = 0;
    private Vector3 _movementStartLocation;

    private SBP_VoidMaw _associatedMawOwner;
    
    public void AdditionalSetUp(SBP_VoidMaw maw, int mawID, Vector3 startLocation, float heroDistance)
    {
        _associatedMawOwner = maw;
        _startLocation = startLocation;
        
        _midPointLocation.Set(_startLocation.z,_startLocation.y,_startLocation.x);
        _midPointLocation *= mawID > 0 ? -1 : 1;
        
        _endLocation = -startLocation;
        
        _moveTime = _baseMoveTime * heroDistance;

        UseNextVoidMawMovement();
    }

    private void UseNextVoidMawMovement()
    {
        switch (_movementID)
        {
            case 0:
                StartMovingVoidMaw(_startLocation,_midPointLocation);
                return;
            case 1:
                StartMovingVoidMaw(_midPointLocation,_endLocation);
                return;
            default:
                _associatedMawOwner.MawReachedEnd();
                return;
        }
        
    }
    
    private void StartMovingVoidMaw(Vector3 startLocation, Vector3 endLocation)
    {
        _movementID++;
        StartCoroutine(MoveVoidMaw(startLocation,endLocation));
    }

    private IEnumerator MoveVoidMaw(Vector3 startLocation, Vector3 endLocation)
    {
        float progress = 0;
        while (progress < 1)
        {
            progress += Time.deltaTime / _moveTime;
            transform.position = Vector3.Lerp(startLocation, endLocation, _moveCurve.Evaluate(progress));
            yield return null;
        }
        transform.position = endLocation;
        UseNextVoidMawMovement();
    }
    
    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        
    }
    #endregion
}
