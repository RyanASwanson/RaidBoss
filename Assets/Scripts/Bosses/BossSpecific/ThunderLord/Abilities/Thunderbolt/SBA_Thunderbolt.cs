using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_Thunderbolt : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private float _maxAttacks;
    [SerializeField] private float _distanceBetweenAttacks;
    [SerializeField] private float _targetZoneMaxTimeBetweenAttacks;
    [SerializeField] private float _timeBetweenAttacks;
    private WaitForSeconds _waitTimeBetweenAttacks;

    private Coroutine _targetZoneSpawningProcess;
    private Coroutine _damageZoneActivationProcess;
    
    [Space]
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _damageZone;
    
    private List<SBP_Thunderbolt> _currentThunderbolts = new List<SBP_Thunderbolt>();

    private IEnumerator ThunderBoltTargetZoneCreationProcess()
    {
        CreateTargetZone();
        Vector3 lastAttackLocation = _storedTarget.transform.position;
        float timeSinceLastAttack = 0;
        
        while (!_storedTarget.IsUnityNull())
        {
            timeSinceLastAttack += Time.deltaTime;
            
            if (Vector3.Distance(lastAttackLocation, _storedTarget.transform.position) > _distanceBetweenAttacks
                || timeSinceLastAttack > _targetZoneMaxTimeBetweenAttacks)
            {
                lastAttackLocation = _storedTarget.transform.position;
                timeSinceLastAttack = 0;
                CreateTargetZone();
                if (_currentTargetZones.Count >= _maxAttacks)
                {
                    yield break;
                }
            }
            yield return null;
        }
    }

    private void CreateTargetZone()
    {
        BossTargetZoneParent targetZone =
            Instantiate(_targetZone, _storedTarget.transform.position, Quaternion.identity)
                .GetComponent<BossTargetZoneParent>();

        targetZone.transform.position = new Vector3(targetZone.transform.position.x, _specificAreaTarget.y,
            targetZone.transform.position.z);

        _currentTargetZones.Add(targetZone);
        
        CreateDamageZone(_currentTargetZones.Count - 1);
    }

    private void StopTargetZoneCreationProcess()
    {
        if (!_targetZoneSpawningProcess.IsUnityNull())
        {
            StopCoroutine(_targetZoneSpawningProcess);
        }
    }
    
    private IEnumerator ThunderboltDamageZoneActivationProcess()
    {
        for (int i = 0; i < _currentThunderbolts.Count; i++)
        {
            _currentThunderbolts[i].StartActivation();
            
            if (!_currentTargetZones[i].IsUnityNull())
            {
                _currentTargetZones[i].RemoveBossTargetZones();
            }
            
            yield return _waitTimeBetweenAttacks;
        }
    }

    private void CreateDamageZone(int targetZoneID)
    {
        GameObject storedDamageZone = Instantiate(_damageZone, _currentTargetZones[targetZoneID].transform.position, Quaternion.identity);
        //Sets up the projectile
        SBP_Thunderbolt thunderbolt = storedDamageZone.GetComponent<SBP_Thunderbolt>();
        thunderbolt.SetUpProjectile(_myBossBase, _abilityID);
        
        _currentThunderbolts.Add(thunderbolt);
    }

    private void StopAttackZoneActivationProcess()
    {
        if (!_damageZoneActivationProcess.IsUnityNull())
        {
            StopCoroutine(_damageZoneActivationProcess);
        }
    }
    
    #region BaseAbility

    public override void AbilitySetUp(BossBase bossBase)
    {
        _waitTimeBetweenAttacks = new WaitForSeconds(_timeBetweenAttacks);
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        _currentTargetZones.Clear();
        _currentThunderbolts.Clear();
        
        _targetZoneSpawningProcess = StartCoroutine(ThunderBoltTargetZoneCreationProcess());
        
        base.StartShowTargetZone();
    }
    
    /// <summary>
    /// Overridden to do nothing
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator TargetZonesProcess()
    {
        yield break;
    }

    protected override void AbilityStart()
    {
        StopTargetZoneCreationProcess();
        _damageZoneActivationProcess = StartCoroutine(ThunderboltDamageZoneActivationProcess());
        base.AbilityStart();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
        StopTargetZoneCreationProcess();
        StopAttackZoneActivationProcess();

        foreach (SBP_Thunderbolt thunderbolt in _currentThunderbolts)
        {
            thunderbolt.CancelThunderboltActivation();
        }
        
        RemoveTargetZones();
    }
    #endregion
}
