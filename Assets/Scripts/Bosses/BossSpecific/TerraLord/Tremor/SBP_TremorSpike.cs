using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_TremorSpike : BossProjectileFramework
{
    [SerializeField] private Vector3 _randomRotation;

    // Start is called before the first frame update
    void Start()
    {
        RandomSpawning();
    }

    private void RandomSpawning()
    {
        transform.eulerAngles = new Vector3(Random.Range(-_randomRotation.x, _randomRotation.x),
            Random.Range(-_randomRotation.y, _randomRotation.y),
            Random.Range(-_randomRotation.z, _randomRotation.z));
    }
}
