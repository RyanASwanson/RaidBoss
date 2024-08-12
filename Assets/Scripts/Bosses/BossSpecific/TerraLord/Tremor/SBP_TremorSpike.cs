using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_TremorSpike : BossProjectileFramework
{
    [SerializeField] private Vector3 _randomRotation;

    [Space]
    [SerializeField] private GameObject _spikeStartVFX;
    [SerializeField] private Transform _startVFXSpawnLocation;

    // Start is called before the first frame update
    void Start()
    {
        RandomSpawning();
        CreateStartVFX();
    }

    private void RandomSpawning()
    {
        transform.eulerAngles = new Vector3(Random.Range(-_randomRotation.x, _randomRotation.x),
            Random.Range(-_randomRotation.y, _randomRotation.y),
            Random.Range(-_randomRotation.z, _randomRotation.z));
    }

    private void CreateStartVFX()
    {
        Instantiate(_spikeStartVFX, transform.position, Quaternion.identity);
    }
}
