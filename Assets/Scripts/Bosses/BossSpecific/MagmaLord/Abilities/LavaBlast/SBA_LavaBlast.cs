using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord's Lava Blast ability.
/// Spawns a safe zone in the center, if no hero is in it when the ability starts
///     deals damage on the outer rim of the arena
/// </summary>
public class SBA_LavaBlast : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _safeZone;
    [SerializeField] private GameObject _targetZone;
    [SerializeField] private GameObject _lavaBlast;
    
    [SerializeField] private GameObject _failedVFX;

    private Queue<BossTargetZoneParent> _storedSafeZones = new Queue<BossTargetZoneParent>();
    
    private BossTargetZoneParent _currentDamageTargetZone;

    private const int LAVA_BLAST_FAILED_AUDIO_ID = 0;
    
    /// <summary>
    /// Checks if at least 1 hero is in the safe zone area
    /// </summary>
    /// <returns></returns>
    private bool IsHeroInSafeZone()
    {
        return _storedSafeZones.Dequeue().GetBossTargetZones()[0].DoesZoneContainHero();
    }

    /// <summary>
    /// If there is a hero in the safe zone the ability fails and doesn't deal damage
    /// </summary>
    private void AbilityFailed()
    {
        Instantiate(_failedVFX, _specificAreaTarget, Quaternion.identity);
        PlayFailedAudio();
    }

    /// <summary>
    /// Plays the audio of the ability failing
    /// </summary>
    private void PlayFailedAudio()
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[LAVA_BLAST_FAILED_AUDIO_ID]);
    }

    #region Base Ability

    /// <summary>
    /// Spawns in the safe zone 
    /// </summary>
    protected override void StartShowTargetZone()
    {
        BossTargetZoneParent newSafeZone = Instantiate(_safeZone, _specificAreaTarget, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        _currentDamageTargetZone = Instantiate(_targetZone, _specificAreaTarget, Quaternion.identity).GetComponent<BossTargetZoneParent>();
        
        newSafeZone.GetBossTargetZones()[0].GetOnTargetZoneSetToHeroInRange().AddListener(SetStateOfCurrentTargetZonesToDeactivated);
        newSafeZone.GetBossTargetZones()[0].GetOnTargetZoneSetToNoHeroInRange().AddListener(SetStateOfCurrentTargetZonesToActivated);

        _storedSafeZones.Enqueue(newSafeZone);

        _currentTargetZones.Add(newSafeZone);
        _currentTargetZones.Add(_currentDamageTargetZone);

        base.StartShowTargetZone();
    }

    protected override void SetStateOfCurrentTargetZonesToDeactivated()
    {
        _currentDamageTargetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(true);
    }

    protected override void SetStateOfCurrentTargetZonesToActivated()
    {
        _currentDamageTargetZone.SetTargetZoneDeactivatedStatesOfAllTargetZones(false);
    }

    protected override void AbilityStart()
    {
        //Checks if at least 1 hero is in the safe zone
        if (IsHeroInSafeZone())
        {
            //If there is, cancel the ability
            AbilityFailed();
            return;
        }

        //Spawn the damage zone
        Instantiate(_lavaBlast, _specificAreaTarget, Quaternion.identity);

        base.AbilityStart();
    }
    #endregion
}
