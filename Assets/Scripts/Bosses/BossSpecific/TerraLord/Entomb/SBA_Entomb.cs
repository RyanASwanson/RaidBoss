using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Terra Lord's Entomb ability
/// </summary>
public class SBA_Entomb : SpecificBossAbilityFramework
{
    [SerializeField] private float _rotationAmount;

    private Quaternion _storedTargetRotation;

    [SerializeField] private GameObject _entomb;

    [SerializeField] private GameObject _targetZone;

    
    /// <summary>
    /// Calculates the Y rotation of the target zone and attack
    /// </summary>
    protected void CalculateAttackRotation()
    {
        float rotationMultiplier = 1;

        //Randomly determine if we are adding or subtracting Y rotation
        //Determines direction of rotation
        switch (Random.Range(0, 2))
        {
            case (0):
                rotationMultiplier *= -1;
                break;
            case (1):
                break;
        }

        //Multiplies our Y rotation by the direction and amount
        float randomYRotation = rotationMultiplier * _rotationAmount;
        
        //Set the rotation of the attack
        _storedTargetRotation = Quaternion.Euler(new Vector3(0, randomYRotation, 0));

    }

    #region Base Ability
    protected override void StartShowTargetZone()
    {
        CalculateAttackRotation();

        _currentTargetZones.Add(Instantiate(_targetZone, _storedTargetLocation, _storedTargetRotation));

        base.StartShowTargetZone();
    }

    protected override void AbilityStart()
    {
        GameObject storedEntomb = Instantiate(_entomb, _storedTargetLocation, _storedTargetRotation);
        storedEntomb.GetComponent<SBP_Entomb>().SetUpProjectile(_myBossBase);

        base.AbilityStart();
    }
    #endregion

}
