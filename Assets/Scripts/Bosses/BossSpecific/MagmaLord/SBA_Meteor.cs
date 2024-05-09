using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Magma lord attack
/// Create a damage target zone then a meteor falls on that location
///     does initial damage and creates a damaging zone
/// </summary>
public class SBA_Meteor : SpecificBossAbilityFramework
{
    [SerializeField] private GameObject _targetZone;
}
