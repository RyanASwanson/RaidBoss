using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SBA_Isolation : SpecificBossAbilityFramework
{
    [Space]
    [SerializeField] private GameObject _targetSafeZone;
    [SerializeField] private GameObject _isolation;

    private BossSharedSafeAndTargetZone _currentTargetSafeZone;
    private SBP_IsolationTargetZone _isolationTargetZone;
    
    private const int ISOLATION_FAILED_AUDIO_ID = 0;
    
    /// <summary>
    /// If there is a hero in the safe zone the ability fails and doesn't deal damage
    /// </summary>
    private void AbilityFailed()
    {
        PlayFailedAudio();
        _isolationTargetZone.SpawnRay();
    }
    
    /// <summary>
    /// Plays the audio of the ability failing
    /// </summary>
    private void PlayFailedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[ISOLATION_FAILED_AUDIO_ID]);
    }
    
    #region Base Ability

    public override void AbilitySetUp(BossBase bossBase)
    {
        base.AbilitySetUp(bossBase);
    }

    protected override void StartShowTargetZone()
    {
        base.StartShowTargetZone();
        
        _isolationTargetZone = Instantiate(_targetSafeZone, _storedTarget.transform.position, Quaternion.identity)
            .GetComponent<SBP_IsolationTargetZone>();
        
        _isolationTargetZone.SetUpProjectile(_myBossBase,_abilityID);
        _isolationTargetZone.AdditionalSetUp(_storedTarget);

        _currentTargetSafeZone = _isolationTargetZone.GetStoredSafeAndTargetZone();

        /*//Spawns the target area
        _newestTargetZone = Instantiate(_targetZone, _storedTargetLocation, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        //Adds the target area to the list of target areas
        _currentTargetZones.Add(_newestTargetZone);*/
    }

    protected override void RemoveTargetZones()
    {
        _isolationTargetZone.RemoveTargetZones();
        base.RemoveTargetZones();
    }

    protected override void AbilityStart()
    {
        if (_storedTarget.IsUnityNull())
        {
            return;
        }
        
        if (_currentTargetSafeZone.GetIsHeroInSafeZone())
        {
            AbilityFailed();
            return;
        }

        GameObject isolation = Instantiate(_isolation, _isolationTargetZone.transform.position, Quaternion.identity);
        isolation.transform.position = new Vector3(_isolationTargetZone.transform.position.x, _specificAreaTarget.y, _isolationTargetZone.transform.position.z);
        
        base.AbilityStart();
    }
    
    protected override void AbilityDurationEnded()
    {
        base.AbilityDurationEnded();
    }

    public override void StopBossAbility()
    {
        base.StopBossAbility();
    }
    
    public override HeroBase GetSpecificHeroTarget()
    {
        List<HeroBase> heroes = HeroesManager.Instance.GetCurrentLivingHeroes();
        
        float currentDistance = 0;
        float currentMinimumDistance;
        float minimumDistance = 0;
        
        HeroBase currentHero = null;
        HeroBase currentFurthestHero = heroes[0];
        HeroBase furthestHero = null;
        

        if (heroes.Count == 1)
        {
            return heroes[0];
        }
        
        switch (heroes.Count)
        {
            case 1:
                return heroes[0];
            case 2:
                return heroes[Random.Range(0, heroes.Count)];
        }

        for (int i = 0; i < heroes.Count; i++)
        {
            currentMinimumDistance = float.MaxValue;

            for (int j = 0; j < heroes.Count; j++)
            {
                if (heroes[i] == heroes[j])
                {
                    continue;
                }
                
                currentDistance = Vector3.Distance(heroes[i].transform.position, heroes[j].transform.position);

                if (currentDistance < currentMinimumDistance)
                {
                    currentMinimumDistance = currentDistance;
                    currentFurthestHero = heroes[i];
                }
            }

            if (currentMinimumDistance > minimumDistance)
            {
                minimumDistance = currentMinimumDistance;
                furthestHero = currentFurthestHero;
            }
        }
        
        return furthestHero;
    }
    #endregion
}
