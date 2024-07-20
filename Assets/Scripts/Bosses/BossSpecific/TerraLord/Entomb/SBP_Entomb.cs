using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBP_Entomb : BossProjectileFramework
{
    [SerializeField] private GameObject _entombWalls;

    // Start is called before the first frame update
    void Start()
    {
        CreateWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateWalls()
    {
        Instantiate(_entombWalls, transform.position, transform.rotation);
    }
}
