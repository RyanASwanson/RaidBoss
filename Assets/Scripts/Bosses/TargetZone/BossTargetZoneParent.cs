using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossTargetZoneParent : MonoBehaviour
{
    [SerializeField] private BossTargetZone[] _bossTargetZone;

    private bool _isDestroyingSelf = false;

    private void OnEnable()
    {
        if (_bossTargetZone.IsUnityNull() || _bossTargetZone.Length == 0)
        {
            _bossTargetZone = GetComponentsInChildren<BossTargetZone>();
        }
    }

    public void RemoveBossTargetZones()
    {
        if (_isDestroyingSelf)
        {
            return;
        }
        
        float longestDestroy = 0;
        foreach (BossTargetZone bossTargetZone in _bossTargetZone)
        {
            if (bossTargetZone.GetDisappearTime() > longestDestroy)
            {
                longestDestroy = bossTargetZone.GetDisappearTime();
            }
            bossTargetZone.RemoveTargetZone();
        }
        
        // Multiplied by 2 just to play it safe
        DestroyTargetZoneParent(longestDestroy * 2);
    }

    public void SetTargetZoneDeactivatedStatesOfAllTargetZones(bool shouldDeactivate)
    {
        foreach (BossTargetZone targetZone in _bossTargetZone)
        {
            targetZone.SetTargetZoneDeactivatedState(shouldDeactivate);
        }
    }

    private void DestroyTargetZoneParent(float destroyDelay)
    {
        Destroy(gameObject,destroyDelay);
        _isDestroyingSelf = true;
    }
    
    public BossTargetZone[] GetBossTargetZones() => _bossTargetZone;
}
