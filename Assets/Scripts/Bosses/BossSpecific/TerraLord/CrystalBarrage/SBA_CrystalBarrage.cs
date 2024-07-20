using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBA_CrystalBarrage : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private int _projectileCount;
    [SerializeField] private float _timeBetweenProjectiles;

    [Space]
    [SerializeField] private float _targetDistance;
    private Vector3 _currentTargetLocation;

    private Vector3[] _targetDirections = { Vector3.forward, Vector3.left, Vector3.back,Vector3.right};
    private const float _targetHeight = -.75f;

    [SerializeField] private GameObject _crystalBarrage;
    [SerializeField] private GameObject _barrageUpwardsVisual;
    [SerializeField] private GameObject _targetZone;

    [Space]
    [SerializeField] private GameObject _upwardsCrystalSource;

    public override void AbilitySetup(BossBase bossBase)
    {
        base.AbilitySetup(bossBase);
        CalculateTargetLocations();
    }

    private void CalculateTargetLocations()
    {
        for (int i = 0; i < _targetDirections.Length; i++)
        {
            _targetDirections[i] *= _targetDistance;
            _targetDirections[i] = new Vector3(_targetDirections[i].x, _targetHeight, _targetDirections[i].z);
        }
            

    }

    protected override void StartShowTargetZone()
    {
        DetermineTargetLocation();

        _currentTargetZones.Add(Instantiate(_targetZone, _currentTargetLocation, Quaternion.identity));

        base.StartShowTargetZone();
    }

    private void DetermineTargetLocation()
    {
        float currentFurthestDistance = float.MaxValue;

        foreach (Vector3 targetDir in _targetDirections)
        {
            float currentDist = Vector3.Distance(_storedTargetLocation, targetDir);
            if (currentDist < currentFurthestDistance)
            {
                _currentTargetLocation = targetDir;
                currentFurthestDistance = currentDist;
            }
                
        }
        
    }

    protected override void AbilityStart()
    {
        Instantiate(_crystalBarrage, _currentTargetLocation, Quaternion.identity);

        base.AbilityStart();
    }

    #region Upwards Projectile
    protected void StartUpwardsProjectileProcess()
    {

    }

    protected IEnumerator UpwardsProjectileProcess()
    {
        yield return new WaitForSeconds(_timeBetweenProjectiles);
    }

    protected void CreateUpwardsProjectile()
    {
        
    }

    #endregion

    #region DamageProjectile

    protected void StartDownwardsProjectileProcess()
    {

    }
    #endregion

}
