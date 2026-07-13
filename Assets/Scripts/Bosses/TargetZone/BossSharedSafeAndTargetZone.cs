using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSharedSafeAndTargetZone : MonoBehaviour
{
    [SerializeField] private BossTargetZoneParent[] _bossSafeZoneParents;
    [SerializeField] private BossTargetZoneParent[] _bossTargetZoneParents;

    public void RemoveAllZones()
    {
        foreach (BossTargetZoneParent safeZone in _bossSafeZoneParents)
        {
            safeZone.RemoveBossTargetZones();
        }

        foreach (BossTargetZoneParent safeZone in _bossTargetZoneParents)
        {
            safeZone.RemoveBossTargetZones();
        }
        
        Destroy(gameObject,5f);
    }
    
    public bool GetIsHeroInSafeZone()
    {
        foreach (BossTargetZoneParent safeZone in _bossSafeZoneParents)
        {
            if (safeZone.GetDoAnyZonesContainHero())
            {
                return true;
            }
        }

        return false;
    }
}
