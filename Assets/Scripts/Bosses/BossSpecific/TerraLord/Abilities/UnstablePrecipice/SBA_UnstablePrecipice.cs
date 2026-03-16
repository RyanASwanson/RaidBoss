using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_UnstablePrecipice : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _projectileSpawnRate;
    [SerializeField] private float _mapRadiusOffset;
    [SerializeField] private float _minimumBossDistance;
    [SerializeField] private float _minimumPreviousProjectileDistance;
    private float _projectileBossDistance;
    private float _previousProjectileDistance;

    [SerializeField] private float _projectileImpactDelay;
    private WaitForSeconds _projectileImpactWait;

    [Space] 
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _unstablePrecipiceAttack;
    
    private Coroutine _passiveAttackProcessCoroutine;

    private Vector3 _attackTargetLocation = Vector3.zero;
    private float _mapRadius;
    private int _sideMultiplier;

    private bool _isAbilityAttacking = false;
    
    public const int UNSTABLE_PRECIPICE_PROJECTILE_IMPACT_AUDIO_ID = 0;

    public void StartPassiveAttackProcess(bool isRightSide)
    {
        _sideMultiplier = isRightSide ? 1 : -1;
        
        StopPassiveAttackProcess();

        _isAbilityAttacking = true;
        
        _passiveAttackProcessCoroutine = StartCoroutine(PassiveAttackProcess());
    }

    public void StopPassiveAttackProcess()
    {
        if (!_passiveAttackProcessCoroutine.IsUnityNull())
        {
            StopCoroutine(_passiveAttackProcessCoroutine);
            _isAbilityAttacking = false;
        }
    }

    private IEnumerator PassiveAttackProcess()
    {
        float timeSinceLastAttack = 0;
        while (true)
        {
            timeSinceLastAttack += Time.deltaTime;

            if (timeSinceLastAttack >= _projectileSpawnRate)
            {
                timeSinceLastAttack-= _projectileSpawnRate;
                SpawnPassiveAttack();
            }
            yield return null;
        }
    }

    private void SpawnPassiveAttack()
    {
        _projectileBossDistance = 0;
        _previousProjectileDistance = 0;

        bool hasAcceptableExitFoundBeenFound = false;
        
        while (!hasAcceptableExitFoundBeenFound)
        {
            _attackTargetLocation.Set(Random.Range(-_mapRadius, _mapRadius),
                _specificAreaTarget.y, Random.Range(-_mapRadius, _mapRadius));

            _attackTargetLocation = Quaternion.Euler(0, -45, 0) * _attackTargetLocation;

            if (_sideMultiplier > 0 != _attackTargetLocation.x > 0)
            {
                _attackTargetLocation.Set(_attackTargetLocation.x *-1,_attackTargetLocation.y,_attackTargetLocation.z);
            }
            
            if (Mathf.Abs(_attackTargetLocation.x) + Mathf.Abs(_attackTargetLocation.z) < _minimumBossDistance)
            {
                continue;
            }

            bool hasCloseTargetZoneBeenFound = false;
            foreach (BossTargetZoneParent bossTargetZoneParent in _currentTargetZones)
            {
                _previousProjectileDistance = Vector3.Distance(_attackTargetLocation, bossTargetZoneParent.transform.position);
                if (_previousProjectileDistance < _minimumPreviousProjectileDistance)
                {
                    hasCloseTargetZoneBeenFound = true;
                    break;
                }
            }

            if (hasCloseTargetZoneBeenFound)
            {
                continue;
            }

            hasAcceptableExitFoundBeenFound = true;
        }
        
        
        BossTargetZoneParent targetZoneParent = Instantiate(_targetZone,_attackTargetLocation,Quaternion.identity).GetComponent<BossTargetZoneParent>();
        
        _currentTargetZones.Add(targetZoneParent);
        
        PlayTargetZoneSpawnedAudio();

        SBP_UnstablePrecipice unstablePrecipiceProjectile = Instantiate(_unstablePrecipiceAttack, _attackTargetLocation, Quaternion.identity).GetComponent<SBP_UnstablePrecipice>();
        
        unstablePrecipiceProjectile.SetUpProjectile(_myBossBase,_abilityID);
        
        StartCoroutine(RemoveTargetZoneAndProjectile(targetZoneParent, unstablePrecipiceProjectile));
    }

    private IEnumerator RemoveTargetZoneAndProjectile(BossTargetZoneParent targetZoneParent, SBP_UnstablePrecipice unstablePrecipiceProjectile)
    {
        yield return _projectileImpactWait;
        
        unstablePrecipiceProjectile.FloorImpact();
        
        _currentTargetZones.Remove(targetZoneParent);
        targetZoneParent.RemoveBossTargetZones();
    }
    
    #region BaseAbility
    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
        
        _projectileImpactWait = new WaitForSeconds(_projectileImpactDelay);
        
        _mapRadius = EnvironmentManager.Instance.GetMapRadius() + _mapRadiusOffset;
    }


    public override void StopBossAbility()
    {
        StopPassiveAttackProcess();
        RemoveTargetZones();
    }
    #endregion

    #region Getters

    public bool IsAbilityAttacking() => _isAbilityAttacking;

    #endregion
}
