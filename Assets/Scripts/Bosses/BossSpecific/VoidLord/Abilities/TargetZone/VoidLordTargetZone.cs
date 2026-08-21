using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidLordTargetZone : BossTargetZone
{
    [SerializeField] private Material _rayOfHopeInRangeMat;
    [SerializeField] private Material _rayOfHopeAndHeroInRangeMat;

    [Space] 
    [SerializeField] private BossSpecificLayerHit _bossSpecificLayerHit;

    private int _bossSpecificLayersInRange = 0;
    private GameObject _lastSpecificLayerObject;
    
    protected override void DetermineTargetZoneMaterial()
    {
        if (_bossSpecificLayersInRange > 0)
        {
            if (_heroesInRange.Count > 0)
            {
                SetTargetZonesToRayAndHeroInRange();
            }
            else
            {
                SetTargetZonesToRayInRange();
            }
            
            return;
        }
        
        base.DetermineTargetZoneMaterial();
    }

    private void AddBossSpecificLayerInRange(GameObject collider)
    {
        _bossSpecificLayersInRange++;
        _lastSpecificLayerObject = collider;
        if (_bossSpecificLayersInRange == 1)
        {
            DetermineTargetZoneMaterial();
        }
        
    }

    private void RemoveBossSpecificLayerInRange(GameObject collider)
    {
        _bossSpecificLayersInRange--;
        if (_bossSpecificLayersInRange == 0)
        {
            DetermineTargetZoneMaterial();
        }
    }
    
    protected void SetTargetZonesToRayInRange()
    {
        AttemptAllTargetZonesToMaterial(_rayOfHopeInRangeMat);
    }

    protected void SetTargetZonesToRayAndHeroInRange()
    {
        AttemptAllTargetZonesToMaterial(_rayOfHopeAndHeroInRangeMat);
    }

    protected override void SubscribeToEvents()
    {
        if (_isSubscribedToEvents)
        {
            return;
        }
        
        _bossSpecificLayerHit.GetOnEnterBossSpecificLayer().AddListener(AddBossSpecificLayerInRange);
        _bossSpecificLayerHit.GetOnExitBossSpecificLayer().AddListener(RemoveBossSpecificLayerInRange);
        _bossSpecificLayerHit.GetOnDestructionOfBossSpecificLayer().AddListener(RemoveBossSpecificLayerInRange);
        
        base.SubscribeToEvents();
    }

    protected override void UnsubscribeFromEvents()
    {
        if (!_isSubscribedToEvents)
        {
            return;
        }
        
        _bossSpecificLayerHit.GetOnEnterBossSpecificLayer().RemoveListener(AddBossSpecificLayerInRange);
        _bossSpecificLayerHit.GetOnExitBossSpecificLayer().RemoveListener(RemoveBossSpecificLayerInRange);
        _bossSpecificLayerHit.GetOnDestructionOfBossSpecificLayer().RemoveListener(RemoveBossSpecificLayerInRange);
        
        base.UnsubscribeFromEvents();
    }
    
}
