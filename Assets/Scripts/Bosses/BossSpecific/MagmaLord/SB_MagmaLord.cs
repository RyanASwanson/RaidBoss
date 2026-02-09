using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides the functionality for the Magma Lord boss
/// </summary>
public class SB_MagmaLord : SpecificBossFramework
{
    [Space] 
    [SerializeField] private GameObject _deathEffect;

    protected override void BossDied()
    {
        base.BossDied();
        Instantiate(_deathEffect, transform);
        //Instantiate(_deathEffect, transform.position, transform.rotation);
    }
}
