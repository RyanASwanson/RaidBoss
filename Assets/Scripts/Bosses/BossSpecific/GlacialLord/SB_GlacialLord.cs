using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SB_GlacialLord : SpecificBossFramework
{
    [SerializeField] private Vector3 _iceCrownSpawnLocation;
    [SerializeField] private GameObject _iceCrown;

    private GlacialLordIceCrown _currentIceCrown;

    protected override void StartFight()
    {
        base.StartFight();
        CreateIceCrown();
    }

    private void CreateIceCrown()
    {
        GameObject crown = Instantiate(_iceCrown, _iceCrownSpawnLocation, Quaternion.identity);

        _currentIceCrown = crown.GetComponent<GlacialLordIceCrown>();
    }
}
