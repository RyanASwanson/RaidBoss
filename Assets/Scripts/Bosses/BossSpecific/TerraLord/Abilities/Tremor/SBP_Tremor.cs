using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Terra Lord's Tremor ability projectiles
/// </summary>
public class SBP_Tremor : BossProjectileFramework
{
    [SerializeField] private float _spikeSeperationTime;

    [SerializeField] private List<GameObject> _spikes;

    [Space]
    [Header("Individual Spikes")]
    [SerializeField] private Vector3 _randomRotation;

    [Space]
    [SerializeField] private GameObject _spikeStartVFX;

    /// <summary>
    /// The process by which the individual spikes appear from the ground
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpikeSpawningProcess()
    {
        for(int i = 0; i <= _spikes.Count-1; i++)
        {
            //Spawns 2 spikes at a time
            SpawnSpike(_spikes[i]);
            i++;
            SpawnSpike(_spikes[i]);

            yield return new WaitForSeconds(_spikeSeperationTime);
        }
    }

    /// <summary>
    /// Spawns the individual spike
    /// </summary>
    /// <param name="spike"></param>
    private void SpawnSpike(GameObject spike)
    {
        spike.SetActive(true);

        RandomSpikeRotation(spike);
        CreateSpikeStartVFX(spike);
    }

    /// <summary>
    /// Spawns in the object with random variance in the rotation
    /// </summary>
    private void RandomSpikeRotation(GameObject spike)
    {
        spike.transform.eulerAngles = new Vector3(Random.Range(-_randomRotation.x, _randomRotation.x),
            Random.Range(-_randomRotation.y, _randomRotation.y),
            Random.Range(-_randomRotation.z, _randomRotation.z));
    }

    /// <summary>
    /// Creates vfx at the base of the spike
    /// </summary>
    private void CreateSpikeStartVFX(GameObject spike)
    {
        Instantiate(_spikeStartVFX, spike.transform.position, Quaternion.identity);
    }


    #region Base Ability
    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
        StartCoroutine(SpikeSpawningProcess());
    }
    #endregion
}
