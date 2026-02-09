using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SBP_Frostbite : BossProjectileFramework
{
    [SerializeField] private float _projectileInterval;
    
    [SerializeField] private float _impactAudioPitchIncrease;

    [SerializeField] private GameObject _projectileStartVFX;

    [SerializeField] private List<GameObject> _projectiles;

    private void StartSpikeSpawningProcess()
    {
        StartCoroutine(SpikeSpawningProcess());
    }

    // <summary>
    /// The process by which the individual spikes appear from the ground
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpikeSpawningProcess()
    {
        for (int i = 0; i <= _projectiles.Count - 1; i++)
        {
            SpawnProjectile(_projectiles[i]);
            PlayProjectileSFX(i);
            yield return new WaitForSeconds(_projectileInterval);
        }
    }

    /// <summary>
    /// Spawns the individual projectile
    /// </summary>
    /// <param name="spike"></param>
    private void SpawnProjectile(GameObject projectile)
    {
        projectile.SetActive(true);

        CreateProjectileStartVFX(projectile);
    }

    /// <summary>
    /// Creates vfx at the base of the spike
    /// </summary>
    private void CreateProjectileStartVFX(GameObject spike)
    {
        Instantiate(_projectileStartVFX, spike.transform.position, Quaternion.identity);
    }

    private void PlayProjectileSFX(int frostBiteSet)
    {
        AudioManager.Instance.PlaySpecificAudio(
            AudioManager.Instance.AllSpecificBossAudio[_myBossBase.GetBossSO().GetBossID()].
                BossAbilityAudio[_abilityID].GeneralAbilityAudio[SBA_Frostbite.FROSTBITE_IMPACT_AUDIO_ID], out EventInstance eventInstance);
        
        eventInstance.getPitch(out float pitch);
        eventInstance.setPitch(pitch + (frostBiteSet * _impactAudioPitchIncrease));
    }

    #region Base Ability
    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="heroBase"></param>
    /// <param name= "newAbilityID"></param>
    public override void SetUpProjectile(BossBase bossBase, int newAbilityID)
    {
        base.SetUpProjectile(bossBase, newAbilityID);
        StartSpikeSpawningProcess();
    }
    #endregion
}
