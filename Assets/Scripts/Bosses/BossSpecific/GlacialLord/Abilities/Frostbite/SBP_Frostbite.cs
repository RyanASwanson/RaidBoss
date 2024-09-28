using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Frostbite : BossProjectileFramework
{
    [SerializeField] private float _projectileInterval;

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

    #region Base Ability
    /// <summary>
    /// Called when projectile is created
    /// Provides the projectile with any additional information it may need
    /// </summary>
    /// <param name="heroBase"></param>
    public override void SetUpProjectile(BossBase bossBase)
    {
        base.SetUpProjectile(bossBase);
        StartSpikeSpawningProcess();
    }
    #endregion
}
